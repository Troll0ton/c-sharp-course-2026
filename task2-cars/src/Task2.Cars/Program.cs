using System.Text;

using Task2.Cars.Factories;
using Task2.Cars.Parsing;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

var carFactory = new CarFactory();

while (true)
{
    Console.Write("Введите марку автомобиля или done для остановки ввода: ");
    var input = Console.ReadLine();

    if (string.Equals(input?.Trim(), "done", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    if (!CarBrandParser.TryParse(input, out var carType))
    {
        Console.WriteLine("Марка не распознана. Доступно: Tesla, Toyota, BMW, Nissan.");
        Console.WriteLine();
        continue;
    }

    var car = carFactory.Create(carType);
    Console.WriteLine(car.GetDescription());
    Console.WriteLine();
}
