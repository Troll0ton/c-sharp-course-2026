namespace Task2.Cars.Cars;

public sealed class NissanCar : ElectricAutomaticCar
{
    public NissanCar()
        : base("Nissan", 5, 60, 1)
    {
    }

    protected override string GetSpecialFeatureDescription()
    {
        return "системой рекуперации энергии и тихим ходом";
    }
}
