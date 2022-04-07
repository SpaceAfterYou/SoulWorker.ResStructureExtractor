namespace SoulWorker.ResStructureExtractor.DataTypes.FileInfo;

internal readonly struct NameFileInfo
{
    #region Fields

    internal string Name { get; }
    internal int Offset { get; }

    #endregion Fields

    #region Constructors

    internal NameFileInfo(string name, int offset)
    {
        Name = name;
        Offset = offset;
    }

    #endregion Constructors
}

// https://youtu.be/BEcv39gKAkw
// https://youtu.be/0BdcUEEvi1M