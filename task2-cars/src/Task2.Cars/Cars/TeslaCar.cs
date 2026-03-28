namespace Task2.Cars.Cars;

public sealed class TeslaCar : ElectricAutomaticCar
{
    public TeslaCar()
        : base("Tesla", 5, 82, 1)
    {
    }

    protected override string GetSpecialFeatureDescription()
    {
        return "автопилотом и большим сенсорным экраном";
    }
}
