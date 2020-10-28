using HomeCenter.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeCenter.SourceGenerators
{
    [Generator]
    public class ActorProxySourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new ActorSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not ActorSyntaxReceiver actorSyntaxReciver) return;

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
                var proxyModel = GetModel(proxy, compilation);

                var templateString = ResourceReader.GetResource("ActorProxy.scriban");

                var result = TemplateGenerator.Execute(templateString, proxyModel);

                return new GeneratedSource(result, proxyModel.ClassName);
            }
            catch (Exception ex)
            {
                return new GeneratedSource(ex.GenerateErrorSourceCode(), proxy.Identifier.Text);
            }
        }

        private ProxyModel GetModel(ClassDeclarationSyntax classSyntax, Compilation compilation)
        {
            var root = classSyntax.Ancestors().OfType<CompilationUnitSyntax>().FirstOrDefault();

            var classSemanticModel = compilation.GetSemanticModel(classSyntax.SyntaxTree);

            var proxyModel = new ProxyModel
            {
                ClassBase = GetClassName(classSyntax),

                ClassName = $"{GetClassName(classSyntax)}Proxy",

                Usings = GetUsings(root),

                Namespace = GetNamespace(root),

                Commands = GetMethodWithParameter(classSyntax, classSemanticModel, nameof(Command)),

                Queries = GetMethodWithParameter(classSyntax, classSemanticModel, nameof(Query)),

                Events = GetMethodWithParameter(classSyntax, classSemanticModel, nameof(Event))
            };

            return proxyModel;
        }

        private List<MethodDescription> GetMethodWithParameter(ClassDeclarationSyntax classSyntax, SemanticModel model, string parameterType, string attributeType = null)
        {
            var filter = classSyntax.DescendantNodes()
                                    .OfType<MethodDeclarationSyntax>()
                                    .Where(m => m.ParameterList.Parameters.Count == 1 && !m.Modifiers.Any(x => x.ValueText == "private"));

            if (attributeType != null)
            {
                filter = filter.Where(m => m.AttributeLists.Any(a => a.Attributes.Any(x => x.Name.ToString() == attributeType)));
            }

            var result = filter.Select(method => new
            {
                Name = method.Identifier.ValueText,
                ReturnType = model.GetTypeInfo(method.ReturnType).Type as INamedTypeSymbol,
                Parameter = (IParameterSymbol)model.GetDeclaredSymbol(method.ParameterList.Parameters.FirstOrDefault())
            }).Where(x => x.Parameter.Type.BaseType?.Name == parameterType || x.Parameter.Type.BaseType?.BaseType?.Name == parameterType || x.Parameter.Type.Name == parameterType)
            .Select(c => new MethodDescription
            {
                MethodName = c.Name,
                ParameterType = c.Parameter.Type.Name,
                ReturnType = c.ReturnType.Name,
                ReturnTypeGenericArgument = c.ReturnType.TypeArguments.FirstOrDefault()?.Name
                // TODO write recursive base type check
            }).ToList();

            //if (_context.Compilation != null && model.GetDeclaredSymbol(classSyntax)?.BaseType?.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is ClassDeclarationSyntax subClassSyntax)
            //{
            //    var semanticModel = _context.Compilation.GetSemanticModel(subClassSyntax.SyntaxTree);

            //    // add usings from base class
            //    _usingSyntax.AddRange(subClassSyntax.SyntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>());

            //    var sub = GetMethodListInner(subClassSyntax, semanticModel, parameterType, attributeType);
            //    result.AddRange(sub);
            //}

            return result;
        }

        private static string GetClassName(ClassDeclarationSyntax proxy) => proxy.Identifier.Text;

        private static string GetNamespace(CompilationUnitSyntax root)
        {
            return root.ChildNodes()
                       .OfType<NamespaceDeclarationSyntax>()
                       .FirstOrDefault()
                       .Name
                       .ToString();
        }

        private static List<string> GetUsings(CompilationUnitSyntax root)
        {
            return root.ChildNodes()
                       .OfType<UsingDirectiveSyntax>()
                       .Select(n => n.Name.ToString())
                       .ToList();
        }
    }
}