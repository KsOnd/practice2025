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
