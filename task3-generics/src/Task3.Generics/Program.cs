using Task3.Generics.Models;
using Task3.Generics.Repositories;
using Task3.Generics.Utilities;

var productRepository = new Repository<Product>();
productRepository.Add(new Product(1, "Laptop", 1200m));
productRepository.Add(new Product(2, "Mouse", 25m));
productRepository.Add(new Product(3, "Monitor", 450m));

var userRepository = new Repository<User>();
userRepository.Add(new User(1, "Alice"));
userRepository.Add(new User(2, "Bob"));

Console.WriteLine("Products count: " + productRepository.Count);
Console.WriteLine("Product with Id 1: " + productRepository.GetById(1));
Console.WriteLine("Users count: " + userRepository.Count);
Console.WriteLine("User with Id 2: " + userRepository.GetById(2));

Console.WriteLine();
Console.WriteLine("Products more expensive than 1000:");
foreach (var product in productRepository.Find(product => product.Price > 1000m))
{
    Console.WriteLine(product);
}

Console.WriteLine();
Console.WriteLine("Distinct numbers:");
foreach (var number in CollectionUtils.Distinct(new List<int> { 1, 2, 2, 3, 1 }))
{
    Console.Write(number + " ");
}

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("Grouped words by length:");
var groupedWords = CollectionUtils.GroupBy(
    new List<string> { "cat", "dog", "bird", "fish" },
    word => word.Length);

foreach (var group in groupedWords)
{
    Console.WriteLine(group.Key + ": " + string.Join(", ", group.Value));
}

Console.WriteLine();
Console.WriteLine("Merged word counters:");
var mergedCounters = CollectionUtils.Merge(
    new Dictionary<string, int> { ["csharp"] = 2, ["dotnet"] = 1 },
    new Dictionary<string, int> { ["csharp"] = 3, ["tests"] = 4 },
    (left, right) => left + right);

foreach (var pair in mergedCounters)
{
    Console.WriteLine(pair.Key + ": " + pair.Value);
}

Console.WriteLine();
var mostExpensiveProduct = CollectionUtils.MaxBy(new List<Product>(productRepository.GetAll()), product => product.Price);
Console.WriteLine("Most expensive product: " + mostExpensiveProduct);

try
{
    productRepository.Add(new Product(1, "Duplicate laptop", 1300m));
}
catch (InvalidOperationException exception)
{
    Console.WriteLine();
    Console.WriteLine("Duplicate add failed: " + exception.Message);
}
