using DiegoG.Finance;
using DiegoG.Finance.Serialization.MessagePackFormatters;
using MessagePack;
using MessagePack.Formatters;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: MessagePackKnownFormatter(typeof(WorkSheetFormatter))]
[assembly: MessagePackKnownFormatter(typeof(WorkSheetHeader))]

namespace DiegoG.Finance.Serialization.MessagePackFormatters;

public sealed class WorkSheetFormatter : IMessagePackFormatter<WorkSheet?>, IMessagePackFormatter<WorkSheetHeader>
{
    private WorkSheetFormatter() { }

    public static readonly WorkSheetFormatter Instance = new();

    /*
     * MessagePack structure
     * 
     * Basic Info Array - Array Header
     * Version - int
     * Title - string
     * Created.DateTime - DateTime
     * Created.Offset - long
     * Everything Else Array - Array Header
     * Rest of the stuff
     */

    public void Serialize(ref MessagePackWriter writer, WorkSheet? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(5); 

        if (string.IsNullOrWhiteSpace(value.Title))
            writer.WriteNil();
        else
        {
            writer.Write(value.Version); // version
            byte[]? rental = null;
            try
            {
                var len = Encoding.UTF8.GetByteCount(value.Title);
                Span<byte> str = len > 1024 ? (rental = ArrayPool<byte>.Shared.Rent(len)).AsSpan(0, len) : stackalloc byte[len];
                Encoding.UTF8.GetBytes(value.Title, str);

                writer.WriteStringHeader(str.Length);
                Span<byte> span = writer.GetSpan(str.Length);
                str.CopyTo(span);
                writer.Advance(str.Length);
            }
            finally
            {
                if (rental is not null)
                    ArrayPool<byte>.Shared.Return(rental);
            } // title

            writer.Write(value.Created.DateTime); // DateTime
            writer.Write(value.Created.Offset.Ticks); // Offset

            // Everything else
            writer.WriteArrayHeader(1);

            if (value.Version == 1)
            {
                MessagePackSerializer.Serialize(ref writer, value.SpendingTrackers, options);
            }
            else
                throw new MessagePackSerializationException($"Unrecognized or unsupported WorkSheet version: {value.Version}");
        }
    }

    public WorkSheet? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.IsNil)
            return null;

        if (reader.ReadArrayHeader() == 5)
        {
            var version = reader.ReadInt32();
            var title = reader.ReadString();
            var created = reader.ReadDateTime();
            var createdOffset = reader.ReadInt64(); // Ticks
            reader.ReadArrayHeader();

            if (version == 1)
            {
                var tracker = MessagePackSerializer.Deserialize<SpendingTrackerSheet>(ref reader, options);
                return new WorkSheet(1, tracker)
                {
                    Created = new DateTimeOffset(created, TimeSpan.FromTicks(createdOffset)),
                    Title = string.IsNullOrWhiteSpace(title) ? null : title
                };
            }
            else
                throw new MessagePackSerializationException($"Unrecognized or unsupported WorkSheet version: {version}");
        }
        else
            throw new MessagePackSerializationException("The first array header of WorkSheet is not 5");
    }

    public void Serialize(ref MessagePackWriter writer, WorkSheetHeader value, MessagePackSerializerOptions options)
    {
        throw new MessagePackSerializationException("WorkSheetContainer objects are not meant to be serialized");
    }

    WorkSheetHeader IMessagePackFormatter<WorkSheetHeader>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.IsNil)
            return default;

        if (reader.ReadArrayHeader() == 5)
        {
            var version = reader.ReadInt32();
            var title = reader.ReadString();
            var createdDateTime = reader.ReadDateTime();
            var createdOffset = reader.ReadInt64(); // Ticks
            reader.ReadArrayHeader();

            var created = new DateTimeOffset(createdDateTime, new TimeSpan(createdOffset));
            return new WorkSheetHeader()
            {
                Created = created,
                IsNotEmpty = true,
                IsPasswordProtected = false,
                Name = title ?? WorkSheet.GetPlaceholderName(created),
                Version = version
            };
        }
        else
            throw new MessagePackSerializationException("The first array header of WorkSheet is not 5");
    }
}
