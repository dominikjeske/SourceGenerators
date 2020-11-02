using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace HomeCenter.SourceGenerators
{
    internal static class GeneratorExecutionContextExtensions
    {
        private const int LineSurfixLenght = 20;
        private const int LineLenght = 100;

        public static string GetLogFlileLocation<T>()
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "obj");
            var logfile = Path.Combine(directory, $"{typeof(T).Name}.log");
            return logfile;
        }

        [Conditional("DEBUG")]
        public static void TryLogSource<T>(this GeneratorExecutionContext executionContext, ClassDeclarationSyntax classDeclaration, GeneratedSource generatedSource)
            where T : ISourceGenerator
        {

//#if DEBUG
            var logfile = GetLogFlileLocation<T>();
            var sb = new StringBuilder();

            var context = $" [{typeof(T).Name} | {classDeclaration.Identifier.Text} | {DateTime.Now:g}] ";
            var header = new string('-', LineSurfixLenght) + context + new string('-', LineLenght - LineSurfixLenght - context.Length);
            sb.AppendLine("");
            sb.AppendLine(header);
            sb.AppendLine("");

            if(generatedSource.Exception != null)
            {
                sb.AppendLine(generatedSource.Exception.ToString());
            }
            else
            {
                sb.AppendLine(generatedSource.SourceCode);
            }

            sb.AppendLine("");
                
            File.AppendAllText(logfile, sb.ToString());
//#endif
        }
    }
}