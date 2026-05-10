using TaskHub.Models;

namespace TaskHub.Services;

public sealed class DeadlineMonitor : IDisposable, IAsyncDisposable
{
    private readonly TaskService _taskService;
    private readonly TimeSpan _checkInterval;
    private readonly Func<DateTime> _clock;
    private readonly CancellationTokenSource _cancellation = new();
    private readonly HashSet<Guid> _notifiedTaskIds = [];
    private Task? _worker;
    private bool _disposed;

    public DeadlineMonitor(TaskService taskService, TimeSpan checkInterval, Func<DateTime>? clock = null)
    {
        _taskService = taskService;
        _checkInterval = checkInterval;
        _clock = clock ?? (() => DateTime.Now);
    }

    public event Action<TaskItem>? OverdueTaskFound;

    public void Start()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _worker ??= Task.Run(() => MonitorAsync(_cancellation.Token));
    }

    private async Task MonitorAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(_checkInterval);

        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            var overdueTasks = _taskService.GetOverdue(_clock());
            var currentOverdueIds = overdueTasks.Select(task => task.Id).ToHashSet();

            foreach (var task in overdueTasks)
            {
                if (_notifiedTaskIds.Add(task.Id))
                {
                    OverdueTaskFound?.Invoke(task);
                }
            }

            _notifiedTaskIds.RemoveWhere(taskId => !currentOverdueIds.Contains(taskId));
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        await _cancellation.CancelAsync();

        if (_worker is not null)
        {
            try
            {
                await _worker;
            }
            catch (OperationCanceledException)
            {
            }
        }

        _cancellation.Dispose();
        _disposed = true;
    }

    // Synchronous Dispose is retained for callers that cannot await
    // (e.g. a 'using' block). It blocks briefly on the worker; prefer
    // DisposeAsync where an async context is available.
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _cancellation.Cancel();

        try
        {
            _worker?.Wait(TimeSpan.FromSeconds(2));
        }
        catch (AggregateException)
        {
        }
        catch (OperationCanceledException)
        {
        }

        _cancellation.Dispose();
        _disposed = true;
    }
}
