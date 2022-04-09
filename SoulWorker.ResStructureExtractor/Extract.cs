namespace SoulWorker.ResStructureExtractor;

public static class Extract
{
    public static ValueTask<Dictionary<string, string[]>> FromUnpacked(string path) => new Extractor(path)
        .FromUnpacked();
        
}