using SoulWorker.ResStructureExtractor.DataTypes.MemoryInfo;
using System.Diagnostics;

namespace SoulWorker.ResStructureExtractor.DataTypes.FileInfo;

internal readonly struct NameUsageFileInfo
{
    #region Fields

    internal NameMemoryInfo Name { get; }
    internal int Offset { get; }
    internal bool IsValidOffset => Offset != 0;

    #endregion Fields

    #region Constructors

    internal NameUsageFileInfo(NameMemoryInfo name, int offset)
    {
        Name = name;
        Offset = offset;

        Debug.WriteLineIf(!IsValidOffset, $"{Name.Value} not found");
    }

    #endregion Constructors
}
