using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace HomeCenter.SourceGenerators
{
    internal static class ExceptionExtensions
    {
        public static GeneratedSource GenerateErrorSourceCode<T>(this Exception exception, ClassDeclarationSyntax classDeclaration, ISourceGeneratorLogger logger) where T : ISourceGenerator
        {
            var context = $"[{typeof(T).Name} - {classDeclaration.Identifier.Text}]";

            var templateString = ResourceReader.GetResource("ErrorModel.cstemplate");
            templateString = templateString.Replace("//Error", $"#error {context} {exception.Message} | Logfile: {logger.LogPath}");

            return new GeneratedSource(templateString, classDeclaration.Identifier.Text, exception);
        }
    }
}