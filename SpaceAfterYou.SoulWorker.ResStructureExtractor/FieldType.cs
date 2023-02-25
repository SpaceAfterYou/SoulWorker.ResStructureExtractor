namespace SpaceAfterYou.SoulWorker.ResStructureExtractor;

internal sealed record FieldType
{
    internal Type Type { get; }
    internal ReadOnlyMemory<byte?> Pattern { get; }

    internal FieldType(Type type, ReadOnlyMemory<byte?> pattern)
    {
        Type = type;
        Pattern = pattern;
    }
}
