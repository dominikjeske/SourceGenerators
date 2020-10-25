using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace HomeCenter.SourceGenerators
{
    internal class ActorSyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> CandidateProxies { get; } = new List<ClassDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if
            (
                syntaxNode is ClassDeclarationSyntax classSyntax && 
                classSyntax.AttributeLists.Count > 0 &&
                classSyntax.AttributeLists.SelectMany(al => al.Attributes
                                          .Where(a => (a.Name as IdentifierNameSyntax).Identifier.Text == "ProxyCodeGenerator"))
                                          .Any()
            )
            {

                CandidateProxies.Add(classSyntax);
            }
        }
    }
}