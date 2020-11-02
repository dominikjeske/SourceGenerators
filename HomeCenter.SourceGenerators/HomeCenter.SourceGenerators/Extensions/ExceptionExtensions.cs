using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;

namespace HomeCenter.SourceGenerators
{
    internal static class ExceptionExtensions
    {
        public static GeneratedSource GenerateErrorSourceCode<T>(this Exception exception, ClassDeclarationSyntax classDeclaration)
        {
            var context = $"[{typeof(T).Name} - {classDeclaration.Identifier.Text}]";

            var logfile = GeneratorExecutionContextExtensions.GetLogFlileLocation<T>();
            var templateString = ResourceReader.GetResource("ErrorModel.cstemplate");
            templateString = templateString.Replace("//Error", $"#error {context} {exception.Message} | Logfile: {logfile}");

            return new GeneratedSource(templateString, classDeclaration.Identifier.Text, exception);
        }
    }
}