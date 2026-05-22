using TaskHub.Models;

namespace TaskHub.Services;

public sealed class StatisticsService
{
    private readonly TaskService _taskService;

    public StatisticsService(TaskService taskService)
    {
        _taskService = taskService;
    }

    public Statistics GetStatistics(DateTime now)
    {
        var tasks = _taskService.GetAll();
        var priorityCounts = Enum.GetValues<TaskPriority>()
            .ToDictionary(priority => priority, priority => tasks.Count(task => task.Priority == priority));

        return new Statistics
        {
            TotalCount = tasks.Count,
            DoneCount = tasks.Count(TaskQueries.IsDone),
            OverdueCount = tasks.Count(task => task.IsOverdue(now)),
            PriorityCounts = priorityCounts
        };
    }
}
