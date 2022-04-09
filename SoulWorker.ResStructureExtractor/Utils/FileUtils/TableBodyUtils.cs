using SoulWorker.ResStructureExtractor.DataTypes.FileInfo;

namespace SoulWorker.ResStructureExtractor.Utils.FileUtils;

internal static class TableBodyUtils
{
    #region Methods

    internal static ValueTask<Range> BodyFrom(ReadOnlyMemory<byte> memory, int offset)
    {
        var mem = memory[offset..];

        var begin = Enumerable
            .Range(0, mem.Length)
            .First(v => mem[v..].Span.StartsWith(_beginBlock.Span));

        var end = Enumerable
            .Range(begin, mem.Length - begin)
            .First(v =>
            {
                var m = mem[v..];
                for (int i = 0; i < _endBlock.Length; i++)
                {
                    var b = _endBlock.Span[i];
                    if (b is null) continue;

                    if (m.Span[i] != b) return false;
                }

                return true;
            });

        return ValueTask.FromResult(new Range(offset + begin, offset + end));
    }

    internal static IEnumerable<TableReadFunctionFileInfo> FunctionsFrom(ReadOnlyMemory<byte> memory, Range range)
    {
        var mem = memory[range];

        return Enumerable
            .Range(0, mem.Length)
            .Where(v => mem[v..].Span.StartsWith(_pattern.Span))
            .Select(v => CreateInfo(mem, v, range))
            .Where(v => 
            {
                var m = memory[v.Offset..];
                return _functionPattern.Any(v => m.Span.StartsWith(v.Span));
            });
    }

    #endregion Methods

    #region Private Methods

    private static TableReadFunctionFileInfo CreateInfo(ReadOnlyMemory<byte> memory, int v, Range range)
    {
        var span = memory[CreateRange(v)].Span;
        var value = BitConverter.ToInt32(span);

        // Start of CALL instruction
        // E8 XX XX XX XX
        var offset = range.Start.Value + v + _pattern.Length - 1;

        return new TableReadFunctionFileInfo(offset, value);
    }

    private static Range CreateRange(int offset)
    {
        int begin = offset + _pattern.Length;
        return new Range(begin, begin + 4);
    }

    #endregion Private Methods

    #region Private Fields

    private readonly static IReadOnlyList<ReadOnlyMemory<byte>> _functionPattern = new ReadOnlyMemory<byte>[]
    {
        // string
        // push    ebp
        // mov     ebp, esp
        // mov     eax, 104Ch
        new byte[]{ 0x55, 0x8B, 0xEC, 0xB8, 0x4C, 0x10, 0x00, 0x00 },

        // string
        // push    ebp
        // mov     ebp, esp
        // sub     esp, 44Ch
        new byte[]{ 0x55, 0x8B, 0xEC, 0x81, 0xEC, 0x4C, 0x04, 0x00, 0x00 },

        // int
        // push    ebp
        // mov     ebp, esp
        // sub     esp, 8
        new byte[] { 0x55, 0x8B, 0xEC, 0x83, 0xEC, 0x08 }
    };

    /// <summary>
    ///    mov     [ebp+var_XX], 0
    /// </summary>
    private readonly static ReadOnlyMemory<byte> _beginBlock = new byte[] { 0xC7, 0x45, 0xA0, 0x00, 0x00, 0x00, 0x00 };

    /// <summary>
    ///    jmp     loc_XXXXXX
    /// </summary>
    private readonly static ReadOnlyMemory<byte?> _endBlock = new byte?[] { 0xE9, null, null, 0xFF, 0xFF };

    /// <summary>
    ///    lea     edx, [ebp+var_]    ; 8D 4D A4
    ///    call    sub_               ; E8 XX XX XX XX
    /// </summary>
    private readonly static ReadOnlyMemory<byte> _pattern = new byte[] { 0x8D, 0x4D, 0xA4, 0xE8 };

    #endregion Private Fields
}
