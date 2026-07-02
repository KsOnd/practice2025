using System;
using System.IO;
using CommandLib;

namespace FileSystemCommands
{
    public class FindFilesCommand : ICommand
    {
        private readonly string _dirPath;
        private readonly string _mask;

        public FindFilesCommand(string dirPath, string mask)
        {
            _dirPath = dirPath ?? throw new ArgumentNullException(nameof(dirPath));
            _mask = mask ?? throw new ArgumentNullException(nameof(mask));
        }

        public void Execute()
        {
            Console.WriteLine($"\nПоиск файлов в {_dirPath} по маске: \"{_mask}\"");

            if (!Directory.Exists(_dirPath))
            {
                Console.WriteLine("Указанный каталог не существует.");
                return;
            }

            try
            {
                string[] matchedFiles = Directory.GetFiles(_dirPath, _mask);
                Console.WriteLine($"Найдено совпадений: {matchedFiles.Length}");
                
                foreach (string file in matchedFiles)
                {
                    Console.WriteLine($" -> {Path.GetFileName(file)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при поиске файлов: {ex.Message}");
            }
        }
    }
}