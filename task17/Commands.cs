using System;
using System.Threading;

namespace Task17
{
    public class HardStopCommand : ICommand
    {
        private readonly ServerThread _serverThread;

        public HardStopCommand(ServerThread serverThread)
        {
            _serverThread = serverThread ?? throw new ArgumentNullException(nameof(serverThread));
        }

        public void Execute()
        {
            if (Thread.CurrentThread != _serverThread.Thread)
            {
                throw new InvalidOperationException("Команда HardStop может быть выполнена только внутри останавливаемого потока!");
            }
            _serverThread.HardStop();
        }
    }

    public class SoftStopCommand : ICommand
    {
        private readonly ServerThread _serverThread;

        public SoftStopCommand(ServerThread serverThread)
        {
            _serverThread = serverThread ?? throw new ArgumentNullException(nameof(serverThread));
        }

        public void Execute()
        {
            if (Thread.CurrentThread != _serverThread.Thread)
            {
                throw new InvalidOperationException("Команда SoftStop может быть выполнена только внутри останавливаемого потока!");
            }
            _serverThread.SoftStop();
        }
    }

    public class PrintCommand : ICommand
    {
        private readonly string _message;

        public PrintCommand(string message)
        {
            _message = message;
        }

        public void Execute()
        {
            Console.WriteLine($"[Выполнение] {_message}");
        }
    }
}