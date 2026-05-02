using Task3.Generics.Abstractions;

namespace Task3.Generics.Models;

public sealed record User(int Id, string Name) : IEntity;
