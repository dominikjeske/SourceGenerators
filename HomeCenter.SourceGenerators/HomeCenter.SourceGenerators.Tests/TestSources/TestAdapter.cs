using HomeCenter.Actors.Core;
using HomeCenter.Messages.Queries.Device;
using System.Threading.Tasks;

namespace HomeCenter.SourceGenerators.Tests
{
    [ProxyCodeGenerator]
    public class TestAdapter : Adapter
    {
        protected async Task Handle(CapabilitiesQuery message)
        {
        }

        protected async Task Handle(SupportedStatesQuery message)
        {
        }

        protected async Task Handle(StateQuery message)
        {
        }
    }
}