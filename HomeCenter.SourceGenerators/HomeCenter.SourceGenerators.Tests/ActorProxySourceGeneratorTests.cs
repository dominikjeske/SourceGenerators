using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace HomeCenter.SourceGenerators.Tests
{
    public class ActorProxySourceGeneratorTests
    {
        [Fact]
        public async Task SimpleGeneratorTest()
        {
            var userSource = await File.ReadAllTextAsync(@"..\..\..\TestSources\TestAdapter.cs");

            var result = GeneratorRunner.Run(userSource, new ActorProxySourceGenerator());

            Assert.Empty(result.Diagnostics);
            Assert.Empty(result.Compilation.GetDiagnostics());
        }
    }
}