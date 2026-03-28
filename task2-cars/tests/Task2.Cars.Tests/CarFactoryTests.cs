using Task2.Cars.Abstractions;
using Task2.Cars.Cars;
using Task2.Cars.Factories;
using Task2.Cars.Parsing;

using MechanicalCar = Task2.Cars.Features.Powertrain.IMechanical;
using ManualTransmission = Task2.Cars.Features.Transmission.IMechanical;

namespace Task2.Cars.Tests;

public class CarFactoryTests
{
    private readonly CarFactory _factory = new();

    [Theory]
    [InlineData(CarType.Tesla, typeof(TeslaCar))]
    [InlineData(CarType.Toyota, typeof(ToyotaCar))]
    [InlineData(CarType.Bmw, typeof(BmwCar))]
    [InlineData(CarType.Nissan, typeof(NissanCar))]
    public void Create_ReturnsExpectedCarType(CarType carType, Type expectedType)
    {
        var car = _factory.Create(carType);

        Assert.IsType(expectedType, car);
    }

    [Theory]
    [InlineData(CarType.Tesla, "Tesla: электрокар")]
    [InlineData(CarType.Toyota, "Toyota: бензиновый автомобиль")]
    [InlineData(CarType.Bmw, "BMW: бензиновый автомобиль")]
    [InlineData(CarType.Nissan, "Nissan: электрокар")]
    public void Create_ReturnsCarWithExpectedDescriptionPrefix(CarType carType, string expectedPrefix)
    {
        var car = _factory.Create(carType);

        Assert.StartsWith(expectedPrefix, car.GetDescription());
    }

    [Fact]
    public void Create_Tesla_ImplementsElectricAndAutomaticInterfaces()
    {
        var car = _factory.Create(CarType.Tesla);

        Assert.IsAssignableFrom<Task2.Cars.Features.Powertrain.IElectric>(car);
        Assert.IsAssignableFrom<Task2.Cars.Features.Transmission.IAutomatical>(car);
    }

    [Fact]
    public void Create_Bmw_ImplementsMechanicalCarAndManualTransmissionInterfaces()
    {
        var car = _factory.Create(CarType.Bmw);

        Assert.IsAssignableFrom<MechanicalCar>(car);
        Assert.IsAssignableFrom<ManualTransmission>(car);
    }

    [Theory]
    [InlineData("Tesla", CarType.Tesla)]
    [InlineData(" toyota ", CarType.Toyota)]
    [InlineData("BMW", CarType.Bmw)]
    [InlineData("nissan", CarType.Nissan)]
    public void Parser_RecognizesKnownBrands(string input, CarType expectedType)
    {
        var success = CarBrandParser.TryParse(input, out var parsedType);

        Assert.True(success);
        Assert.Equal(expectedType, parsedType);
    }

    [Fact]
    public void Parser_ReturnsFalseForUnknownBrand()
    {
        var success = CarBrandParser.TryParse("Audi", out _);

        Assert.False(success);
    }
}
