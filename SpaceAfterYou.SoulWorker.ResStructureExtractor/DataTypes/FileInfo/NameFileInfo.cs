namespace SpaceAfterYou.SoulWorker.ResStructureExtractor.DataTypes.FileInfo;

internal readonly struct NameFileInfo
{
    internal string Name { get; }
    internal int Offset { get; }

    internal NameFileInfo(string name, int offset)
    {
        Name = name;
        Offset = offset;
    }
}

// https://youtu.be/BEcv39gKAkw
// https://youtu.be/0BdcUEEvi1M