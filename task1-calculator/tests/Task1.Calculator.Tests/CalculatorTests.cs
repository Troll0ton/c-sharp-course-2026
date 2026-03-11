using Task1.Calculator;
using Xunit;

namespace Task1.Calculator.Tests;

public class CalculatorTests
{
    [Theory]
    [InlineData(2, 3, '+', 5)]
    [InlineData(5, 3, '-', 2)]
    [InlineData(4, 3, '*', 12)]
    [InlineData(10, 2, '/', 5)]
    public void TryCalculate_ReturnsExpectedResult(double left, double right, char operation, double expected)
    {
        var success = Calculator.TryCalculate(left, right, operation, out var result, out var error);

        Assert.True(success);
        Assert.Null(error);
        Assert.Equal(expected, result, precision: 10);
    }

    [Fact]
    public void TryCalculate_DivisionByZero_ReturnsError()
    {
        var success = Calculator.TryCalculate(10, 0, '/', out var result, out var error);

        Assert.False(success);
        Assert.Equal(0, result);
        Assert.NotNull(error);
    }

    [Fact]
    public void TryCalculate_UnknownOperation_ReturnsError()
    {
        var success = Calculator.TryCalculate(10, 2, '%', out var result, out var error);

        Assert.False(success);
        Assert.Equal(0, result);
        Assert.NotNull(error);
    }

    [Theory]
    [InlineData("q")]
    [InlineData("Q")]
    [InlineData("  q  ")]
    public void IsExitCommand_ReturnsTrue_ForExitInput(string input)
    {
        var isExit = Calculator.IsExitCommand(input);

        Assert.True(isExit);
    }

    [Theory]
    [InlineData("10.5")]
    [InlineData("10,5")]
    [InlineData("  123  ")]
    public void TryParseNumber_ParsesValidNumbers(string input)
    {
        var success = Calculator.TryParseNumber(input, out _);

        Assert.True(success);
    }
}
