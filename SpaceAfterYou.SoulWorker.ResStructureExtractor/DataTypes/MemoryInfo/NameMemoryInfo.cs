namespace SpaceAfterYou.SoulWorker.ResStructureExtractor.DataTypes.MemoryInfo;

internal readonly struct NameMemoryInfo
{
    internal string Value { get; }
    internal int Address { get; }

    internal NameMemoryInfo(string name, int address)
    {
        Value = name;
        Address = address;
    }
}
