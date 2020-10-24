using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace HomeCenter.SourceGenerators.Tests
{
    public class ActorProxySourceGeneratorTests
    {
        public class GeneratorTests
        {
            [Fact]
            public async Task SimpleGeneratorTest()
            {
                var userSource = await File.ReadAllTextAsync(@"..\..\..\TestSources\ActorProxyTestSources.cs");

                var result = GeneratorRunner.Run(userSource, new ActorProxySourceGenerator());

                Assert.Empty(result.diagnostics);
                Assert.Empty(result.Compilation.GetDiagnostics());
            }
        }
    }

    internal static class GeneratorRunner
    {
        private static Compilation CreateCompilation(string source)
        {
            var syntaxTrees = new[] 
            { 
                CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview)) 
            };
            var references = new[] 
            { 
                MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) 
            };
            var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication);
            var compilation = CSharpCompilation.Create(nameof(GeneratorRunner), syntaxTrees, references, options);

            return compilation;
        }
        public static GeneratorResult Run(string sourceCode, params ISourceGenerator[] generators)
        {
            Compilation compilation = CreateCompilation(sourceCode);

            var driver = CSharpGeneratorDriver.Create(ImmutableArray.Create(generators), 
                                                      ImmutableArray<AdditionalText>.Empty, 
                                                      (CSharpParseOptions)compilation.SyntaxTrees.First().Options, 
                                                      null);
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

            return new GeneratorResult(outputCompilation, diagnostics);
        }
    }

    internal record GeneratorResult(Compilation Compilation, ImmutableArray<Diagnostic> diagnostics);
}