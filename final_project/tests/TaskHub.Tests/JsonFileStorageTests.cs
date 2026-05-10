using TaskHub.Models;
using TaskHub.Storage;

namespace TaskHub.Tests;

public sealed class JsonFileStorageTests
{
    [Fact]
    public async Task SaveAndLoadAsync_PreservesTasks()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"taskhub-{Guid.NewGuid():N}.json");
        using var storage = new JsonFileStorage<TaskItem>(filePath);
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Persisted",
            Description = "Stored in JSON",
            Priority = TaskPriority.Medium,
            Deadline = new DateTime(2026, 6, 1, 9, 0, 0),
            Status = TaskItemStatus.InProgress
        };

        try
        {
            await storage.SaveAsync([task]);

            var loadedTasks = await storage.LoadAsync();

            Assert.Single(loadedTasks);
            Assert.Equal(task.Id, loadedTasks[0].Id);
            Assert.Equal("Persisted", loadedTasks[0].Title);
            Assert.Equal(TaskPriority.Medium, loadedTasks[0].Priority);
            Assert.Equal(TaskItemStatus.InProgress, loadedTasks[0].Status);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public async Task LoadAsync_ReturnsEmptyListWhenFileDoesNotExist()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"missing-taskhub-{Guid.NewGuid():N}.json");
        using var storage = new JsonFileStorage<TaskItem>(filePath);

        var tasks = await storage.LoadAsync();

        Assert.Empty(tasks);
    }

    [Fact]
    public async Task LoadAsync_ThrowsInvalidOperationExceptionForInvalidJson()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"invalid-taskhub-{Guid.NewGuid():N}.json");
        await File.WriteAllTextAsync(filePath, "{ invalid json");
        using var storage = new JsonFileStorage<TaskItem>(filePath);

        try
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => storage.LoadAsync());

            Assert.Contains("invalid JSON", exception.Message);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public async Task SaveAsync_CreatesMissingDirectory()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"taskhub-dir-{Guid.NewGuid():N}");
        var filePath = Path.Combine(directory, "tasks.json");
        using var storage = new JsonFileStorage<TaskItem>(filePath);

        try
        {
            await storage.SaveAsync([]);

            Assert.True(File.Exists(filePath));
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }

    [Fact]
    public async Task LoadAndSave_ThrowAfterDispose()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"disposed-taskhub-{Guid.NewGuid():N}.json");
        var storage = new JsonFileStorage<TaskItem>(filePath);

        storage.Dispose();

        await Assert.ThrowsAsync<ObjectDisposedException>(() => storage.LoadAsync());
        await Assert.ThrowsAsync<ObjectDisposedException>(() => storage.SaveAsync([]));
    }
}
