namespace Task2.Cars.Cars;

public sealed class BmwCar : MechanicalManualCar
{
    public BmwCar()
        : base("BMW", 5, "бензиновый", 6)
    {
    }

    protected override string GetSpecialFeatureDescription()
    {
        return "спортивной подвеской и режимом dynamic drive";
    }
}
