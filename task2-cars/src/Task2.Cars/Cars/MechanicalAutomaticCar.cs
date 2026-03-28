using Task2.Cars.Abstractions;
using Task2.Cars.Features.Transmission;

namespace Task2.Cars.Cars;

public abstract class MechanicalAutomaticCar : ACar, Task2.Cars.Features.Powertrain.IMechanical, IAutomatical
{
    protected MechanicalAutomaticCar(string brand, int seats, string fuelType, int automaticGears)
        : base(brand, seats)
    {
        FuelType = fuelType;
        AutomaticGears = automaticGears;
    }

    public string FuelType { get; }

    public int AutomaticGears { get; }

    protected override string GetEnergySourceDescription()
    {
        return $"{FuelType} автомобиль";
    }

    protected override string GetTransmissionDescription()
    {
        return $"автоматической коробкой передач ({AutomaticGears} ступеней)";
    }
}
