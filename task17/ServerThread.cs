using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Task17;

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new BlockingCollection<ICommand>(100); // Ограниченный размер
    private readonly IScheduler _scheduler;
    private readonly Thread _thread;
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    public ServerThread(IScheduler scheduler)
    {
        _scheduler = scheduler;
        _thread = new Thread(Run);
    }

    public void Start() => _thread.Start();

    public void AddCommand(ICommand cmd) => _queue.Add(cmd);

    public void Stop()
    {
        _cts.Cancel();
        _queue.CompleteAdding();
    }

    private void Run()
    {
        while (!_cts.IsCancellationRequested)
        {
            try
            {
                if (_scheduler.HasCommand())
                {
                    if (_queue.TryTake(out var newCmd))
                    {
                        ExecuteAndSchedule(newCmd);
                    }
                    else
                    {
                        var scheduledCmd = _scheduler.Select();
                        ExecuteAndSchedule(scheduledCmd);
                    }
                }
                else
                {
                    var cmd = _queue.Take(_cts.Token);
                    ExecuteAndSchedule(cmd);
                }
            }
            catch (OperationCanceledException) { break; }
            catch (InvalidOperationException) { break; }
        }
    }

    private void ExecuteAndSchedule(ICommand cmd)
    {
        cmd.Execute();
        if (cmd is IMultistepCommand multiCmd && !multiCmd.IsCompleted)
        {
            _scheduler.Add(cmd);
        }
    }
}