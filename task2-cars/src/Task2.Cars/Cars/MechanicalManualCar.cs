using Task2.Cars.Abstractions;

namespace Task2.Cars.Cars;

public abstract class MechanicalManualCar : ACar, Task2.Cars.Features.Powertrain.IMechanical, Task2.Cars.Features.Transmission.IMechanical
{
    protected MechanicalManualCar(string brand, int seats, string fuelType, int manualGears)
        : base(brand, seats)
    {
        FuelType = fuelType;
        ManualGears = manualGears;
    }

    public string FuelType { get; }

    public int ManualGears { get; }

    protected override string GetEnergySourceDescription()
    {
        return $"{FuelType} автомобиль";
    }

    protected override string GetTransmissionDescription()
    {
        return $"механической коробкой передач ({ManualGears} ступеней)";
    }
}
