using SpaceAfterYou.SoulWorker.ResStructureExtractor;
using SpaceAfterYou.SoulWorker.ResStructureExtractor.DataTypes.FileInfo;
using System.Text;

namespace SpaceAfterYou.SoulWorker.ResStructureExtractor.Utils.FileUtils;

internal static class TableNameUtils
{
    internal static int UsageOffset(ReadOnlyMemory<byte> buffer, int address)
    {
        var bytes = BitConverter.GetBytes(address);

        return Enumerable
            .Range(0, buffer.Length - 7)
            .FirstOrDefault(v =>
            {
                // search 0x51 0x68 0xXX 0xXX 0xXX 0xXX 0x8B 0x95

                // byte sequence
                var bs = buffer[v..].Span;
                if (!bs.StartsWith(bytes)) return false;

                // byte prev
                var bp = buffer.Span[v - 1];
                if (bp != (byte)Asm.Push) return false;

                // byte prev prev 🍆
                var bpp = buffer.Span[v - 2];
                if (bpp != (byte)Asm.PushEcx) return false;

                // byte next
                var bn = bs[4];
                if (bn != 0x8B) return false;

                // byte next next
                var bnn = bs[5];
                if (bnn != 0x95) return false;

                return true;
            });
    }

    internal static IEnumerable<NameFileInfo> All(ReadOnlyMemory<byte> buffer)
    {
        for (int begin = 0; begin < buffer.Length; ++begin)
        {
            var memory = buffer[begin..];

            var variant = _tableStartNameVariants.FirstOrDefault(v => memory.Span.StartsWith(v.Span));
            if (variant.Length == 0) continue;

            var span = memory.Span;
            for (int end = variant.Length; end < span.Length; ++end)
            {
                var b = span[end];

                if (char.IsLetter((char)b) || char.IsDigit((char)b) || b == Underscore) continue;

                // Next byte always must be a EOS
                // But in some cases it may be different
                if (b != EndOfString) break;

                var name = Encoding.ASCII.GetString(span[..end]);
                yield return new NameFileInfo(name, begin);

                begin += end;
                break;
            }
        }
    }

    private const byte Underscore = 0x5F;
    private const byte EndOfString = byte.MinValue;

    private static readonly IReadOnlyList<ReadOnlyMemory<byte>> _tableStartNameVariants = new ReadOnlyMemory<byte>[]
    {
        // 74 62 5F - "tb_"
        new byte[] { 0x74, 0x62, 0x5F },
        
        // 54 62 5F - "Tb_"
        new byte[] { 0x54, 0x62, 0x5F },
        
        // 54 42 5F - "TB_"
        new byte[] { 0x54, 0x42, 0x5F }
    };
}
