using AbbLdesReader;
using AbbLdesReader.LdesModels;

namespace LdesReader.Strategies;

public abstract class IsVersionOfEventProcessor : IEventProcessor
{
    protected abstract IEnumerable<string> VersionsToProcess { get; }

    public bool CanProcess(LdesObject ldesObject)
    {
        if (!ldesObject.Properties.TryGetValue("http://purl.org/dc/terms/isVersionOf", out var isVersionOf))
            return false;

        return VersionsToProcess.Any(v=>isVersionOf[0].Id!.StartsWith(v));
    }

    public abstract Task ProcessAsync(LdesObject ldesObject);
}
