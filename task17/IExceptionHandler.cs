using System;

namespace Task17
{
    public interface IExceptionHandler
    {
        void Handle(ICommand command, Exception exception);
    }

    public class ConsoleExceptionHandler : IExceptionHandler
    {
        public void Handle(ICommand command, Exception exception)
        {
            Console.WriteLine($"Исключение в команде {command.GetType().Name}: {exception.Message}");
        }
    }
}