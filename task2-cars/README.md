# Task2 Cars

Консольное приложение на C#, которое запрашивает у пользователя марку автомобиля и выводит описание выбранной марки.

В проекте реализованы:
- `CarType` для выбора типа автомобиля
- `CarFactory`, возвращающая `ICar`
- `ICar` с методом `GetDescription()`
- абстрактный базовый класс `ACar`
- интерфейсы для типа автомобиля: `IElectric`, `IMechanical`
- интерфейсы для коробки передач: `IAutomatical`, `IMechanical`
- отдельный класс для каждой марки автомобиля

Из-за совпадения имени `IMechanical` для двух разных требований интерфейсы разнесены по разным пространствам имён:
- `Task2.Cars.Features.Powertrain.IMechanical`
- `Task2.Cars.Features.Transmission.IMechanical`

## Требования

- .NET SDK 10.0 или выше

## Сборка

Из папки `task2-cars`:

```powershell
dotnet build Task2.Cars.slnx
```

## Запуск приложения

Из папки `task2-cars`:

```powershell
dotnet run --project .\src\Task2.Cars\Task2.Cars.csproj
```

## Поддерживаемые марки

- `Tesla`
- `Toyota`
- `BMW`
- `Nissan`

Для завершения введите `done`.

## Тесты

Из папки `task2-cars`:

```powershell
dotnet test Task2.Cars.slnx
```
