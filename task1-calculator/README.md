# Task1 Calculator

Консольный калькулятор на C# с поддержкой операций:
- сложение (`+`)
- вычитание (`-`)
- умножение (`*`)
- деление (`/`)

Программа работает в бесконечном цикле и завершает работу по специальному символу `q` (регистр не важен).

## Требования

- .NET SDK 10.0 или выше

Проверить установленный SDK:

```powershell
dotnet --version
```

## Сборка

Из папки `task1-calculator`:

```powershell
dotnet build Task1.Calculator.sln
```

## Запуск приложения

Из папки `task1-calculator`:

```powershell
dotnet run --project .\src\Task1.Calculator\Task1.Calculator.csproj
```

## Использование

1. Введите первое число.
2. Введите второе число.
3. Введите операцию: `+`, `-`, `*` или `/`.
4. Получите результат в консоли.
5. Для выхода введите `q` на любом шаге.

## Запуск тестов

Из папки `task1-calculator`:

```powershell
dotnet test Task1.Calculator.sln
```
