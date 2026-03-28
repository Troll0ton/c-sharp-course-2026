using Task2.Cars.Abstractions;
using Task2.Cars.Features.Powertrain;
using Task2.Cars.Features.Transmission;

namespace Task2.Cars.Cars;

public abstract class ElectricAutomaticCar : ACar, IElectric, IAutomatical
{
    protected ElectricAutomaticCar(string brand, int seats, int batteryCapacityKwh, int automaticGears)
        : base(brand, seats)
    {
        BatteryCapacityKwh = batteryCapacityKwh;
        AutomaticGears = automaticGears;
    }

    public int BatteryCapacityKwh { get; }

    public int AutomaticGears { get; }

    protected override string GetEnergySourceDescription()
    {
        return $"электрокар (батарея {BatteryCapacityKwh} кВт⋅ч)";
    }

    protected override string GetTransmissionDescription()
    {
        return AutomaticGears == 1
            ? "автоматической коробкой передач с одним режимом движения"
            : $"автоматической коробкой передач ({AutomaticGears} режимов движения)";
    }
}
