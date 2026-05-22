using TaskHub.Models;
using TaskHub.Services;

namespace TaskHub.Tests;

public sealed class DeadlineMonitorTests
{
    [Fact]
    public async Task Start_RaisesNotificationForOverdueTaskOnce()
    {
        var taskService = new TaskService();
        var now = new DateTime(2026, 5, 10, 12, 0, 0);
        var task = taskService.CreateTask("Overdue", "", TaskPriority.High, now.AddMinutes(-1), TaskItemStatus.New);
        using var monitor = new DeadlineMonitor(taskService, TimeSpan.FromMilliseconds(10), () => now);
        var notifications = new List<TaskItem>();
        var firstNotification = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        monitor.OverdueTaskFound += overdueTask =>
        {
            notifications.Add(overdueTask);
            firstNotification.TrySetResult();
        };

        monitor.Start();
        await firstNotification.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await Task.Delay(50);

        Assert.Single(notifications);
        Assert.Equal(task.Id, notifications[0].Id);
    }

    [Fact]
    public void Start_ThrowsAfterDispose()
    {
        var taskService = new TaskService();
        var monitor = new DeadlineMonitor(taskService, TimeSpan.FromMilliseconds(10));

        monitor.Dispose();
        monitor.Dispose();

        Assert.Throws<ObjectDisposedException>(monitor.Start);
    }

    [Fact]
    public async Task DisposeAsync_StopsWorkerAndIsIdempotent()
    {
        var taskService = new TaskService();
        var monitor = new DeadlineMonitor(taskService, TimeSpan.FromMilliseconds(10));

        monitor.Start();
        await monitor.DisposeAsync();
        await monitor.DisposeAsync();

        Assert.Throws<ObjectDisposedException>(monitor.Start);
    }
}
