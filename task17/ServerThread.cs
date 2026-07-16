using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Task17;

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new(100);
    
    private readonly IScheduler? _scheduler; 
    private readonly Thread _thread;
    private readonly CancellationTokenSource _cts = new();
    private volatile bool _isHardStopped = false;

    public ServerThread()
    {
        _scheduler = null; 
        _thread = new Thread(Run);
    }

    public ServerThread(IScheduler scheduler)
    {
        _scheduler = scheduler;
        _thread = new Thread(Run);
    }

    public void Start() => _thread.Start();

    public void AddCommand(ICommand cmd)
    {
        if (_isHardStopped || _cts.IsCancellationRequested) return;
        _queue.Add(cmd);
    }

    public void Stop()
    {
        _cts.Cancel();
        _queue.CompleteAdding();
    }

    public void HardStop()
    {
        _isHardStopped = true;
        _cts.Cancel();
        _queue.CompleteAdding();
    }

    private void Run()
    {
        while (!_isHardStopped && !_cts.IsCancellationRequested)
        {
            try
            {
                if (_scheduler != null && _scheduler.HasCommand())
                {
                    if (_queue.TryTake(out var newCmd)){
                        ProcessCommand(newCmd);
                    }
                    else
                    {
                        var scheduledCmd = _scheduler.Select();
                        ProcessCommand(scheduledCmd);
                    }
                }
                else
                {
                    var cmd = _queue.Take(_cts.Token);
                    ProcessCommand(cmd);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (InvalidOperationException)
            {
                break; // Возникает при остановке, когда CompleteAdding() вызван
            }
        }
        Console.WriteLine("Поток сервера полностью остановлен.");
    }

    private void ProcessCommand(ICommand cmd)
    {
        if (_isHardStopped) return;

        cmd.Execute();

        if (_scheduler != null && cmd is ILongRunningCommand longCmd && !longCmd.IsCompleted)
        {
            _scheduler.Add(cmd);
        }
    }
}