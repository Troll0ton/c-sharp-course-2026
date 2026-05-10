using TaskHub.Common;
using TaskHub.Models;
using TaskHub.Services;
using TaskHub.Storage;

namespace TaskHub.ConsoleUi;

public sealed class ConsoleMenu
{
    private readonly TaskService _taskService;
    private readonly IAsyncStorage<TaskItem> _storage;
    private readonly StatisticsService _statisticsService;
    private readonly DeadlineMonitor _deadlineMonitor;
    private readonly object _consoleSync = new();
    private bool _hasUnsavedChanges;

    public ConsoleMenu(
        TaskService taskService,
        IAsyncStorage<TaskItem> storage,
        StatisticsService statisticsService,
        DeadlineMonitor deadlineMonitor)
    {
        _taskService = taskService;
        _storage = storage;
        _statisticsService = statisticsService;
        _deadlineMonitor = deadlineMonitor;
    }

    public async Task RunAsync()
    {
        _deadlineMonitor.OverdueTaskFound += ShowOverdueNotification;
        _taskService.TaskChanged += OnTaskChanged;
        _deadlineMonitor.Start();

        try
        {
            await RunMenuLoopAsync();
        }
        finally
        {
            _taskService.TaskChanged -= OnTaskChanged;
            _deadlineMonitor.OverdueTaskFound -= ShowOverdueNotification;
        }
    }

    private async Task RunMenuLoopAsync()
    {
        var exitRequested = false;
        while (!exitRequested)
        {
            PrintMainMenu();

            try
            {
                var choice = ConsoleInput.ReadMenuChoice("Choose: ", 0, 8);
                Console.WriteLine();

                switch (choice)
                {
                    case 1:
                        CreateTask();
                        break;
                    case 2:
                        ShowTasks();
                        break;
                    case 3:
                        EditTask();
                        break;
                    case 4:
                        DeleteTask();
                        break;
                    case 5:
                        SearchTasks();
                        break;
                    case 6:
                        ShowStatistics();
                        break;
                    case 7:
                        await SaveTasksAsync();
                        break;
                    case 8:
                        await LoadTasksAsync();
                        break;
                    case 0:
                        await SaveTasksAsync();
                        exitRequested = true;
                        break;
                }
            }
            catch (ValidationException exception)
            {
                Console.WriteLine($"Validation error: {exception.Message}");
            }
            catch (InvalidOperationException exception)
            {
                Console.WriteLine($"Operation error: {exception.Message}");
            }
            catch (IOException exception)
            {
                Console.WriteLine($"File error: {exception.Message}");
            }

            Console.WriteLine();
        }
    }

    private void OnTaskChanged(TaskItem task) => _hasUnsavedChanges = true;

    private void PrintMainMenu()
    {
        Console.WriteLine(_hasUnsavedChanges ? "TaskHub (unsaved changes)" : "TaskHub");
        Console.WriteLine("1. Create task");
        Console.WriteLine("2. View tasks");
        Console.WriteLine("3. Edit task");
        Console.WriteLine("4. Delete task");
        Console.WriteLine("5. Search tasks");
        Console.WriteLine("6. Statistics");
        Console.WriteLine("7. Save tasks");
        Console.WriteLine("8. Load tasks");
        Console.WriteLine("0. Save and exit");
    }

    private void CreateTask()
    {
        var title = ConsoleInput.ReadRequiredString("Title: ");
        Console.Write("Description: ");
        var description = Console.ReadLine() ?? string.Empty;
        var priority = ConsoleInput.ReadEnum<TaskPriority>("Priority:");
        var deadline = ConsoleInput.ReadDateTime("Deadline");
        var status = ConsoleInput.ReadEnum<TaskItemStatus>("Status:");

        var task = _taskService.CreateTask(title, description, priority, deadline, status);
        Console.WriteLine($"Task created: {task.Title}");
    }

    private void ShowTasks()
    {
        Console.WriteLine("1. All tasks");
        Console.WriteLine("2. Done tasks");
        Console.WriteLine("3. Incomplete tasks");
        Console.WriteLine("4. High priority tasks");

        var choice = ConsoleInput.ReadMenuChoice("Choose: ", 1, 4);
        IReadOnlyList<TaskItem> tasks = [];

        switch (choice)
        {
            case 1:
                tasks = _taskService.GetAll();
                break;
            case 2:
                tasks = _taskService.Find(TaskQueries.IsDone);
                break;
            case 3:
                tasks = _taskService.Find(TaskQueries.IsIncomplete);
                break;
            case 4:
                tasks = _taskService.Find(TaskQueries.IsHighPriority);
                break;
        }

        PrintTasks(tasks);
    }

    private void EditTask()
    {
        var task = SelectTask();
        if (task is null)
        {
            return;
        }

        var title = ConsoleInput.ReadOptionalString("Title", task.Title);
        var description = ConsoleInput.ReadOptionalString("Description", task.Description);
        var priority = ConsoleInput.ReadOptionalEnum("Priority", task.Priority);
        var deadline = ConsoleInput.ReadOptionalDateTime("Deadline", task.Deadline);
        var status = ConsoleInput.ReadOptionalEnum("Status", task.Status);

        if (_taskService.UpdateTask(task.Id, title, description, priority, deadline, status))
        {
            Console.WriteLine("Task updated.");
        }
    }

    private void DeleteTask()
    {
        var task = SelectTask();
        if (task is null)
        {
            return;
        }

        if (_taskService.DeleteTask(task.Id))
        {
            Console.WriteLine("Task deleted.");
        }
    }

    private void SearchTasks()
    {
        Console.WriteLine("1. Search by title");
        Console.WriteLine("2. Search by status");
        Console.WriteLine("3. Search by priority");

        var choice = ConsoleInput.ReadMenuChoice("Choose: ", 1, 3);
        IReadOnlyList<TaskItem> tasks = [];

        switch (choice)
        {
            case 1:
                tasks = _taskService.SearchByTitle(ConsoleInput.ReadRequiredString("Title contains: "));
                break;
            case 2:
                tasks = _taskService.SearchByStatus(ConsoleInput.ReadEnum<TaskItemStatus>("Status:"));
                break;
            case 3:
                tasks = _taskService.SearchByPriority(ConsoleInput.ReadEnum<TaskPriority>("Priority:"));
                break;
        }

        PrintTasks(tasks);
    }

    private void ShowStatistics()
    {
        var statistics = _statisticsService.GetStatistics(DateTime.Now);

        Console.WriteLine($"Total tasks: {statistics.TotalCount}");
        Console.WriteLine($"Done tasks: {statistics.DoneCount}");
        Console.WriteLine($"Overdue tasks: {statistics.OverdueCount}");
        Console.WriteLine("Priority statistics:");

        foreach (var (priority, count) in statistics.PriorityCounts)
        {
            Console.WriteLine($"- {priority}: {count}");
        }
    }

    private async Task SaveTasksAsync()
    {
        await _storage.SaveAsync(_taskService.GetAll());
        _hasUnsavedChanges = false;
        Console.WriteLine($"Tasks saved to {AppConstants.StorageFileName}.");
    }

    private async Task LoadTasksAsync()
    {
        var tasks = await _storage.LoadAsync();
        _taskService.ReplaceAll(tasks);
        _hasUnsavedChanges = false;
        Console.WriteLine($"Loaded tasks: {tasks.Count}");
    }

    private TaskItem? SelectTask()
    {
        var tasks = _taskService.GetAll();
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks found.");
            return null;
        }

        for (var index = 0; index < tasks.Count; index++)
        {
            Console.Write($"{index + 1}. ");
            PrintTask(tasks[index]);
        }

        var choice = ConsoleInput.ReadMenuChoice("Choose task: ", 1, tasks.Count);
        return tasks[choice - 1];
    }

    private static void PrintTasks(IReadOnlyList<TaskItem> tasks)
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks found.");
            return;
        }

        foreach (var task in tasks)
        {
            PrintTask(task);
        }
    }

    private static void PrintTask(TaskItem task)
    {
        Console.WriteLine(
            $"[{task.Id.ToString()[..8]}] {task.Title} | {task.Priority} | {task.Status} | deadline: {task.Deadline:yyyy-MM-dd HH:mm}");

        if (!string.IsNullOrWhiteSpace(task.Description))
        {
            Console.WriteLine($"    {task.Description}");
        }
    }

    private void ShowOverdueNotification(TaskItem task)
    {
        lock (_consoleSync)
        {
            Console.WriteLine();
            Console.WriteLine($"[Overdue] {task.Title} was due at {task.Deadline:yyyy-MM-dd HH:mm}");
            Console.Write("Choose: ");
        }
    }
}
