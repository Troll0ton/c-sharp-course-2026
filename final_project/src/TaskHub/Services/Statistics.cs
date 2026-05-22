using TaskHub.Models;

namespace TaskHub.Services;

public sealed class Statistics
{
    public int TotalCount { get; init; }

    public int DoneCount { get; init; }

    public int OverdueCount { get; init; }

    public Dictionary<TaskPriority, int> PriorityCounts { get; init; } = [];
}
