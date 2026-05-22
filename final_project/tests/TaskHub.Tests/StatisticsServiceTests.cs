using TaskHub.Models;
using TaskHub.Services;

namespace TaskHub.Tests;

public sealed class StatisticsServiceTests
{
    [Fact]
    public void GetStatistics_ReturnsTotalsAndPriorityCounts()
    {
        var taskService = new TaskService();
        var now = new DateTime(2026, 5, 10, 12, 0, 0);
        taskService.CreateTask("Done", "", TaskPriority.Low, now.AddDays(-1), TaskItemStatus.Done);
        taskService.CreateTask("Overdue", "", TaskPriority.High, now.AddMinutes(-1), TaskItemStatus.New);
        taskService.CreateTask("Future", "", TaskPriority.High, now.AddDays(1), TaskItemStatus.InProgress);
        var statisticsService = new StatisticsService(taskService);

        var statistics = statisticsService.GetStatistics(now);

        Assert.Equal(3, statistics.TotalCount);
        Assert.Equal(1, statistics.DoneCount);
        Assert.Equal(1, statistics.OverdueCount);
        Assert.Equal(1, statistics.PriorityCounts[TaskPriority.Low]);
        Assert.Equal(0, statistics.PriorityCounts[TaskPriority.Medium]);
        Assert.Equal(2, statistics.PriorityCounts[TaskPriority.High]);
    }
}
