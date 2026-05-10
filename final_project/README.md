# TaskHub

TaskHub is a console task manager for creating, editing, searching, completing, saving, and loading tasks.

## Build and Test

```powershell
dotnet build .\TaskHub.slnx
dotnet test .\TaskHub.slnx
dotnet test .\TaskHub.slnx --collect:"XPlat Code Coverage"
```

## Run

```powershell
dotnet run --project .\src\TaskHub\TaskHub.csproj
```

Tasks are saved asynchronously to `tasks.json` near the built application.

## Implemented Requirements

- OOP models and services
- `List<T>` for task storage and `Dictionary<TKey, TValue>` for priority statistics
- generic async JSON file storage
- delegates through `TaskChangedHandler`, `Action<TaskItem>`, and `Func<TaskItem, bool>`
- exception handling and validation
- `IDisposable` in file storage and deadline monitor
- static helper classes and methods
- `async`/`await` for file saving and loading
- background deadline monitoring with `Task`, `PeriodicTimer`, and `CancellationToken`
