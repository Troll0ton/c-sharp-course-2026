using Task3.Generics.Models;
using Task3.Generics.Utilities;

namespace Task3.Generics.Tests;

public class CollectionUtilsTests
{
    [Fact]
    public void Distinct_RemovesDuplicateIntsAndKeepsFirstOccurrenceOrder()
    {
        var source = new List<int> { 3, 1, 3, 2, 1 };

        var result = CollectionUtils.Distinct(source);

        Assert.Equal(new List<int> { 3, 1, 2 }, result);
    }

    [Fact]
    public void Distinct_RemovesDuplicateStrings()
    {
        var source = new List<string> { "one", "two", "one", "three", "two" };

        var result = CollectionUtils.Distinct(source);

        Assert.Equal(new List<string> { "one", "two", "three" }, result);
    }

    [Fact]
    public void GroupBy_GroupsWordsByLength()
    {
        var source = new List<string> { "cat", "dog", "bird", "fish", "csharp" };

        var result = CollectionUtils.GroupBy(source, word => word.Length);

        Assert.Equal(new List<string> { "cat", "dog" }, result[3]);
        Assert.Equal(new List<string> { "bird", "fish" }, result[4]);
        Assert.Equal(new List<string> { "csharp" }, result[6]);
    }

    [Fact]
    public void GroupBy_ReturnsEmptyDictionaryForEmptyList()
    {
        var result = CollectionUtils.GroupBy(new List<string>(), word => word.Length);

        Assert.Empty(result);
    }

    [Fact]
    public void Merge_CopiesUniqueKeysAndResolvesConflicts()
    {
        var first = new Dictionary<string, int>
        {
            ["csharp"] = 2,
            ["dotnet"] = 1,
        };
        var second = new Dictionary<string, int>
        {
            ["csharp"] = 3,
            ["tests"] = 4,
        };

        var result = CollectionUtils.Merge(first, second, (left, right) => left + right);

        Assert.Equal(5, result["csharp"]);
        Assert.Equal(1, result["dotnet"]);
        Assert.Equal(4, result["tests"]);
    }

    [Fact]
    public void Merge_DoesNotMutateSources()
    {
        var first = new Dictionary<string, int> { ["same"] = 1 };
        var second = new Dictionary<string, int> { ["same"] = 2 };

        CollectionUtils.Merge(first, second, (left, right) => left + right);

        Assert.Equal(1, first["same"]);
        Assert.Equal(2, second["same"]);
    }

    [Fact]
    public void MaxBy_ReturnsItemWithMaximumSelectedValue()
    {
        var products = new List<Product>
        {
            new(1, "Mouse", 25m),
            new(2, "Laptop", 1200m),
            new(3, "Monitor", 450m),
        };

        var result = CollectionUtils.MaxBy(products, product => product.Price);

        Assert.Equal(products[1], result);
    }

    [Fact]
    public void MaxBy_ThrowsForEmptyList()
    {
        Assert.Throws<InvalidOperationException>(
            () => CollectionUtils.MaxBy(new List<int>(), number => number));
    }
}
