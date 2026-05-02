using Task3.Generics.Models;
using Task3.Generics.Repositories;

namespace Task3.Generics.Tests;

public class RepositoryTests
{
    [Fact]
    public void Add_IncreasesCountAndStoresProductById()
    {
        var repository = new Repository<Product>();
        var product = new Product(1, "Laptop", 1200m);

        repository.Add(product);

        Assert.Equal(1, repository.Count);
        Assert.Equal(product, repository.GetById(1));
    }

    [Fact]
    public void Add_WorksWithDifferentEntityTypes()
    {
        var productRepository = new Repository<Product>();
        var userRepository = new Repository<User>();

        productRepository.Add(new Product(1, "Laptop", 1200m));
        userRepository.Add(new User(1, "Alice"));

        Assert.Equal("Laptop", productRepository.GetById(1)?.Name);
        Assert.Equal("Alice", userRepository.GetById(1)?.Name);
    }

    [Fact]
    public void Add_ThrowsForDuplicateId()
    {
        var repository = new Repository<Product>();
        repository.Add(new Product(1, "Laptop", 1200m));

        Assert.Throws<InvalidOperationException>(() => repository.Add(new Product(1, "Monitor", 450m)));
    }

    [Fact]
    public void Remove_ReturnsTrueAndDeletesExistingItem()
    {
        var repository = new Repository<User>();
        repository.Add(new User(1, "Alice"));

        var removed = repository.Remove(1);

        Assert.True(removed);
        Assert.Equal(0, repository.Count);
        Assert.Null(repository.GetById(1));
    }

    [Fact]
    public void Remove_ReturnsFalseForMissingItem()
    {
        var repository = new Repository<User>();

        var removed = repository.Remove(42);

        Assert.False(removed);
    }

    [Fact]
    public void GetById_ReturnsNullForMissingItem()
    {
        var repository = new Repository<Product>();

        var product = repository.GetById(42);

        Assert.Null(product);
    }

    [Fact]
    public void GetAll_ReturnsAllStoredItems()
    {
        var repository = new Repository<Product>();
        var first = new Product(1, "Laptop", 1200m);
        var second = new Product(2, "Mouse", 25m);
        repository.Add(first);
        repository.Add(second);

        var products = repository.GetAll();

        Assert.Equal(2, products.Count);
        Assert.Contains(first, products);
        Assert.Contains(second, products);
    }

    [Fact]
    public void Find_ReturnsItemsMatchedByPredicate()
    {
        var repository = new Repository<Product>();
        var expensive = new Product(1, "Laptop", 1200m);
        var cheap = new Product(2, "Mouse", 25m);
        repository.Add(expensive);
        repository.Add(cheap);

        var result = repository.Find(product => product.Price > 1000m);

        Assert.Single(result);
        Assert.Equal(expensive, result[0]);
    }
}
