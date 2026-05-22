using TaskHub.ConsoleUi;
using TaskHub.Models;

namespace TaskHub.Tests;

public sealed class ConsoleInputTests
{
    [Fact]
    public void ReadRequiredString_RepeatsUntilValueIsProvided()
    {
        var (result, output) = WithConsoleInput(
            $"{Environment.NewLine}  task title  {Environment.NewLine}",
            () => ConsoleInput.ReadRequiredString("Title: "));

        Assert.Equal("task title", result);
        Assert.Contains("Value cannot be empty.", output);
    }

    [Fact]
    public void ReadOptionalString_ReturnsNullForEmptyInput()
    {
        var (result, _) = WithConsoleInput(
            Environment.NewLine,
            () => ConsoleInput.ReadOptionalString("Title", "Current"));

        Assert.Null(result);
    }

    [Fact]
    public void ReadEnum_AcceptsEnumNameAfterInvalidInput()
    {
        var (result, output) = WithConsoleInput(
            $"wrong{Environment.NewLine}High{Environment.NewLine}",
            () => ConsoleInput.ReadEnum<TaskPriority>("Priority:"));

        Assert.Equal(TaskPriority.High, result);
        Assert.Contains("Invalid choice.", output);
    }

    [Fact]
    public void ReadOptionalEnum_ReturnsNullOrParsedValue()
    {
        var (emptyResult, _) = WithConsoleInput(
            Environment.NewLine,
            () => ConsoleInput.ReadOptionalEnum("Priority", TaskPriority.Low));
        var (parsedResult, output) = WithConsoleInput(
            $"bad{Environment.NewLine}2{Environment.NewLine}",
            () => ConsoleInput.ReadOptionalEnum("Priority", TaskPriority.Low));
        var (nameResult, _) = WithConsoleInput(
            $"High{Environment.NewLine}",
            () => ConsoleInput.ReadOptionalEnum("Priority", TaskPriority.Low));

        Assert.Null(emptyResult);
        Assert.Equal(TaskPriority.Medium, parsedResult);
        Assert.Equal(TaskPriority.High, nameResult);
        Assert.Contains("Invalid choice.", output);
    }

    [Fact]
    public void ReadDateTime_RepeatsUntilValidDateIsProvided()
    {
        var (result, output) = WithConsoleInput(
            $"not-a-date{Environment.NewLine}2026-05-20 18:30{Environment.NewLine}",
            () => ConsoleInput.ReadDateTime("Deadline"));

        Assert.Equal(new DateTime(2026, 5, 20, 18, 30, 0), result);
        Assert.Contains("Invalid date and time.", output);
    }

    [Fact]
    public void ReadOptionalDateTime_ReturnsNullOrParsedValue()
    {
        var current = new DateTime(2026, 5, 10, 12, 0, 0);
        var (emptyResult, _) = WithConsoleInput(
            Environment.NewLine,
            () => ConsoleInput.ReadOptionalDateTime("Deadline", current));
        var (parsedResult, output) = WithConsoleInput(
            $"wrong{Environment.NewLine}2026-05-20 18:30{Environment.NewLine}",
            () => ConsoleInput.ReadOptionalDateTime("Deadline", current));

        Assert.Null(emptyResult);
        Assert.Equal(new DateTime(2026, 5, 20, 18, 30, 0), parsedResult);
        Assert.Contains("Invalid date and time.", output);
    }

    [Fact]
    public void ReadMenuChoice_RepeatsUntilValueIsInRange()
    {
        var (result, output) = WithConsoleInput(
            $"x{Environment.NewLine}9{Environment.NewLine}2{Environment.NewLine}",
            () => ConsoleInput.ReadMenuChoice("Choose: ", 1, 3));

        Assert.Equal(2, result);
        Assert.Contains("Invalid menu item.", output);
    }

    private static (T Result, string Output) WithConsoleInput<T>(string input, Func<T> action)
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;
        using var reader = new StringReader(input);
        using var writer = new StringWriter();

        try
        {
            Console.SetIn(reader);
            Console.SetOut(writer);
            return (action(), writer.ToString());
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }
}
