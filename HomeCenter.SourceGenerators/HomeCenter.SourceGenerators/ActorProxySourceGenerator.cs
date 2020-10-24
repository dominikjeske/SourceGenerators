using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace HomeCenter.SourceGenerators
{
    [Generator]
    internal class ActorProxySourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            context.AddSource(nameof(ActorProxySourceGenerator), @"namespace HomeCenter.SourceGenerators.Tests.TestSources
            {
                public class TestGenerator
                    {
                    }
                }");
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new ActorSyntaxReceiver());
        }
    }

    class ActorSyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> CandidateFields { get; } = new List<ClassDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax && classDeclarationSyntax.AttributeLists.Count > 0)
            {
                CandidateFields.Add(classDeclarationSyntax);
            }
        }
    }
}