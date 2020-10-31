using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace HomeCenter.SourceGenerators
{
    [Generator]
    public class MessageFactoryGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new MessageFactorySyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not MessageFactorySyntaxReceiver actorSyntaxReciver) return;

            foreach (var proxy in actorSyntaxReciver.CandidateProxies)
            {
                var source = GenearteProxy(proxy, context.Compilation);
                context.AddSource(source.FileName, source.SourceCode);
            }
        }

        private GeneratedSource GenearteProxy(ClassDeclarationSyntax proxy, Compilation compilation)
        {
            try
            {
                var factoryModel = GetModel(proxy, compilation);

                var templateString = ResourceReader.GetResource("MessageFactory.scriban");

                var result = TemplateGenerator.Execute(templateString, factoryModel);

                return new GeneratedSource(result, nameof(MessageFactoryGenerator));
            }
            catch (Exception ex)
            {
                return new GeneratedSource(ex.GenerateErrorSourceCode(), proxy.Identifier.Text);
            }
        }

        private MessageFactoryModel GetModel(ClassDeclarationSyntax classSyntax, Compilation compilation)
        {
            var root = classSyntax.GetCompilationUnit();

            var proxyModel = new MessageFactoryModel
            {
                ClassName = classSyntax.GetClassName(),

                ClassModifier = classSyntax.GetClassModifier(),

                Usings = root.GetUsings(),

                Namespace = root.GetNamespace(),
            };

            return proxyModel;
        }
    }
}