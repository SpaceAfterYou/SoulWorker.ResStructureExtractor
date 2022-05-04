namespace SoulWorker.ResStructureExtractor;

public static class Extract
{
    public static async ValueTask<Dictionary<string, string[]>> FromUnpacked(string path)
    {
        var extractor = await Extractor.Create(path);
        return await extractor.FromUnpacked();
    }
}
