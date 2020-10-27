using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace HomeCenter.SourceGenerators
{
    [Generator]
    internal class ActorProxySourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not ActorSyntaxReceiver actorSyntaxReciver) return;
            
            foreach(var proxy in actorSyntaxReciver.CandidateProxies)
            {
                var source = GenearteProxy(proxy, context.Compilation);
                context.AddSource(source.FileName, source.SourceCode);
            }
        }

        private GeneratedSource GenearteProxy(ClassDeclarationSyntax proxy, Compilation compilation)
        {
            var root = proxy.Ancestors().OfType<CompilationUnitSyntax>().FirstOrDefault();

            var proxyModel = new ProxyModel();

            proxyModel.ClassBase = proxy.Identifier.Text;

            proxyModel.ClassName = $"{proxy.Identifier.Text}Proxy";

            proxyModel.Usings = root.ChildNodes()
                                    .OfType<UsingDirectiveSyntax>()
                                    .Select(n => n.Name.ToString())
                                    .ToList();

            proxyModel.Namespace = root.ChildNodes()
                                       .OfType<NamespaceDeclarationSyntax>()
                                       .FirstOrDefault()
                                       .Name
                                       .ToString();

            var templateString = ResourceReader.GetResource("ActorProxy.template");

            var result = TemplateGenerator.Execute(templateString, proxyModel);

            return new GeneratedSource(result, proxyModel.ClassName);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new ActorSyntaxReceiver());
        }
    }

    internal record GeneratedSource(string SourceCode, string FileName);

    internal record ProxyModel 
    {
        public string ClassName { get; set; }

        public string Namespace { get; set; }

        public string ClassBase { get; set; }

        public List<string> Usings { get; set; } = new List<string>();
    };
}