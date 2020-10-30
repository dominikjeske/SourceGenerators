using HomeCenter.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
            var classSymbol = classSemanticModel.GetDeclaredSymbol(classSyntax);

            var proxyModel = new ProxyModel
            {
                ClassBase = GetClassName(classSyntax),

                ClassName = $"{GetClassName(classSyntax)}{ProxyAttribute.Name}",

                ClassModifier = GetClassModifier(classSyntax),

                Usings = GetUsings(root),

                Namespace = GetNamespace(root),

                Commands = GetMethodWithParameter(classSyntax, classSemanticModel, nameof(Command)),

                Queries = GetMethodWithParameter(classSyntax, classSemanticModel, nameof(Query)),

                Events = GetMethodWithParameter(classSyntax, classSemanticModel, nameof(Event)),

                ConstructorParameters = GetConstructor(classSymbol),

                InjectedProperties = GetInjectedProperties(classSymbol)
            };

            return proxyModel;
        }

        private List<ParameterDescriptor> GetConstructor(INamedTypeSymbol classSymbol)
        {
            IMethodSymbol baseConstructor = classSymbol.Constructors.OrderByDescending(p => p.Parameters.Length).FirstOrDefault();

            var parList = baseConstructor.Parameters.Select(par => new ParameterDescriptor()
            {
                Name = par.Name,
                Type = par.Type.ToString()
            }).ToList();

            return parList;
        }

        private List<PropertyAssignDescriptor> GetInjectedProperties(INamedTypeSymbol classSymbol)
        {
 
            var dependencyProperties = classSymbol.GetAllMembers()
                                                  .Where(x => x.Kind == SymbolKind.Property && x.GetAttributes().Any(a => a.AttributeClass.Name == nameof(DIAttribute)))
                                                  .OfType<IPropertySymbol>()
                                                  .Select(par => new PropertyAssignDescriptor()
                                                  {
                                                      Destination = par.Name,
                                                      Type = par.Type.ToString(),
                                                      Source = par.Name.ToCamelCase()

                                                  }).ToList();
            return dependencyProperties;
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

            return result;
        }

        private static string GetClassName(ClassDeclarationSyntax proxy) => proxy.Identifier.Text;

        private static string GetClassModifier(ClassDeclarationSyntax proxy) => proxy.Modifiers.ToFullString();

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