using TaskHub.Common;
using TaskHub.Models;

namespace TaskHub.Services;

public sealed class TaskService
{
    private readonly List<TaskItem> _tasks = [];
    private readonly object _syncRoot = new();

    public event TaskChangedHandler? TaskChanged;

    public TaskItem CreateTask(
        string title,
        string description,
        TaskPriority priority,
        DateTime deadline,
        TaskItemStatus status)
    {
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = NormalizeRequired(title, "Title"),
            Description = description.Trim(),
            Priority = priority,
            Deadline = deadline,
            Status = status
        };

        lock (_syncRoot)
        {
            _tasks.Add(task);
        }

        var createdTask = Clone(task);
        TaskChanged?.Invoke(createdTask);
        return createdTask;
    }

    public IReadOnlyList<TaskItem> GetAll()
    {
        lock (_syncRoot)
        {
            return _tasks.Select(Clone).ToList();
        }
    }

    public TaskItem? GetById(Guid id)
    {
        lock (_syncRoot)
        {
            var task = _tasks.FirstOrDefault(task => task.Id == id);
            return task is null ? null : Clone(task);
        }
    }

    public IReadOnlyList<TaskItem> Find(Func<TaskItem, bool> predicate)
    {
        lock (_syncRoot)
        {
            return _tasks.Where(predicate).Select(Clone).ToList();
        }
    }

    public IReadOnlyList<TaskItem> SearchByTitle(string titlePart)
    {
        var value = titlePart.Trim();
        return Find(task => task.Title.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyList<TaskItem> SearchByStatus(TaskItemStatus status)
        => Find(task => task.Status == status);

    public IReadOnlyList<TaskItem> SearchByPriority(TaskPriority priority)
        => Find(task => task.Priority == priority);

    public IReadOnlyList<TaskItem> GetOverdue(DateTime now)
        => Find(task => task.IsOverdue(now));

    public bool UpdateTask(
        Guid id,
        string? title = null,
        string? description = null,
        TaskPriority? priority = null,
        DateTime? deadline = null,
        TaskItemStatus? status = null)
    {
        TaskItem? changedTask = null;

        lock (_syncRoot)
        {
            var task = _tasks.FirstOrDefault(task => task.Id == id);
            if (task is null)
            {
                return false;
            }

            if (title is not null)
            {
                task.Title = NormalizeRequired(title, "Title");
            }

            if (description is not null)
            {
                task.Description = description.Trim();
            }

            if (priority.HasValue)
            {
                task.Priority = priority.Value;
            }

            if (deadline.HasValue)
            {
                task.Deadline = deadline.Value;
            }

            if (status.HasValue)
            {
                task.Status = status.Value;
            }

            changedTask = Clone(task);
        }

        TaskChanged?.Invoke(changedTask);
        return true;
    }

    public bool DeleteTask(Guid id)
    {
        TaskItem? removedTask = null;

        lock (_syncRoot)
        {
            var task = _tasks.FirstOrDefault(task => task.Id == id);
            if (task is null)
            {
                return false;
            }

            removedTask = Clone(task);
            _tasks.Remove(task);
        }

        TaskChanged?.Invoke(removedTask);
        return true;
    }

    public void ReplaceAll(IEnumerable<TaskItem> tasks)
    {
        var normalizedTasks = tasks.Select(NormalizeLoadedTask).ToList();

        lock (_syncRoot)
        {
            _tasks.Clear();
            _tasks.AddRange(normalizedTasks);
        }
    }

    private static TaskItem NormalizeLoadedTask(TaskItem task)
    {
        task.Id = task.Id == Guid.Empty ? Guid.NewGuid() : task.Id;
        task.Title = NormalizeRequired(task.Title, "Title");
        task.Description = task.Description.Trim();
        return task;
    }

    private static string NormalizeRequired(string value, string fieldName)
    {
        var normalized = value.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ValidationException($"{fieldName} cannot be empty.");
        }

        return normalized;
    }

    private static TaskItem Clone(TaskItem task)
        => new()
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority,
            Deadline = task.Deadline,
            Status = task.Status
        };
}
