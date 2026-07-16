using System;

namespace Task17
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ТЕСТ 1: Демонстрация SoftStop ===");
            var handler = new ConsoleExceptionHandler();
            var server1 = new ServerThread(handler);
            server1.Start();

            server1.AddCommand(new PrintCommand("Команда №1"));
            server1.AddCommand(new PrintCommand("Команда №2"));
            server1.AddCommand(new SoftStopCommand(server1));
            server1.AddCommand(new PrintCommand("Команда №3 (должна выполниться, так как это SoftStop)"));

            server1.Thread.Join();
            Console.WriteLine("-> Поток сервера 1 успешно остановлен через SoftStop.\n");

            Console.WriteLine("=== ТЕСТ 2: Демонстрация HardStop ===");
            var server2 = new ServerThread(handler);
            server2.Start();

            server2.AddCommand(new PrintCommand("Команда №1 во 2-м сервере"));
            server2.AddCommand(new HardStopCommand(server2));
            server2.AddCommand(new PrintCommand("Команда №2 во 2-м сервере (НЕ должна выполниться!)"));

            server2.Thread.Join();
            Console.WriteLine("-> Поток сервера 2 успешно остановлен через HardStop.");
        }
    }
}