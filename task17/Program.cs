using System;
using System.Threading;

namespace Task17;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Запуск демонстрации задачи №19 ===");

        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);
        
        server.Start();

        // Добавляем 5 экземпляров TestCommand (каждый должен выполниться по 3 раза)
        for (int i = 1; i <= 5; i++)
        {
            server.AddCommand(new TestCommand(id: i, maxRuns: 3));
        }

        // Даем командам немного времени на циклическое выполнение.
        // За это время они успеют сделать несколько шагов, чередуясь друг с другом.
        Thread.Sleep(50); 

        Console.WriteLine("\n--> Инициируем HardStop...");
        server.HardStop();

        Console.WriteLine("Демонстрация завершена.");
    }
}