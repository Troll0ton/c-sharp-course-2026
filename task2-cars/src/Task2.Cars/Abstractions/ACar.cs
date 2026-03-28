namespace Task2.Cars.Abstractions;

public abstract class ACar : ICar
{
    protected ACar(string brand, int seats)
    {
        Brand = brand;
        Seats = seats;
    }

    public string Brand { get; }

    public int Seats { get; }

    public virtual string GetDescription()
    {
        return $"{Brand}: {GetEnergySourceDescription()} с {GetTransmissionDescription()}, {GetSeatDescription()}, {GetSpecialFeatureDescription()}.";
    }

    protected virtual string GetSeatDescription()
    {
        return $"{Seats} местами";
    }

    protected abstract string GetEnergySourceDescription();

    protected abstract string GetTransmissionDescription();

    protected abstract string GetSpecialFeatureDescription();
}
