using SoulWorker.ResStructureExtractor.DataTypes.FileInfo;
using System.Text;

namespace SoulWorker.ResStructureExtractor.Utils.FileUtils;

internal static class TableNameUtils
{
    #region Methods

    internal static int UsageOffset(ReadOnlyMemory<byte> buffer, int address)
    {
        var bytes = BitConverter.GetBytes(address);

        return Enumerable
            .Range(0, buffer.Length)
            .First(v => buffer[v..].Span.StartsWith(bytes) && buffer.Span[v - 1] == (byte)AssemblyOpcodes.Push);
    }

    internal static IEnumerable<NameFileInfo> All(ReadOnlyMemory<byte> buffer)
    {
        for (int begin = 0; begin < buffer.Length; ++begin)
        {
            var memory = buffer[begin..];
            var span = memory.Span;

            if (!span.StartsWith(_tableStartName.Span)) continue;

            for (int end = _tableStartName.Length; end < span.Length; ++end)
            {
                var b = span[end];

                if (char.IsLetter((char)b) || b == Underscore) continue;

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

    #endregion Methods

    #region Private Constants

    private const byte Underscore = 0x5F;
    private const byte EndOfString = byte.MinValue;

    #endregion Private Constants

    #region Private Static Fields

    /// <summary>
    /// string - "tb_"
    /// 74 62 5F
    /// </summary>
    private static readonly ReadOnlyMemory<byte> _tableStartName = new byte[] { 0x74, 0x62, 0x5F };

    #endregion Private Static Fields
}
