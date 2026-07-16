using System;
using System.Threading;

namespace Task17;

public class LongRunningCommand : IMultistepCommand
{
    private readonly int _totalSteps;
    private int _currentStep = 0;

    public bool IsCompleted => _currentStep >= _totalSteps;

    public LongRunningCommand(int totalSteps) => _totalSteps = totalSteps;

    public void Execute()
    {
        if (IsCompleted) return;
        _currentStep++;
        Thread.Sleep(10); // Имитация тяжелой работы 1 "кванта" времени (10мс)
    }
}

public class ShortCommand : ICommand
{
    private readonly Action _action;
    public ShortCommand(Action action) => _action = action;
    
    public void Execute()
    {
        Thread.Sleep(5); // Быстрая команда
        _action?.Invoke();
    }
}

public class TestCommand : ILongRunningCommand
{
    public int Id { get; }
    private int _counter = 0;
    private readonly int _maxRuns;

    // По условию задачи нам нужно выполнить команду 3 раза
    public bool IsCompleted => _counter >= _maxRuns;

    public TestCommand(int id, int maxRuns = 3)
    {
        Id = id;
        _maxRuns = maxRuns;
    }

    public void Execute()
    {
        if (IsCompleted) return;
        _counter++;
        Console.WriteLine($"Поток {Id} вызов {_counter}");
    }
}