using System;
using System.IO;
using CommandLib;

namespace FileSystemCommands
{
    public class DirectorySizeCommand : ICommand
    {
        private readonly string _dirPath;

        public DirectorySizeCommand(string dirPath)
        {
            _dirPath = dirPath ?? throw new ArgumentNullException(nameof(dirPath));
        }

        public void Execute()
        {
            Console.WriteLine($"\nВычисление размера для: {_dirPath}");

            if (!Directory.Exists(_dirPath))
            {
                Console.WriteLine("Указанный каталог не существует.");
                return;
            }

            try
            {
                long totalSize = 0;
                string[] files = Directory.GetFiles(_dirPath, "*", SearchOption.AllDirectories);
                
                foreach (string file in files)
                {
                    totalSize += new FileInfo(file).Length;
                }

                Console.WriteLine($"Размер: {totalSize / (1024.0 * 1024.0)} МБ. Файлов найдено: {files.Length}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при подсчете размера: {ex.Message}");
            }
        }
    }
}