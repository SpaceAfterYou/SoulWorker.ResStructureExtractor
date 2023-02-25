using SpaceAfterYou.SoulWorker.ResStructureExtractor.DataTypes.MemoryInfo;

namespace SpaceAfterYou.SoulWorker.ResStructureExtractor.DataTypes.FileInfo;

internal readonly struct LoopFileInfo
{
    internal NameMemoryInfo Name { get; }
    internal Range Range { get; }

    internal LoopFileInfo(NameMemoryInfo name, Range range)
    {
        Name = name;
        Range = range;
    }
}
