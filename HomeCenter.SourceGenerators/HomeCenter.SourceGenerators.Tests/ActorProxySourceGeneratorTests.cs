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
            var userSource = await File.ReadAllTextAsync(@"..\..\..\TestInputs\TestAdapter.cs");
            var expectedResult = await File.ReadAllTextAsync(@"..\..\..\TestOutputs\TestAdapterProxy.cs");

            var result = GeneratorRunner.Run(userSource, new ActorProxySourceGenerator());

            expectedResult.AssertSourceCodesEquals(result.generatedCode);
        }
    }

}