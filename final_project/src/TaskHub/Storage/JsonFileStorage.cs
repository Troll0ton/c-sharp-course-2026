using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskHub.Storage;

public sealed class JsonFileStorage<T> : IAsyncStorage<T>, IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string _filePath;
    private bool _disposed;

    public JsonFileStorage(string filePath)
    {
        _filePath = filePath;
    }

    public async Task<IReadOnlyList<T>> LoadAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!File.Exists(_filePath))
        {
            return [];
        }

        try
        {
            await using var stream = new FileStream(
                _filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 4096,
                useAsync: true);

            var items = await JsonSerializer.DeserializeAsync<List<T>>(stream, JsonOptions, cancellationToken);
            return items ?? [];
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException($"File '{_filePath}' contains invalid JSON.", exception);
        }
    }

    public async Task SaveAsync(IReadOnlyList<T> items, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = new FileStream(
            _filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true);

        await JsonSerializer.SerializeAsync(stream, items, JsonOptions, cancellationToken);
    }

    /// <summary>
    /// This type owns no long-lived unmanaged resources: each operation opens
    /// and closes its own <see cref="FileStream"/> within the method scope.
    /// <see cref="IDisposable"/> is implemented to give callers a consistent
    /// lifetime contract (and room to add cached resources later) — all this
    /// method does is mark the instance disposed so further calls fail fast.
    /// </summary>
    public void Dispose()
    {
        _disposed = true;
    }
}
