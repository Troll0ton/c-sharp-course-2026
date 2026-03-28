namespace Task2.Cars.Cars;

public sealed class ToyotaCar : MechanicalAutomaticCar
{
    public ToyotaCar()
        : base("Toyota", 5, "бензиновый", 6)
    {
    }

    protected override string GetSpecialFeatureDescription()
    {
        return "экономичным расходом топлива и камерой заднего вида";
    }
}
