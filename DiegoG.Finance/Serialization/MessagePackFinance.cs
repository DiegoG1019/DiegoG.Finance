using MessagePack;
using MessagePack.Resolvers;
using System.Buffers.Text;
using System.Buffers;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;

namespace DiegoG.Finance.Serialization;

public static class MessagePackFinance
{
    public static async Task PackAndWriteBinary(WorkSheet workSheet, Stream output)
    {
        using var archive = new ZipArchive(output, ZipArchiveMode.Create);

        var headerEntry = archive.CreateEntry("Header");
        using (var headerStream = headerEntry.Open())
        {
            var header = new WorkSheetHeader()
            {
                IsNotEmpty = true,
                Created = workSheet.Created,
                Name = workSheet.Title,
                Pages = workSheet.Book.Count,
                Version = workSheet.Version,
                Currency = workSheet.Currency
            };

            await MessagePackSerializer.SerializeAsync(headerStream, workSheet);
        }

        var typesEntry = archive.CreateEntry("TypesAndCategories");
        using (var expenseStream = typesEntry.Open())
            await MessagePackSerializer.SerializeAsync(expenseStream, workSheet.ExpenseTypesAndCategories.GetInfo());

        var bookEntry = archive.CreateEntry("Book");
        using (var bookStream = bookEntry.Open())
            await MessagePackSerializer.SerializeAsync(bookStream, workSheet.Book.GetInfo());
    }

    public static async Task<WorkSheet> ReadPackedBinary(Stream input)
    {
        using var archive = new ZipArchive(input, ZipArchiveMode.Read);

        var headerEntry = archive.GetEntry("Header") ?? throw new InvalidDataException("Could not read a WorkSheet header from the input");
        WorkSheetHeader header;
        using (var headerStream = headerEntry.Open())
            header = await MessagePackSerializer.DeserializeAsync<WorkSheetHeader>(headerStream);

        var typesEntry = archive.GetEntry("TypesAndCategories") ?? throw new InvalidDataException("Could not read the WorkSheet's types and categories from the input");
        List<ExpenseType.Info> expenseTypes;
        using (var expenseStream = typesEntry.Open())
            expenseTypes = await MessagePackSerializer.DeserializeAsync<List<ExpenseType.Info>>(expenseStream);

        var bookEntry = archive.GetEntry("Book") ?? throw new InvalidDataException("Could not read the WorkSheet's book-keeping info from the input");
        WorkSheetBook.Info bookPages;
        using (var bookStream = bookEntry.Open())
            bookPages = await MessagePackSerializer.DeserializeAsync<WorkSheetBook.Info>(bookStream);

        return new WorkSheet(
            header.Currency,
            expenseTypes,
            bookPages
        )
        {
            Created = header.Created,
            Title = header.Name
        };
    }

    public static async Task<WorkSheetHeader> ReadPackedBinaryHeader(Stream input)
    {
        using var archive = new ZipArchive(input, ZipArchiveMode.Read);

        var headerEntry = archive.GetEntry("Header") ?? throw new InvalidDataException("Could not read a WorkSheet header from the input");
        using var headerStream = headerEntry.Open();
        return await MessagePackSerializer.DeserializeAsync<WorkSheetHeader>(headerStream);
    }

    public static async Task<string> PackAndWriteToBase64(WorkSheet workSheet)
    {
        using var mem = new MemoryStream(4096);

        await PackAndWriteBinary(workSheet, mem);

        if (mem.TryGetBuffer(out var buffer) is false)
            Debug.Fail("Could not obtain the MemoryStream buffer");

        return Convert.ToBase64String(buffer);
    }

    public static async Task<WorkSheet> ReadPackedBase64(string str)
    {
        var stringByteLen = Encoding.UTF8.GetByteCount(str);
        var b64Buffer = ArrayPool<byte>.Shared.Rent(stringByteLen);
        try
        {
            int written = Encoding.UTF8.GetBytes(str, b64Buffer);
            var b64Status = Base64.DecodeFromUtf8InPlace(b64Buffer.AsSpan(0, written), out written); // It's now raw bytes, but they're still compressed
            Debug.Assert(b64Status == OperationStatus.Done);

            using var mem = new MemoryStream(b64Buffer, 0, written);
            return await ReadPackedBinary(mem);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(b64Buffer);
        }
    }

    public static async ValueTask<WorkSheetHeader> ReadPackedBase64Header(string str)
    {
        var stringByteLen = Encoding.UTF8.GetByteCount(str);
        var b64Buffer = ArrayPool<byte>.Shared.Rent(stringByteLen);
        try
        {
            int written = Encoding.UTF8.GetBytes(str, b64Buffer);
            var b64Status = Base64.DecodeFromUtf8InPlace(b64Buffer.AsSpan(0, written), out written); // It's now raw bytes, but they're still compressed
            Debug.Assert(b64Status == OperationStatus.Done);

            using var mem = new MemoryStream(b64Buffer, 0, written);
            return await ReadPackedBinaryHeader(mem);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(b64Buffer);
        }
    }
}
