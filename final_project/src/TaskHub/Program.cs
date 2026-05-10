using System.Diagnostics.CodeAnalysis;
using TaskHub.Common;
using TaskHub.ConsoleUi;
using TaskHub.Models;
using TaskHub.Services;
using TaskHub.Storage;

namespace TaskHub;

[ExcludeFromCodeCoverage]
internal static class Program
{
    private static async Task Main()
    {
        var storagePath = Path.Combine(AppContext.BaseDirectory, AppConstants.StorageFileName);

        using var storage = new JsonFileStorage<TaskItem>(storagePath);
        var taskService = new TaskService();
        var statisticsService = new StatisticsService(taskService);

        try
        {
            var tasks = await storage.LoadAsync();
            taskService.ReplaceAll(tasks);
            Console.WriteLine($"Loaded tasks: {tasks.Count}");
        }
        catch (Exception exception) when (exception is IOException or InvalidOperationException)
        {
            Console.WriteLine($"Could not load saved tasks: {exception.Message}");
        }

        await using var deadlineMonitor = new DeadlineMonitor(
            taskService,
            TimeSpan.FromSeconds(AppConstants.DeadlineCheckIntervalSeconds));

        var menu = new ConsoleMenu(taskService, storage, statisticsService, deadlineMonitor);
        await menu.RunAsync();
    }
}
