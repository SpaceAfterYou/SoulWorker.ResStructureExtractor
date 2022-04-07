using SoulWorker.ResStructureExtractor.DataTypes.MemoryInfo;

namespace SoulWorker.ResStructureExtractor.DataTypes.FileInfo;

internal readonly struct NameUsageFileInfo
{
    #region Fields

    internal NameMemoryInfo Name { get; }
    internal int Offset { get; }

    #endregion Fields

    #region Constructors

    internal NameUsageFileInfo(NameMemoryInfo name, int offset)
    {
        Name = name;
        Offset = offset;
    }

    #endregion Constructors
}

internal readonly struct LoopFileInfo
{
    #region Fields

    internal NameMemoryInfo Name { get; }
    internal Range Range { get; }

    #endregion Fields

    #region Constructors

    internal LoopFileInfo(NameMemoryInfo name, Range range)
    {
        Name = name;
        Range = range;
    }

    #endregion Constructors
}
