using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Scriban;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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

            proxyModel.BaseClassName = proxy.Identifier.Text;

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

            var xx = ResourceReader.GetEmbededResourceNames<ActorProxySourceGenerator>();

            var template = Template.Parse("Hello {{name}}!");
            var result = template.Render(new { Name = "World" }); // => "Hello World!" 

            return new GeneratedSource("", proxyModel.ClassName);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new ActorSyntaxReceiver());
        }
    }

    public class ResourceReader
    {
        public static IEnumerable<string> FindEmbededResources<TAssembly>(Func<string, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return
                GetEmbededResourceNames<TAssembly>()
                    .Where(predicate)
                    .Select(name => ReadEmbededResource(typeof(TAssembly), name))
                    .Where(x => !string.IsNullOrEmpty(x));
        }

        public static IEnumerable<string> GetEmbededResourceNames<TAssembly>()
        {
            var assembly = Assembly.GetAssembly(typeof(TAssembly));
            return assembly.GetManifestResourceNames();
        }

        public static string ReadEmbededResource<TAssembly, TNamespace>(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            return ReadEmbededResource(typeof(TAssembly), typeof(TNamespace), name);
        }

        public static string ReadEmbededResource(Type assemblyType, Type namespaceType, string name)
        {
            if (assemblyType == null) throw new ArgumentNullException(nameof(assemblyType));
            if (namespaceType == null) throw new ArgumentNullException(nameof(namespaceType));
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            return ReadEmbededResource(assemblyType, $"{namespaceType.Namespace}.{name}");
        }

        public static string ReadEmbededResource(Type assemblyType, string name)
        {
            if (assemblyType == null) throw new ArgumentNullException(nameof(assemblyType));
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            var assembly = Assembly.GetAssembly(assemblyType);
            using (var resourceStream = assembly.GetManifestResourceStream(name))
            {
                if (resourceStream == null) return null;
                using (var streamReader = new StreamReader(resourceStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }

    internal record GeneratedSource(string SourceCode, string FileName);

    internal record ProxyModel 
    {
        public string ClassName { get; set; }

        public string Namespace { get; set; }

        public string BaseClassName { get; set; }

        public List<string> Usings { get; set; } = new List<string>();
    };
}