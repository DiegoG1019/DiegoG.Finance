using MessagePack;
using MessagePack.Resolvers;
using System.Buffers.Text;
using System.Buffers;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;

namespace DiegoG.Finance.Serialization.MessagePackFormatters;

public static class MessagePackFinance
{
    public static MessagePackSerializerOptions WithFinanceFormatters(this MessagePackSerializerOptions options)
        => options.WithResolver(CompositeResolver.Create(FinanceResolver.Instance, options.Resolver));

    public static async Task PackAndWriteBinary(WorkSheet workSheet, Stream output)
    {
        using var zip = new GZipStream(output, CompressionLevel.SmallestSize);
        await MessagePackSerializer.SerializeAsync(
            zip,
            workSheet
        );
    }

    public static async Task<WorkSheet> ReadPackedBinary(Stream input)
    {
        using var zip = new GZipStream(input, CompressionMode.Decompress);
        return await MessagePackSerializer.DeserializeAsync<WorkSheet>(zip);
    }

    public static async Task<WorkSheetHeader> ReadPackedBinaryHeader(Stream input)
    {
        using var zip = new GZipStream(input, CompressionMode.Decompress);
        return await MessagePackSerializer.DeserializeAsync<WorkSheetHeader>(zip);
    }

    public static async Task<string> PackAndWriteToBase64(WorkSheet workSheet)
    {
        using var mem = new MemoryStream(4096);
        using var zip = new GZipStream(mem, CompressionLevel.SmallestSize);

        await MessagePackSerializer.SerializeAsync(
            zip,
            workSheet
        );

        if (mem.TryGetBuffer(out var buffer) is false)
            Debug.Fail("Could not obtain the MemoryStream buffer");

        return Convert.ToBase64String(buffer);
    }

    public static ValueTask<WorkSheet> ReadPackedBase64(string str)
    {
        byte[]? rented = null;
        try
        {
            var stringByteLen = Encoding.UTF8.GetByteCount(str);
            var b64Buffer = new byte[stringByteLen];

            Encoding.UTF8.GetBytes(str, b64Buffer);
            var b64Status = Base64.DecodeFromUtf8InPlace(b64Buffer, out int written); // It's now raw bytes, but they're still compressed
            Debug.Assert(b64Status == OperationStatus.Done);

            using var mem = new MemoryStream(b64Buffer, 0, written);
            using var zip = new GZipStream(mem, CompressionMode.Decompress);
            return MessagePackSerializer.DeserializeAsync<WorkSheet>(zip);
        }
        finally
        {
            if (rented is not null)
                ArrayPool<byte>.Shared.Return(rented);
        }
    }

    public static ValueTask<WorkSheetHeader> ReadPackedBase64Header(string str)
    {
        byte[]? rented = null;
        try
        {
            var stringByteLen = Encoding.UTF8.GetByteCount(str);
            var b64Buffer = new byte[stringByteLen];

            Encoding.UTF8.GetBytes(str, b64Buffer);
            var b64Status = Base64.DecodeFromUtf8InPlace(b64Buffer, out int written); // It's now raw bytes, but they're still compressed
            Debug.Assert(b64Status == OperationStatus.Done);

            using var mem = new MemoryStream(b64Buffer, 0, written);
            using var zip = new GZipStream(mem, CompressionMode.Decompress);
            return MessagePackSerializer.DeserializeAsync<WorkSheetHeader>(zip);
        }
        finally
        {
            if (rented is not null)
                ArrayPool<byte>.Shared.Return(rented);
        }
    }
}
