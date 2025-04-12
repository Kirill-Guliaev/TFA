using System.Collections.Concurrent;
using System.Diagnostics.Metrics;

namespace TFA.Domain.Monitoring;

internal class DomainMetrics
{
    private readonly Meter forumsFetched;
    private readonly ConcurrentDictionary<string, Counter<int>> counters = new();

    public DomainMetrics(IMeterFactory meter)
    {
        forumsFetched = meter.Create("TFA.Domain");
    }

    public void ForumsFetched(bool success)
    {
        IncrementCounter("forums.fetched", 1, new Dictionary<string, object?>
        {
            ["success"] = success
        });
    }

    private void IncrementCounter(string name, int value, IDictionary<string, object?>? additionalTags = null)
    {
        var counter = counters.GetOrAdd(name, _ => forumsFetched.CreateCounter<int>(name));
        counter.Add(value, additionalTags?.ToArray() ?? ReadOnlySpan<KeyValuePair<string, object?>>.Empty);
    }
}
