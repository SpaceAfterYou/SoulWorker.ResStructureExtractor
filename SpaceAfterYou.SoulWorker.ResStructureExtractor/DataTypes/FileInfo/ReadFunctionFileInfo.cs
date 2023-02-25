using SpaceAfterYou.SoulWorker.ResStructureExtractor.Utils;

namespace SpaceAfterYou.SoulWorker.ResStructureExtractor.DataTypes.FileInfo;

internal readonly struct TableReadFunctionFileInfo
{
    internal int FileOffset { get; }

    /// <summary>
    /// Relative to current position
    /// </summary>
    internal int CallOffset { get; }

    internal int Offset => AsmUtils.OffsetFromCalc(FileOffset, CallOffset);

    internal TableReadFunctionFileInfo(int fileOffset, int callOffset)
    {
        FileOffset = fileOffset;
        CallOffset = callOffset;
    }
}
