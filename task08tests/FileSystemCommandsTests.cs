using System;
using System.IO;
using Xunit;
using FileSystemCommands;

namespace task08tests
{
    public class FileSystemCommandsTests
    {
        [Fact]
        public void DirectorySizeCommand_ShouldCalculateSize()
        {
            var testDir = Path.Combine(Path.GetTempPath(), "TestDir_Size_" + Guid.NewGuid());
            Directory.CreateDirectory(testDir);
            
            File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello");
            File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World");

            var command = new DirectorySizeCommand(testDir);
            var originalOut = Console.Out;

            try
            {
                using (var sw = new StringWriter())
                {
                    Console.SetOut(sw);
                    command.Execute();

                    var output = sw.ToString();
                    
                    Assert.Contains("Файлов найдено: 2", output);
                }
            }
            finally
            {
                Console.SetOut(originalOut);
                if (Directory.Exists(testDir))
                {
                    Directory.Delete(testDir, true);
                }
            }
        }

        [Fact]
        public void FindFilesCommand_ShouldFindMatchingFiles()
        {
            var testDir = Path.Combine(Path.GetTempPath(), "TestDir_Find_" + Guid.NewGuid());
            Directory.CreateDirectory(testDir);
            
            File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
            File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");

            var command = new FindFilesCommand(testDir, "*.txt");
            var originalOut = Console.Out;

            try
            {
                using (var sw = new StringWriter())
                {
                    Console.SetOut(sw);

                    command.Execute();

                    var output = sw.ToString();
                    
                    Assert.Contains("Найдено совпадений: 1", output);
                    Assert.Contains("file1.txt", output);
                    Assert.DoesNotContain("file2.log", output);
                }
            }
            finally
            {
                Console.SetOut(originalOut);
                
                if (Directory.Exists(testDir))
                {
                    Directory.Delete(testDir, true);
                }
            }
        }
    }
}