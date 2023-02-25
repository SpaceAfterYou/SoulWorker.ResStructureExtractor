namespace SpaceAfterYou.SoulWorker.ResStructureExtractor;

public static class Extract
{
    public static async Task<Dictionary<string, Type[]>> FromUnpacked(string path)
    {
        var extractor = await Extractor.Create(path);
        return extractor.FromUnpacked();
    }
}
