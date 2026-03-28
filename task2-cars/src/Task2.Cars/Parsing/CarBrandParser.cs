using Task2.Cars.Abstractions;

namespace Task2.Cars.Parsing;

public static class CarBrandParser
{
    private static readonly Dictionary<string, CarType> KnownBrands = new(StringComparer.OrdinalIgnoreCase)
    {
        ["tesla"] = CarType.Tesla,
        ["toyota"] = CarType.Toyota,
        ["bmw"] = CarType.Bmw,
        ["nissan"] = CarType.Nissan
    };

    public static bool TryParse(string? input, out CarType carType)
    {
        carType = default;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        return KnownBrands.TryGetValue(input.Trim(), out carType);
    }
}
