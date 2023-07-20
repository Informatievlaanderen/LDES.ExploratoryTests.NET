using AbbLdesReader.LdesModels;

namespace AbbLdesReader;

public interface IEventProcessor
{
    public bool CanProcess(LdesObject ldesObject);

    public Task ProcessAsync(LdesObject ldesObject);
}
