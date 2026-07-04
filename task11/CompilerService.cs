using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace task11
{
    public static class CompilerService
    {
        public static ICalculator CompileCalculator(string sourceCode)
        {
            if (string.IsNullOrWhiteSpace(sourceCode))
                throw new ArgumentException("Исходный код не может быть пустым.", nameof(sourceCode));

            string modifiedCode = sourceCode.Replace("public class Calculator", "public class Calculator : task11.ICalculator");

            string fullCode = $@"using System;
                                namespace DynamicCompilation
                                {{
                                    {modifiedCode}
                                }}";

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(fullCode);

            var references = new List<MetadataReference>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location))
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }

            var compilation = CSharpCompilation.Create(
                $"DynamicCalculator_{Guid.NewGuid()}",
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    string errors = string.Join(Environment.NewLine, result.Diagnostics);
                    throw new InvalidOperationException($"Ошибка динамической компиляции:{Environment.NewLine}{errors}");
                }

                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);

                Type calculatorType = assembly.GetType("DynamicCompilation.Calculator");
                if (calculatorType == null)
                    throw new TypeLoadException("Не удалось обнаружить класс Calculator в скомпилированной сборке.");

                object instance = Activator.CreateInstance(calculatorType);

                return (ICalculator)instance;
            }
        }
    }
}