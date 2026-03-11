using System.Globalization;

namespace Task1.Calculator;

public static class Calculator
{
    public static bool TryCalculate(double left, double right, char operation, out double result, out string? error)
    {
        result = 0;
        error = null;

        switch (operation)
        {
            case '+':
                result = left + right;
                return true;
            case '-':
                result = left - right;
                return true;
            case '*':
                result = left * right;
                return true;
            case '/':
                if (right == 0)
                {
                    error = "Деление на ноль невозможно.";
                    return false;
                }

                result = left / right;
                return true;
            default:
                error = "Неизвестная операция. Доступно: +, -, *, /.";
                return false;
        }
    }

    public static bool IsExitCommand(string? input)
    {
        return string.Equals(input?.Trim(), "q", StringComparison.OrdinalIgnoreCase);
    }

    public static bool TryParseNumber(string input, out double number)
    {
        var trimmed = input.Trim();
        return double.TryParse(trimmed, NumberStyles.Float, CultureInfo.CurrentCulture, out number)
            || double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out number);
    }
}
