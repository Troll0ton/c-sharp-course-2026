using System.Globalization;

namespace TaskHub.ConsoleUi;

public static class ConsoleInput
{
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm";

    public static string ReadRequiredString(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var value = Console.ReadLine()?.Trim() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            Console.WriteLine("Value cannot be empty.");
        }
    }

    public static string? ReadOptionalString(string prompt, string currentValue)
    {
        Console.Write($"{prompt} [{currentValue}]: ");
        var value = Console.ReadLine();
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public static TEnum ReadEnum<TEnum>(string prompt)
        where TEnum : struct, Enum
    {
        var values = Enum.GetValues<TEnum>();

        while (true)
        {
            Console.WriteLine(prompt);
            foreach (var value in values)
            {
                Console.WriteLine($"{Convert.ToInt32(value, CultureInfo.InvariantCulture)}. {value}");
            }

            Console.Write("Choose: ");
            var input = Console.ReadLine();
            if (int.TryParse(input, out var number) && Enum.IsDefined(typeof(TEnum), number))
            {
                return (TEnum)Enum.ToObject(typeof(TEnum), number);
            }

            if (Enum.TryParse<TEnum>(input, ignoreCase: true, out var parsed) && Enum.IsDefined(parsed))
            {
                return parsed;
            }

            Console.WriteLine("Invalid choice.");
        }
    }

    public static TEnum? ReadOptionalEnum<TEnum>(string prompt, TEnum currentValue)
        where TEnum : struct, Enum
    {
        Console.WriteLine($"{prompt} [{currentValue}]");
        Console.WriteLine("Press Enter to keep the current value.");
        var values = Enum.GetValues<TEnum>();

        foreach (var value in values)
        {
            Console.WriteLine($"{Convert.ToInt32(value, CultureInfo.InvariantCulture)}. {value}");
        }

        while (true)
        {
            Console.Write("Choose: ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (int.TryParse(input, out var number) && Enum.IsDefined(typeof(TEnum), number))
            {
                return (TEnum)Enum.ToObject(typeof(TEnum), number);
            }

            if (Enum.TryParse<TEnum>(input, ignoreCase: true, out var parsed) && Enum.IsDefined(parsed))
            {
                return parsed;
            }

            Console.WriteLine("Invalid choice.");
        }
    }

    public static DateTime ReadDateTime(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} ({DateTimeFormat}): ");
            var value = Console.ReadLine();

            if (DateTime.TryParseExact(
                    value,
                    DateTimeFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var deadline))
            {
                return deadline;
            }

            Console.WriteLine("Invalid date and time.");
        }
    }

    public static DateTime? ReadOptionalDateTime(string prompt, DateTime currentValue)
    {
        while (true)
        {
            Console.Write($"{prompt} [{currentValue:yyyy-MM-dd HH:mm}]: ");
            var value = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (DateTime.TryParseExact(
                    value,
                    DateTimeFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var deadline))
            {
                return deadline;
            }

            Console.WriteLine("Invalid date and time.");
        }
    }

    public static int ReadMenuChoice(string prompt, int minValue, int maxValue)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();

            if (int.TryParse(input, out var choice) && choice >= minValue && choice <= maxValue)
            {
                return choice;
            }

            Console.WriteLine("Invalid menu item.");
        }
    }
}
