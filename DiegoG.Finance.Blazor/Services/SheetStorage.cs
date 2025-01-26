using Blazored.LocalStorage;
using DiegoG.Finance.Serialization.MessagePackFormatters;
using MessagePack;
using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DiegoG.Finance.Blazor.Services;

public class WorkSheetStorage(ILocalStorageService storage)
{
    public async IAsyncEnumerable<WorkSheetHeader> EnumerateAvailableWorksheets()
    {
        foreach (var key in await storage.KeysAsync())
        {
            var (success, header) = await TryReadWorksheetInfo(key);
            if (success is false)
                continue;
            yield return header;
        }
    }

    public async ValueTask<(bool Success, WorkSheetHeader Header)> TryReadWorksheetInfo(string key)
    {
        var dat = await storage.GetItemAsStringAsync(key);
        return string.IsNullOrWhiteSpace(dat) ? default : (true, await MessagePackFinance.ReadPackedBase64Header(dat) with { Path = key });
    }

    public async ValueTask<WorkSheet?> LoadWorkSheet(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var dat = await storage.GetItemAsStringAsync(key);
        return string.IsNullOrWhiteSpace(dat) ? null : await MessagePackFinance.ReadPackedBase64(dat);
    }

    public async ValueTask<WorkSheet?> LoadWorkSheet(WorkSheetHeader header)
    {
        if (string.IsNullOrWhiteSpace(header.Path))
            throw new ArgumentException($"The WorkSheetHeader does not have a path associated to it", nameof(header));

        var dat = await storage.GetItemAsStringAsync(header.Path);
        return string.IsNullOrWhiteSpace(dat) ? null : await MessagePackFinance.ReadPackedBase64(dat);
    }

    public async Task StoreSheet(WorkSheet sheet)
        => await storage.SetItemAsStringAsync(sheet.Title, await MessagePackFinance.PackAndWriteToBase64(sheet));

    public static string GetDefaultFileName(WorkSheet sheet)
        => $"{sheet.Title}.dgfinance";
}
