using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginContracts;

namespace PluginRunner
{
    class Program
    {
        class PluginMetadata
        {
            public string Name { get; set; }
            public string[] Dependencies { get; set; }
            public Type PluginType { get; set; }
        }

        static void Main(string[] args)
        {
            Console.Title = "Менеджер загрузки плагинов (Граф зависимостей)";
            
            string pluginsFolder = AppDomain.CurrentDomain.BaseDirectory;
            
            Console.WriteLine($"Поиск библиотек в каталоге: {pluginsFolder}\n");

            var allPlugins = new List<PluginMetadata>();
            string[] dllFiles = Directory.GetFiles(pluginsFolder, "*.dll");
            foreach (string dllPath in dllFiles)
            {
                try {
                    Assembly assembly = Assembly.LoadFrom(dllPath);
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        var attr = type.GetCustomAttribute<PluginLoadAttribute>();
                        if (attr != null && typeof(ICommand).IsAssignableFrom(type) && !type.IsAbstract)
                        {
                            allPlugins.Add(new PluginMetadata
                            {
                                Name = attr.Name,
                                Dependencies = attr.Dependencies,
                                PluginType = type
                            });
                        }
                    }
                }
                catch (BadImageFormatException) { }
                catch (Exception ex) { Console.WriteLine($"Ошибка при загрузке {Path.GetFileName(dllPath)}: {ex.Message}"); }
            }

            if (allPlugins.Count == 0)
            {
                Console.WriteLine("Плагины не найдены.");
                return;
            }

            Console.WriteLine($"Найдено плагинов: {allPlugins.Count}. Выполняем разрешение зависимостей...\n");

            List<Type> sortedPluginTypes;
            try
            {
                sortedPluginTypes = TopologicalSort(allPlugins);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка разрешения зависимостей: {ex.Message}");
                return;
            }

            Console.WriteLine("=== Запуск плагинов ===\n");
            foreach (var type in sortedPluginTypes)
            {
                try
                {
                    var command = (ICommand)Activator.CreateInstance(type);
                    command.Execute();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при выполнении плагина {type.Name}: {ex.Message}");
                }
            }
        }

        static List<Type> TopologicalSort(List<PluginMetadata> plugins)
        {
            var sortedList = new List<Type>();
            var visited = new Dictionary<string, int>(); 
            var pluginDict = plugins.ToDictionary(p => p.Name);

            void Dfs(string pluginName)
            {
                if (!visited.ContainsKey(pluginName)) visited[pluginName] = 0;

                if (visited[pluginName] == 1)
                    throw new InvalidOperationException($"Обнаружена циклическая зависимость для плагина: '{pluginName}'.");
                
                if (visited[pluginName] == 2)
                    return;

                visited[pluginName] = 1;

                if (pluginDict.TryGetValue(pluginName, out var pluginNode))
                {
                    foreach (var dependency in pluginNode.Dependencies)
                    {
                        if (!pluginDict.ContainsKey(dependency))
                            throw new DllNotFoundException($"Плагин '{pluginName}' требует отсутствующий плагин: '{dependency}'");
                        
                        Dfs(dependency);
                    }
                    sortedList.Add(pluginNode.PluginType);
                }
                visited[pluginName] = 2;
            }

            foreach (var plugin in plugins)
            {
                Dfs(plugin.Name);
            }
            return sortedList;
        }
    }
}