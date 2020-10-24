using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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

                Assert.Empty(result.Diagnostics);
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
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var compilation = CSharpCompilation.Create(nameof(GeneratorRunner), syntaxTrees, references, options);

            return compilation;
        }
        public static GeneratorResult Run(string sourceCode, ISourceGenerator generators)
        {
            Compilation compilation = CreateCompilation(sourceCode);

            var driver = CSharpGeneratorDriver.Create(ImmutableArray.Create(generators),
                                                      ImmutableArray<AdditionalText>.Empty,
                                                      (CSharpParseOptions)compilation.SyntaxTrees.First().Options,
                                                      null);
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);


            var generatedCode = GetGeneratedCode(generators, outputCompilation);

            return new GeneratorResult(outputCompilation, diagnostics, generatedCode);
        }

        private static string GetGeneratedCode(ISourceGenerator generators, Compilation outputCompilation)
        {
            return outputCompilation.SyntaxTrees.FirstOrDefault(file => file.FilePath.IndexOf(generators.GetType().Name) > -1)?.ToString();
        }
    }

    public static class StringExtensions
    {
        public static string TrimWhiteSpaces(this string text)
        {
            if (text == null)
            {
                return text;
            }

            return Regex.Replace(text, @"\s+", string.Empty);
        }
    }

    internal record GeneratorResult(Compilation Compilation, ImmutableArray<Diagnostic> Diagnostics, string generatedCode);
}