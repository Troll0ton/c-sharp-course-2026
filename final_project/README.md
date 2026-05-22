# TaskHub

TaskHub - консольное приложение на C# для управления задачами. Проект покрывает полный цикл работы с задачами: создание, просмотр, редактирование, удаление, поиск, статистику, сохранение в JSON-файл, загрузку из файла и фоновую проверку дедлайнов.

## Как запустить

Команды выполняются из папки `final_project`.

```powershell
dotnet build .\TaskHub.slnx
dotnet run --project .\src\TaskHub\TaskHub.csproj
```

Тесты:

```powershell
dotnet test .\TaskHub.slnx
dotnet test .\TaskHub.slnx --collect:"XPlat Code Coverage"
```

При запуске приложение пытается загрузить задачи из `tasks.json`. Файл хранится рядом с собранным приложением, то есть в `bin/Debug/...` после запуска через `dotnet run`.

## Что умеет приложение

Главный экран - консольное меню:

```text
1. Create task
2. View tasks
3. Edit task
4. Delete task
5. Search tasks
6. Statistics
7. Save tasks
8. Load tasks
0. Save and exit
```

Пользователь может:

- создать задачу с названием, описанием, приоритетом, дедлайном и статусом;
- посмотреть все задачи, выполненные, невыполненные или задачи с высоким приоритетом;
- отредактировать название, описание, приоритет, дедлайн и статус;
- удалить выбранную задачу;
- найти задачи по части названия, статусу или приоритету;
- посмотреть статистику по общему количеству, выполненным, просроченным и приоритетам;
- сохранить задачи в файл;
- загрузить задачи из файла;
- получать фоновые уведомления о просроченных задачах.

## Структура проекта

```text
final_project/
  TaskHub.slnx
  src/
    TaskHub/
      Program.cs
      Common/
      ConsoleUi/
      Models/
      Services/
      Storage/
  tests/
    TaskHub.Tests/
```

Основные части:

- `Program.cs` - точка входа: создает сервисы, загружает данные, запускает монитор дедлайнов и меню.
- `Models/TaskItem.cs` - модель задачи с `Id`, `Title`, `Description`, `Priority`, `Deadline`, `Status`.
- `Models/TaskPriority.cs` - приоритеты `Low`, `Medium`, `High`.
- `Models/TaskItemStatus.cs` - статусы `New`, `InProgress`, `Done`.
- `ConsoleUi/ConsoleMenu.cs` - консольное меню и пользовательские сценарии.
- `ConsoleUi/ConsoleInput.cs` - чтение и валидация ввода из консоли.
- `Services/TaskService.cs` - бизнес-логика задач: создание, редактирование, удаление, поиск, замена списка.
- `Services/TaskQueries.cs` - статические предикаты для фильтрации задач.
- `Services/StatisticsService.cs` - расчет статистики.
- `Services/DeadlineMonitor.cs` - фоновая проверка просроченных задач.
- `Storage/IAsyncStorage.cs` - generic-интерфейс асинхронного хранилища.
- `Storage/JsonFileStorage.cs` - сохранение и загрузка JSON через async file I/O.
- `tests/TaskHub.Tests` - unit-тесты для сервисов, меню, ввода, статистики, хранилища и монитора.

## Как работает основной поток

1. `Program.Main` вычисляет путь к `tasks.json`.
2. Создается `JsonFileStorage<TaskItem>`.
3. Создаются `TaskService` и `StatisticsService`.
4. Приложение асинхронно загружает задачи из файла через `LoadAsync`.
5. Создается `DeadlineMonitor`, который проверяет дедлайны раз в 5 секунд.
6. `ConsoleMenu.RunAsync` запускает меню и подписывается на события изменения задач и просрочки.
7. При выходе через `0` задачи сохраняются через `SaveAsync`.

## Выполнение функциональных требований

| Требование | Где реализовано | Почему выполнено |
| --- | --- | --- |
| 1. Создание задачи | `ConsoleMenu.CreateTask`, `TaskService.CreateTask`, `TaskItem` | Пользователь вводит название, описание, приоритет, дедлайн и статус. Сервис создает задачу с новым `Guid`. |
| 2. Просмотр задач | `ConsoleMenu.ShowTasks`, `TaskService.GetAll`, `TaskQueries` | Меню показывает все задачи, выполненные, невыполненные и задачи с высоким приоритетом. |
| 3. Редактирование задачи | `ConsoleMenu.EditTask`, `TaskService.UpdateTask` | Можно изменить название, описание, приоритет, дедлайн и статус. Enter оставляет старое значение. |
| 4. Удаление задачи | `ConsoleMenu.DeleteTask`, `TaskService.DeleteTask` | Пользователь выбирает задачу из списка, сервис удаляет ее по `Id`. |
| 5. Поиск задач | `ConsoleMenu.SearchTasks`, `SearchByTitle`, `SearchByStatus`, `SearchByPriority` | Реализован поиск по части названия без учета регистра, по статусу и по приоритету. |
| 6. Статистика | `ConsoleMenu.ShowStatistics`, `StatisticsService.GetStatistics` | Показываются общее количество, выполненные, просроченные и словарь количества задач по приоритетам. |
| 7. Сохранение задач в файл | `ConsoleMenu.SaveTasksAsync`, `JsonFileStorage.SaveAsync` | Все задачи сериализуются в JSON-файл `tasks.json`. |
| 8. Загрузка задач из файла | `Program.Main`, `ConsoleMenu.LoadTasksAsync`, `JsonFileStorage.LoadAsync` | Загрузка выполняется при старте и вручную из меню. Данные заменяют текущий список задач. |
| 9. Асинхронная работа с файлами | `IAsyncStorage<T>`, `JsonFileStorage<T>` | Используются `async`/`await`, `FileStream` с `useAsync: true`, `JsonSerializer.SerializeAsync` и `DeserializeAsync`. |
| 10. Фоновая проверка задач | `DeadlineMonitor`, `ConsoleMenu.ShowOverdueNotification` | Отдельный `Task` с `PeriodicTimer` раз в 5 секунд ищет просроченные задачи и выводит уведомление. |

## Выполнение технических требований

| Требование | Реализация |
| --- | --- |
| ООП и классы | Проект разделен на модели, сервисы, UI и хранилище: `TaskItem`, `TaskService`, `StatisticsService`, `ConsoleMenu`, `DeadlineMonitor`, `JsonFileStorage<T>`. |
| Коллекции `List` и `Dictionary` | `TaskService` хранит задачи в `List<TaskItem>`. `Statistics.PriorityCounts` использует `Dictionary<TaskPriority, int>`. |
| Generic | `IAsyncStorage<T>` и `JsonFileStorage<T>` работают с любым типом данных, в проекте используются как `JsonFileStorage<TaskItem>`. |
| Делегаты | Есть пользовательский делегат `TaskChangedHandler`; также используются `Func<TaskItem, bool>` для поиска и `Action<TaskItem>` в событии просрочки. |
| Обработка исключений | `ConsoleMenu.RunMenuLoopAsync` обрабатывает `ValidationException`, `InvalidOperationException`, `IOException`; `Program.Main` обрабатывает ошибки загрузки. |
| `IDisposable` | `JsonFileStorage<T>` реализует `IDisposable`; `DeadlineMonitor` реализует `IDisposable` и `IAsyncDisposable` для остановки фоновой задачи и освобождения `CancellationTokenSource`. |
| `static` классы/методы | `AppConstants`, `ConsoleInput`, `TaskQueries` - статические классы; в них вынесены константы, ввод и повторно используемые предикаты. |
| `async`/`await` | Асинхронные методы используются в `Program.Main`, `ConsoleMenu.SaveTasksAsync`, `ConsoleMenu.LoadTasksAsync`, `JsonFileStorage<T>`, `DeadlineMonitor`. |
| Многопоточность | `DeadlineMonitor.Start` запускает фоновый `Task`; `TaskService` защищает общий список задач через `lock`; вывод уведомлений синхронизируется через `_consoleSync`. |

## Хранение данных

Задачи сохраняются в JSON с читаемым форматированием и enum-значениями строками:

```json
[
  {
    "Id": "e1ec70e9-91d1-4c61-8bcb-30d68e1b8c6b",
    "Title": "Prepare report",
    "Description": "Finish final report",
    "Priority": "High",
    "Deadline": "2026-05-25T18:00:00",
    "Status": "InProgress"
  }
]
```

Если файл отсутствует, загрузка возвращает пустой список. Если JSON поврежден, ошибка преобразуется в `InvalidOperationException` с понятным сообщением.

## Валидация и безопасность данных

- Пустое название задачи запрещено.
- Ввод enum можно делать числом или названием.
- Дата вводится в формате `yyyy-MM-dd HH:mm`.
- `TaskService` возвращает копии задач, чтобы внешний код не менял внутренний список напрямую.
- Доступ к списку задач синхронизирован через `lock`, потому что меню и фоновый монитор могут обращаться к данным одновременно.


