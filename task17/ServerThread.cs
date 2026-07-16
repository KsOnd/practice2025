using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Task17
{
    public class ServerThread
    {
        private readonly BlockingCollection<ICommand> _queue = new BlockingCollection<ICommand>();
        private readonly Thread _thread;
        private readonly IExceptionHandler _exceptionHandler;
        private Action _strategy;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public Thread Thread => _thread;
        public bool IsRunning { get; private set; } = true;

        public ServerThread(IExceptionHandler exceptionHandler)
        {
            _exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
            _strategy = DefaultStrategy;
            _thread = new Thread(Run);
        }

        public void Start()
        {
            _thread.Start();
        }

        public void AddCommand(ICommand command)
        {
            if (!_queue.IsAddingCompleted)
            {
                _queue.Add(command);
            }
        }

        private void Run()
        {
            while (IsRunning)
            {
                _strategy();
            }
        }

        private void DefaultStrategy()
        {
            try
            {
                ICommand command = _queue.Take(_cts.Token);
                try
                {
                    command.Execute();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.Handle(command, ex);
                }
            }
            catch (OperationCanceledException)
            {
                Stop();
            }
            catch (Exception ex)
            {
                Stop();
            }
        }

        public void UpdateStrategy(Action newStrategy)
        {
            _strategy = newStrategy;
        }

        public void Stop()
        {
            IsRunning = false;
            _queue.CompleteAdding();
        }

        public void HardStop()
        {
            _cts.Cancel();
            Stop();
        }

        public void SoftStop()
        {
            UpdateStrategy(() =>
            {
                if (_queue.Count > 0)
                {
                    if (_queue.TryTake(out var command))
                    {
                        try
                        {
                            command.Execute();
                        }
                        catch (Exception ex)
                        {
                            _exceptionHandler.Handle(command, ex);
                        }
                    }
                }
                else
                {
                    Stop();
                }
            });
        }
    }
}