using TaskHub.Common;
using TaskHub.Models;
using TaskHub.Services;

namespace TaskHub.Tests;

public sealed class TaskServiceTests
{
    [Fact]
    public void CreateTask_AddsTaskWithNormalizedTitle()
    {
        var service = new TaskService();

        var task = service.CreateTask(
            "  Write report  ",
            "course project",
            TaskPriority.High,
            new DateTime(2026, 5, 20, 18, 30, 0),
            TaskItemStatus.New);

        Assert.Equal("Write report", task.Title);
        Assert.Single(service.GetAll());
    }

    [Fact]
    public void CreateTask_ThrowsWhenTitleIsEmpty()
    {
        var service = new TaskService();

        Assert.Throws<ValidationException>(() => service.CreateTask(
            string.Empty,
            string.Empty,
            TaskPriority.Low,
            DateTime.Now,
            TaskItemStatus.New));
    }

    [Fact]
    public void SearchByPriority_ReturnsOnlyMatchingTasks()
    {
        var service = new TaskService();
        service.CreateTask("Low", "", TaskPriority.Low, DateTime.Now.AddDays(1), TaskItemStatus.New);
        service.CreateTask("High", "", TaskPriority.High, DateTime.Now.AddDays(1), TaskItemStatus.New);

        var result = service.SearchByPriority(TaskPriority.High);

        Assert.Single(result);
        Assert.Equal("High", result[0].Title);
    }

    [Fact]
    public void SearchByTitleAndStatus_ReturnMatchingTasks()
    {
        var service = new TaskService();
        service.CreateTask("Prepare slides", "", TaskPriority.Low, DateTime.Now.AddDays(1), TaskItemStatus.New);
        service.CreateTask("Review code", "", TaskPriority.High, DateTime.Now.AddDays(1), TaskItemStatus.Done);

        var titleResults = service.SearchByTitle("SLIDES");
        var statusResults = service.SearchByStatus(TaskItemStatus.Done);

        Assert.Single(titleResults);
        Assert.Equal("Prepare slides", titleResults[0].Title);
        Assert.Single(statusResults);
        Assert.Equal("Review code", statusResults[0].Title);
    }

    [Fact]
    public void UpdateTask_ChangesEditableFields()
    {
        var service = new TaskService();
        var task = service.CreateTask("Initial", "", TaskPriority.Low, DateTime.Now.AddDays(1), TaskItemStatus.New);

        var updated = service.UpdateTask(
            task.Id,
            title: "Updated",
            description: "new description",
            priority: TaskPriority.Medium,
            deadline: DateTime.Now.AddDays(2),
            status: TaskItemStatus.InProgress);

        var stored = service.GetById(task.Id);

        Assert.True(updated);
        Assert.NotNull(stored);
        Assert.Equal("Updated", stored.Title);
        Assert.Equal("new description", stored.Description);
        Assert.Equal(TaskPriority.Medium, stored.Priority);
        Assert.Equal(TaskItemStatus.InProgress, stored.Status);
    }

    [Fact]
    public void DeleteTask_RemovesTask()
    {
        var service = new TaskService();
        var task = service.CreateTask("Delete me", "", TaskPriority.Low, DateTime.Now.AddDays(1), TaskItemStatus.New);

        var deleted = service.DeleteTask(task.Id);

        Assert.True(deleted);
        Assert.Empty(service.GetAll());
    }

    [Fact]
    public void UpdateAndDelete_ReturnFalseForMissingTask()
    {
        var service = new TaskService();

        Assert.False(service.UpdateTask(Guid.NewGuid(), title: "Missing"));
        Assert.False(service.DeleteTask(Guid.NewGuid()));
    }

    [Fact]
    public void ReplaceAll_NormalizesLoadedTasks()
    {
        var service = new TaskService();
        var loadedTask = new TaskItem
        {
            Id = Guid.Empty,
            Title = "  Loaded  ",
            Description = "  stored  ",
            Priority = TaskPriority.Medium,
            Deadline = DateTime.Now.AddDays(1),
            Status = TaskItemStatus.New
        };

        service.ReplaceAll([loadedTask]);
        var storedTask = service.GetAll()[0];

        Assert.NotEqual(Guid.Empty, storedTask.Id);
        Assert.Equal("Loaded", storedTask.Title);
        Assert.Equal("stored", storedTask.Description);
    }

    [Fact]
    public void ReturnedTasks_AreCopies()
    {
        var service = new TaskService();
        var task = service.CreateTask("Original", "", TaskPriority.Low, DateTime.Now.AddDays(1), TaskItemStatus.New);

        task.Title = "Changed outside";
        var stored = service.GetById(task.Id);
        var all = service.GetAll();
        all[0].Title = "Changed copy";

        Assert.NotNull(stored);
        Assert.Equal("Original", stored.Title);
        Assert.Equal("Original", service.GetById(task.Id)!.Title);
    }
}
