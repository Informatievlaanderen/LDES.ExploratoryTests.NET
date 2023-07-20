using AbbLdesReader;
using AbbLdesReader.LdesModels;

namespace LdesReader.Strategies;

public class EventEventProcessor:IEventProcessor
{
    private readonly OrganisationCache _cache;

    public EventEventProcessor(OrganisationCache cache)
    {
        _cache = cache;
    }

    public bool CanProcess(LdesObject ldesObject) => ldesObject.Id is not null;

    public Task ProcessAsync(LdesObject ldesObject)
    {
        _cache.LastEventId = ldesObject.Id!;
        return Task.CompletedTask;
    }
}
