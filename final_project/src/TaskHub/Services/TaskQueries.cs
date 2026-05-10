using TaskHub.Models;

namespace TaskHub.Services;

public static class TaskQueries
{
    public static bool IsDone(TaskItem task) => task.Status == TaskItemStatus.Done;

    public static bool IsIncomplete(TaskItem task) => task.Status != TaskItemStatus.Done;

    public static bool IsHighPriority(TaskItem task) => task.Priority == TaskPriority.High;
}
