using TaskHub.ConsoleUi;
using TaskHub.Models;
using TaskHub.Services;
using TaskHub.Storage;
using System.Reflection;

namespace TaskHub.Tests;

public sealed class ConsoleMenuTests
{
    [Fact]
    public async Task RunAsync_CreatesSearchesShowsStatisticsSavesAndExits()
    {
        var taskService = new TaskService();
        var storage = new FakeStorage<TaskItem>();
        var menu = CreateMenu(taskService, storage);
        var input = string.Join(Environment.NewLine,
            "1",
            "Write docs",
            "Course project",
            "3",
            "2026-05-20 18:30",
            "1",
            "5",
            "1",
            "write",
            "6",
            "7",
            "0",
            string.Empty);

        var output = await RunMenuAsync(menu, input);

        Assert.Contains("Task created: Write docs", output);
        Assert.Contains("Total tasks: 1", output);
        Assert.Equal(2, storage.SaveCount);
        Assert.Single(storage.SavedItems);
        Assert.Equal("Write docs", storage.SavedItems[0].Title);
    }

    [Fact]
    public async Task RunAsync_LoadsViewsEditsAndDeletesTask()
    {
        var loadedTask = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Loaded",
            Description = "from storage",
            Priority = TaskPriority.High,
            Deadline = new DateTime(2026, 5, 20, 18, 30, 0),
            Status = TaskItemStatus.New
        };
        var taskService = new TaskService();
        var storage = new FakeStorage<TaskItem> { ItemsToLoad = [loadedTask] };
        var menu = CreateMenu(taskService, storage);
        var input = string.Join(Environment.NewLine,
            "8",
            "2",
            "4",
            "3",
            "1",
            "Updated",
            string.Empty,
            "2",
            "2026-05-30 10:00",
            "2",
            "4",
            "1",
            "0",
            string.Empty);

        var output = await RunMenuAsync(menu, input);

        Assert.Contains("Loaded tasks: 1", output);
        Assert.Contains("Task updated.", output);
        Assert.Contains("Task deleted.", output);
        Assert.Empty(taskService.GetAll());
    }

    [Fact]
    public async Task RunAsync_ViewsAndSearchesByAllSupportedFilters()
    {
        var taskService = new TaskService();
        taskService.CreateTask("Done low", "", TaskPriority.Low, DateTime.Now.AddDays(1), TaskItemStatus.Done);
        taskService.CreateTask("Active high", "", TaskPriority.High, DateTime.Now.AddDays(1), TaskItemStatus.InProgress);
        var storage = new FakeStorage<TaskItem>();
        var menu = CreateMenu(taskService, storage);
        var input = string.Join(Environment.NewLine,
            "2",
            "1",
            "2",
            "2",
            "2",
            "3",
            "5",
            "2",
            "2",
            "5",
            "3",
            "3",
            "0",
            string.Empty);

        var output = await RunMenuAsync(menu, input);

        Assert.Contains("Done low", output);
        Assert.Contains("Active high", output);
        Assert.Single(storage.SavedItems, task => task.Priority == TaskPriority.High);
    }

    [Fact]
    public async Task RunAsync_ShowsMessageWhenEditOrDeleteHasNoTasks()
    {
        var taskService = new TaskService();
        var storage = new FakeStorage<TaskItem>();
        var menu = CreateMenu(taskService, storage);
        var input = string.Join(Environment.NewLine,
            "3",
            "4",
            "0",
            string.Empty);

        var output = await RunMenuAsync(menu, input);

        Assert.Equal(2, CountOccurrences(output, "No tasks found."));
    }

    [Fact]
    public async Task RunAsync_PrintsNoTasksFoundForEmptyViewResult()
    {
        var taskService = new TaskService();
        taskService.CreateTask("Active", "", TaskPriority.Low, DateTime.Now.AddDays(1), TaskItemStatus.New);
        var storage = new FakeStorage<TaskItem>();
        var menu = CreateMenu(taskService, storage);
        var input = string.Join(Environment.NewLine,
            "2",
            "2",
            "0",
            string.Empty);

        var output = await RunMenuAsync(menu, input);

        Assert.Contains("No tasks found.", output);
    }

    [Fact]
    public async Task RunAsync_PrintsValidationErrors()
    {
        var taskService = new TaskService();
        var storage = new FakeStorage<TaskItem>
        {
            ItemsToLoad =
            [
                new TaskItem
                {
                    Title = "",
                    Description = "",
                    Priority = TaskPriority.Low,
                    Deadline = DateTime.Now.AddDays(1),
                    Status = TaskItemStatus.New
                }
            ]
        };
        var menu = CreateMenu(taskService, storage);
        var input = string.Join(Environment.NewLine,
            "8",
            "0",
            string.Empty);

        var output = await RunMenuAsync(menu, input);

        Assert.Contains("Validation error: Title cannot be empty.", output);
    }

    [Fact]
    public void ShowOverdueNotification_PrintsOverdueTask()
    {
        var taskService = new TaskService();
        var storage = new FakeStorage<TaskItem>();
        var menu = CreateMenu(taskService, storage);
        var task = new TaskItem
        {
            Title = "Late task",
            Deadline = new DateTime(2026, 5, 10, 12, 0, 0),
            Priority = TaskPriority.High,
            Status = TaskItemStatus.New
        };
        var method = typeof(ConsoleMenu).GetMethod(
            "ShowOverdueNotification",
            BindingFlags.Instance | BindingFlags.NonPublic);

        var output = WithConsoleOutput(() => method!.Invoke(menu, [task]));

        Assert.Contains("[Overdue] Late task was due at 2026-05-10 12:00", output);
    }

    [Fact]
    public async Task RunAsync_PrintsOperationErrorsAndFileErrors()
    {
        var taskService = new TaskService();
        var storage = new FakeStorage<TaskItem>
        {
            LoadException = new InvalidOperationException("bad json"),
            SaveException = new IOException("disk full"),
            ClearSaveExceptionAfterThrow = true
        };
        var menu = CreateMenu(taskService, storage);
        var input = string.Join(Environment.NewLine,
            "8",
            "7",
            "0",
            "0",
            string.Empty);

        var output = await RunMenuAsync(menu, input);

        Assert.Contains("Operation error: bad json", output);
        Assert.Contains("File error: disk full", output);
    }

    [Fact]
    public async Task RunAsync_ShowsUnsavedChangesIndicatorAfterCreatingTask()
    {
        var taskService = new TaskService();
        var storage = new FakeStorage<TaskItem>();
        var menu = CreateMenu(taskService, storage);
        var input = string.Join(Environment.NewLine,
            "1",
            "Write docs",
            "Course project",
            "3",
            "2026-05-20 18:30",
            "1",
            "0",
            string.Empty);

        var output = await RunMenuAsync(menu, input);

        Assert.Contains("TaskHub (unsaved changes)", output);
    }

    [Fact]
    public async Task RunAsync_DoesNotShowUnsavedIndicatorWhenNothingChanged()
    {
        var taskService = new TaskService();
        var storage = new FakeStorage<TaskItem>();
        var menu = CreateMenu(taskService, storage);
        var input = string.Join(Environment.NewLine, "0", string.Empty);

        var output = await RunMenuAsync(menu, input);

        Assert.DoesNotContain("(unsaved changes)", output);
    }

    [Fact]
    public async Task RunAsync_ClearsUnsavedIndicatorAfterSave()
    {
        var taskService = new TaskService();
        var storage = new FakeStorage<TaskItem>();
        var menu = CreateMenu(taskService, storage);
        var input = string.Join(Environment.NewLine,
            "1",
            "Write docs",
            "Course project",
            "3",
            "2026-05-20 18:30",
            "1",
            "7",
            "0",
            string.Empty);

        var output = await RunMenuAsync(menu, input);

        // The indicator is shown only for the single iteration between
        // creating the task and saving it, then cleared on save.
        Assert.Equal(1, CountOccurrences(output, "TaskHub (unsaved changes)"));
    }

    private static ConsoleMenu CreateMenu(TaskService taskService, IAsyncStorage<TaskItem> storage)
    {
        var statisticsService = new StatisticsService(taskService);
        var deadlineMonitor = new DeadlineMonitor(taskService, TimeSpan.FromDays(1));
        return new ConsoleMenu(taskService, storage, statisticsService, deadlineMonitor);
    }

    private static async Task<string> RunMenuAsync(ConsoleMenu menu, string input)
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;
        using var reader = new StringReader(input);
        using var writer = new StringWriter();

        try
        {
            Console.SetIn(reader);
            Console.SetOut(writer);
            await menu.RunAsync();
            return writer.ToString();
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    private static string WithConsoleOutput(Action action)
    {
        var originalOut = Console.Out;
        using var writer = new StringWriter();

        try
        {
            Console.SetOut(writer);
            action();
            return writer.ToString();
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    private static int CountOccurrences(string value, string pattern)
    {
        var count = 0;
        var index = 0;

        while ((index = value.IndexOf(pattern, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += pattern.Length;
        }

        return count;
    }

    private sealed class FakeStorage<T> : IAsyncStorage<T>
    {
        public IReadOnlyList<T> ItemsToLoad { get; init; } = [];

        public List<T> SavedItems { get; } = [];

        public int SaveCount { get; private set; }

        public Exception? LoadException { get; init; }

        public Exception? SaveException { get; set; }

        public bool ClearSaveExceptionAfterThrow { get; init; }

        public Task<IReadOnlyList<T>> LoadAsync(CancellationToken cancellationToken = default)
        {
            if (LoadException is not null)
            {
                throw LoadException;
            }

            return Task.FromResult(ItemsToLoad);
        }

        public Task SaveAsync(IReadOnlyList<T> items, CancellationToken cancellationToken = default)
        {
            SaveCount++;

            if (SaveException is not null)
            {
                var exception = SaveException;
                if (ClearSaveExceptionAfterThrow)
                {
                    SaveException = null;
                }

                throw exception;
            }

            SavedItems.Clear();
            SavedItems.AddRange(items);
            return Task.CompletedTask;
        }
    }
}
