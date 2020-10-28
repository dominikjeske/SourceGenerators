using HomeCenter.Actors.Core;
using HomeCenter.Messages.Queries.Device;
using System.Threading.Tasks;

namespace HomeCenter.SourceGenerators.Tests
{
    [ProxyCodeGenerator]
    public class TestAdapter : Adapter
    {
        protected Task<string> Handle(CapabilitiesQuery message)
        {
            return Task.FromResult("xxx");
        }

        protected Task<string> HandleSupportedState(SupportedStatesQuery message)
        {
            return Task.FromResult("xxx");
        }

        protected Task<string> HandleState(StateQuery message)
        {
            return Task.FromResult("xxx");
        }
    }
}