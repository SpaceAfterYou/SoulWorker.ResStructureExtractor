namespace SpaceAfterYou.SoulWorker.ResStructureExtractor;

public static class Extract
{
    public static Dictionary<string, Type[]> FromUnpacked(byte[] buffer)
    {
        var extractor = Extractor.Create(buffer);
        return extractor.FromUnpacked();
    }

    public static Dictionary<string, Type[]> FromUnpacked(string path)
    {
        var extractor = Extractor.Create(path);
        return extractor.FromUnpacked();
    }
}
