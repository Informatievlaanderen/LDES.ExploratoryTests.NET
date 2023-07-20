using AbbLdesReader.LdesModels;

namespace AbbLdesReader;

public class LdesProcessor
{
    private IEventProcessor[] _processors;
    private Func<IEnumerable<LdesObject>,string?> _nextPageOrNull;

    public LdesProcessor()
    {
        _processors = Array.Empty<IEventProcessor>();
    }

    public LdesProcessor NextPageBy(Func<IEnumerable<LdesObject>, string?> nextPageOrNull)
    {
        _nextPageOrNull = nextPageOrNull;
        return this;
    }

    public LdesProcessor WithEventProcessor(IEventProcessor eventProcessor)
    {
        _processors = _processors.Append(eventProcessor).ToArray();
        return this;
    }

    public async Task Start(string? startPage = null, string? startAtId = null)
    {
        do
        {
            if (startPage is null) return;

            Console.WriteLine($"Getting page: {startPage}");

            var pageUri = new Uri(startPage);
            var ldesPage = (await LdesReader.ReadPage(pageUri)).ToList();

            await ProcessPage(ldesPage, startAtId);
            startPage = _nextPageOrNull(ldesPage);

        } while (startPage is not null);
    }

    private async Task ProcessPage(IEnumerable<LdesObject> ldesPage, string? startAtId = null)
    {
        if (startAtId is not null)
        {
            ldesPage = ldesPage
                .SkipWhile(o => o.Id != startAtId)
                .Skip(1)
                .ToArray();
        }

        foreach (var ldesObject in ldesPage)
        {
            foreach (var processor in _processors.Where(p => p.CanProcess(ldesObject)))
            {
                await processor.ProcessAsync(ldesObject);
            }
        }
    }
}
