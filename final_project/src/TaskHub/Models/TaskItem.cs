namespace TaskHub.Models;

public sealed class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public TaskPriority Priority { get; set; }

    public DateTime Deadline { get; set; }

    public TaskItemStatus Status { get; set; } = TaskItemStatus.New;

    public bool IsOverdue(DateTime now) => Status != TaskItemStatus.Done && Deadline < now;
}
