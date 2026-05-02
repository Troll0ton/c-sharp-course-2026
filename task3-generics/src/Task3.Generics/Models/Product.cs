using Task3.Generics.Abstractions;

namespace Task3.Generics.Models;

public sealed record Product(int Id, string Name, decimal Price) : IEntity;
