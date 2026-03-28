namespace Task2.Cars.Abstractions;

public interface ICar
{
    string Brand { get; }

    int Seats { get; }

    string GetDescription();
}
