using System;
using PluginContracts;

namespace TestPlugins
{
    [PluginLoad("PluginA")]
    public class FirstPlugin : ICommand
    {
        public void Execute() => Console.WriteLine("PluginA: Инициализация базовых подсистем.");
    }

    // Зависит от PluginA
    [PluginLoad("PluginB", "PluginA")]
    public class SecondPlugin : ICommand
    {
        public void Execute() => Console.WriteLine("PluginB: Подключение к базе данных (требует PluginA).");
    }

    // Зависит от PluginB
    [PluginLoad("PluginC", "PluginB")]
    public class ThirdPlugin : ICommand
    {
        public void Execute() => Console.WriteLine("PluginC: Запуск пользовательского интерфейса (требует PluginB).");
    }

    // Плагин без зависимостей
    [PluginLoad("IndependentPlugin")]
    public class IndependentPlugin : ICommand
    {
        public void Execute() => Console.WriteLine("IndependentPlugin: Фоновая задача, ни от кого не зависит.");
    }
}