using SpaceAfterYou.SoulWorker.ResStructureExtractor.DataTypes.FileInfo;

namespace SpaceAfterYou.SoulWorker.ResStructureExtractor.Utils.FileUtils;

internal static class TableBodyUtils
{
    internal static bool StartsWith(ReadOnlyMemory<byte> memory, ReadOnlyMemory<byte?> pattern) => Enumerable
        .Range(0, pattern.Length)
        .All((i) =>
        {
            var b = pattern.Span[i];
            return b is null || memory.Span[i] == b;
        });

    internal static bool ContainsPattern(ReadOnlyMemory<byte> memory, ReadOnlyMemory<byte?> pattern) => Enumerable
        .Range(0, memory.Length - pattern.Length)
        .Any(v => StartsWith(memory[v..], pattern));

    internal static int OffsetByPattern(ReadOnlyMemory<byte> memory, ReadOnlyMemory<byte?> pattern) => Enumerable
        .Range(0, memory.Length - pattern.Length)
        .First(v => StartsWith(memory[v..], pattern)) + pattern.Length;

    internal static Range BodyFrom(ReadOnlyMemory<byte> memory, int offset)
    {
        var mem = memory[offset..];

        var begin = offset + Enumerable
            .Range(0, mem.Length)
            .First(v => mem[v..].Span.StartsWith(_beginBlock.Span));

        var end = OffsetByPattern(memory[begin..], _endBlock);

        return new Range(begin, begin + end);
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
                return GetFunctionPatterns().Any(v => m.Span.StartsWith(v.Span));
            });
    }

    private static TableReadFunctionFileInfo CreateInfo(ReadOnlyMemory<byte> memory, int v, Range range)
    {
        var callOffset = BitConverter.ToInt32(memory.Span[CreateRange(v)].ToArray(), 0);

        // Start of CALL instruction
        // E8 XX XX XX XX
        var fileOffset = range.Start.Value + v + _pattern.Length - 1;

        return new TableReadFunctionFileInfo(fileOffset, callOffset);
    }

    private static Range CreateRange(int offset)
    {
        int begin = offset + _pattern.Length;
        return new Range(begin, begin + 4);
    }

    private static IEnumerable<ReadOnlyMemory<byte>> GetFunctionPatterns()
    {
        // string
        // push    ebp
        // mov     ebp, esp
        // mov     eax, 104Ch
        yield return new byte[] { 0x55, 0x8B, 0xEC, 0xB8, 0x4C, 0x10, 0x00, 0x00 };

        // string
        // push    ebp
        // mov     ebp, esp
        // sub     esp, 44Ch
        yield return new byte[] { 0x55, 0x8B, 0xEC, 0x81, 0xEC, 0x4C, 0x04, 0x00, 0x00 };

        // int
        // push    ebp
        // mov     ebp, esp
        // sub     esp, 8
        yield return new byte[] { 0x55, 0x8B, 0xEC, 0x83, 0xEC, 0x08 };
    }

    /// <summary>
    ///    mov     [ebp+var_XX], 0
    /// </summary>
    private readonly static ReadOnlyMemory<byte> _beginBlock = new byte[] { 0xC7, 0x45, 0xA0, 0x00, 0x00, 0x00, 0x00 };

    /// <summary>
    ///    jmp     loc_XXXXXX
    /// </summary>
    private readonly static ReadOnlyMemory<byte?> _endBlock = new byte?[] { 0x8B, 0xE5, 0x5D, 0xC3 };

    /// <summary>
    ///    lea     edx, [ebp+var_]    ; 8D 4D A4
    ///    call    sub_               ; E8 XX XX XX XX
    /// </summary>
    private readonly static ReadOnlyMemory<byte> _pattern = new byte[] { 0x8D, 0x4D, 0xA4, 0xE8 };
}
