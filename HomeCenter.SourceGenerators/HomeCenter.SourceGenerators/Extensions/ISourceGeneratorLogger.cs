using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HomeCenter.SourceGenerators
{
    internal interface ISourceGeneratorLogger
    {
        string LogPath { get; }
        void LogGeneratedSource(ClassDeclarationSyntax classDeclaration, GeneratedSource generatedSource);
        void LogMessage(string message);
    }
}