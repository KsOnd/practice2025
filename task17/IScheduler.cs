namespace Task17;

public interface IScheduler
{
    bool HasCommand();
    ICommand Select();
    void Add(ICommand cmd);
}