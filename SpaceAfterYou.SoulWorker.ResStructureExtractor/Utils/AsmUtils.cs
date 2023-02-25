namespace SpaceAfterYou.SoulWorker.ResStructureExtractor.Utils;

internal static class AsmUtils
{
    // 0x5 - skip CALL instruction (E8 XX XX XX XX)
    internal static int OffsetFromCalc(int @base, int offset) => @base + offset + 0x5;
}
