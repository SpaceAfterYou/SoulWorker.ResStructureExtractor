namespace SpaceAfterYou.SoulWorker.ResStructureExtractor;

public static class Extract
{
    public static Dictionary<string, Type[]> FromUnpacked(string path)
    {
        var extractor = Extractor.Create(path);
        return extractor.FromUnpacked();
    }
}
