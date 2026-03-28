using Task2.Cars.Abstractions;
using Task2.Cars.Cars;

namespace Task2.Cars.Factories;

public sealed class CarFactory
{
    public ICar Create(CarType carType)
    {
        return carType switch
        {
            CarType.Tesla => new TeslaCar(),
            CarType.Toyota => new ToyotaCar(),
            CarType.Bmw => new BmwCar(),
            CarType.Nissan => new NissanCar(),
            _ => throw new ArgumentOutOfRangeException(nameof(carType), carType, "Неизвестный тип автомобиля.")
        };
    }
}
