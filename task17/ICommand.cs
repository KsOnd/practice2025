namespace Task17;

public interface ICommand
{
    void Execute();
}

public interface IMultistepCommand : ICommand
{
    bool IsCompleted { get; }
}

public interface ILongRunningCommand : ICommand
{
    bool IsCompleted { get; }
}