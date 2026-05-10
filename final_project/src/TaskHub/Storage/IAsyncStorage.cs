namespace TaskHub.Storage;

public interface IAsyncStorage<T>
{
    Task<IReadOnlyList<T>> LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(IReadOnlyList<T> items, CancellationToken cancellationToken = default);
}
