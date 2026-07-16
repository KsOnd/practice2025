namespace Task17;

public interface ICommand
{
    void Execute();
}

public interface IMultistepCommand : ICommand
{
    bool IsCompleted { get; }
}