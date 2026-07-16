using System.Collections.Generic;

namespace Task17;

public class RoundRobinScheduler : IScheduler
{
    private readonly Queue<ICommand> _jobs = new Queue<ICommand>();

    public bool HasCommand() => _jobs.Count > 0;

    public ICommand Select() => _jobs.Dequeue();

    public void Add(ICommand cmd) => _jobs.Enqueue(cmd);
}