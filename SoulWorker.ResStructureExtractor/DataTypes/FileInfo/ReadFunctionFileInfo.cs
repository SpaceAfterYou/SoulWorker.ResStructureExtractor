using SoulWorker.ResStructureExtractor.Utils;

namespace SoulWorker.ResStructureExtractor.DataTypes.FileInfo;

internal readonly struct TableReadFunctionFileInfo
{
    #region Fields

    internal int FileOffset { get; }

    /// <summary>
    /// Relative to current position
    /// </summary>
    internal int CallOffset { get; }

    internal int Offset => AsmUtils.OffsetFromCalc(FileOffset, CallOffset);

    #endregion Fields

    #region Constructors

    internal TableReadFunctionFileInfo(int fileOffset, int callOffset)
    {
        FileOffset = fileOffset;
        CallOffset = callOffset;
    }

    #endregion Constructors
}
