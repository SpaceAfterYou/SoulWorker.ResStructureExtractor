namespace SoulWorker.ResStructureExtractor.DataTypes.FileInfo;

internal readonly struct TableReadFunctionFileInfo
{
    #region Fields

    internal int FileOffset { get; }

    /// <summary>
    /// Relative to current position
    /// </summary>
    internal int CallOffset { get; }

    // 0x5 - skip CALL instruction (E8 XX XX XX XX)
    internal int Offset => FileOffset + CallOffset + 0x5;

    #endregion Fields

    #region Constructors

    internal TableReadFunctionFileInfo(int fileOffset, int callOffset)
    {
        FileOffset = fileOffset;
        CallOffset = callOffset;
    }

    #endregion Constructors
}
