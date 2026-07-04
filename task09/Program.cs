using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace task09
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Анализатор метаданных библиотеки";

            if (args.Length == 0)
            {
                Console.WriteLine("Не указан путь к библиотеке.");
                Console.WriteLine("Использование: dotnet run --project task09 -- <путь_к_библиотеке.dll>");
                return;
            }

            string dllPath = args[0];

            if (!File.Exists(dllPath))
            {
                Console.WriteLine($"Файл не найден по пути: {dllPath}");
                return;
            }

            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);
                Console.WriteLine($"\n=== Метаданные библиотеки: {assembly.GetName().Name} ===\n");
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    if (!type.IsClass) continue;

                    Console.WriteLine($"Класс: {type.FullName}");

                    var attributes = type.GetCustomAttributes();
                    if (attributes.Any())
                    {
                        Console.WriteLine("  Атрибуты:");
                        foreach (var attr in attributes)
                        {
                            Console.WriteLine($"    - [{attr.GetType().Name}]");
                        }
                    }

                    var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    Console.WriteLine("  Конструкторы:");
                    if (constructors.Length == 0) Console.WriteLine("    (нет)");
                    
                    foreach (var ctor in constructors)
                    {
                        string paramStr = string.Join(", ", ctor.GetParameters()
                                                .Select(p => $"{p.ParameterType.Name} {p.Name}"));
                        
                        Console.WriteLine($"    - {type.Name}({paramStr})");
                    }

                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                    var actualMethods = methods.Where(m => !m.IsSpecialName).ToArray();

                    Console.WriteLine("  Методы:");
                    if (actualMethods.Length == 0) Console.WriteLine("    (нет)");
                    
                    foreach (var method in actualMethods)
                    {
                        string paramStr = string.Join(", ", method.GetParameters()
                                                .Select(p => $"{p.ParameterType.Name} {p.Name}"));
                        
                        Console.WriteLine($"    - {method.ReturnType.Name} {method.Name}({paramStr})");
                    }

                    Console.WriteLine(new string('-', 50));
                }
            }
            catch (BadImageFormatException)
            {
                Console.WriteLine("Указанный файл не является корректной .NET сборкой (DLL).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла непредвиденная ошибка: {ex.Message}");
            }
        }
    }
}