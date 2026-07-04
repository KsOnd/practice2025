using System;
using System.IO;
using System.Reflection;
using CommandLib;

namespace CommandRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Динамический запуск команд";

            string dllName = "FileSystemCommands.dll";
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dllName);
            string currentFolder = AppDomain.CurrentDomain.BaseDirectory;
            string fileMask = "*.dll";

            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    if (typeof(ICommand).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        ICommand commandInstance = null;

                        // Создаем экземпляры в зависимости от имени класса и параметров конструктора
                        if (type.Name == "DirectorySizeCommand")
                        {
                            commandInstance = (ICommand)Activator.CreateInstance(type, new object[] { currentFolder });
                        }
                        else if (type.Name == "FindFilesCommand")
                        {
                            commandInstance = (ICommand)Activator.CreateInstance(type, new object[] { currentFolder, fileMask });
                        }

                        commandInstance?.Execute();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка рефлексии при выполнении команд: {ex.Message}");
            }
        }
    }
}