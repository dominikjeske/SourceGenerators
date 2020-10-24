using HomeCenter.Abstractions;
using HomeCenter.Actors.Core;
using Proto;

namespace HomeCenter.SourceGenerators.Tests.TestSources
{
    internal class ActorProxyTestSources : Adapter
    {
        private readonly IRoslynCompilerService _roslynCompilerService;

        public ActorProxyTestSources(IRoslynCompilerService roslynCompilerService)
        {
            _roslynCompilerService = roslynCompilerService;
        }
    }
}