using System.Globalization;

using Task1.Calculator;

var culture = CultureInfo.CurrentCulture;

while (true)
{
    Console.WriteLine("Введите первое число (или q для выхода):");
    var leftInput = Console.ReadLine();
    if (Calculator.IsExitCommand(leftInput))
    {
        break;
    }

    if (string.IsNullOrWhiteSpace(leftInput) || !Calculator.TryParseNumber(leftInput, out var left))
    {
        Console.WriteLine("Некорректный ввод первого числа.");
        Console.WriteLine();
        continue;
    }

    Console.WriteLine("Введите второе число (или q для выхода):");
    var rightInput = Console.ReadLine();
    if (Calculator.IsExitCommand(rightInput))
    {
        break;
    }

    if (string.IsNullOrWhiteSpace(rightInput) || !Calculator.TryParseNumber(rightInput, out var right))
    {
        Console.WriteLine("Некорректный ввод второго числа.");
        Console.WriteLine();
        continue;
    }

    Console.WriteLine("Выберите операцию (+, -, *, /) или q для выхода:");
    var operationInput = Console.ReadLine();
    if (Calculator.IsExitCommand(operationInput))
    {
        break;
    }

    if (string.IsNullOrWhiteSpace(operationInput) || operationInput.Trim().Length != 1)
    {
        Console.WriteLine("Некорректный ввод операции.");
        Console.WriteLine();
        continue;
    }

    var operation = operationInput.Trim()[0];
    if (!Calculator.TryCalculate(left, right, operation, out var result, out var error))
    {
        Console.WriteLine(error);
        Console.WriteLine();
        continue;
    }

    Console.WriteLine($"Результат: {result.ToString("G17", culture)}");
    Console.WriteLine();
}
