using HomeCenter.Abstractions;
using HomeCenter.Messages.Queries.Device;
using Microsoft.Extensions.Logging;
using Proto;
using System.Threading.Tasks;

namespace HomeCenter.SourceGenerators.Tests
{
    public class TestAdapterProxy : TestAdapter
    {
        protected async override Task ReceiveAsyncInternal(IContext context)
        {
            if (await HandleSystemMessages(context)) return;

            var msg = FormatMessage(context.Message);
            if (msg is ActorMessage ic)
            {
                ic.Context = context;
            }

            if (msg is CapabilitiesQuery query_0)
            {
                var result = Handle(query_0);
                context.Respond(result);
                return;
            }
            else if (msg is SupportedStatesQuery query_1)
            {
                var result = Handle(query_1);
                context.Respond(result);
                return;
            }
            else if (msg is StateQuery query_2)
            {
                var result = Handle(query_2);
                context.Respond(result);
                return;
            }

            await UnhandledMessage(msg);
        }

        public TestAdapterProxy(IMessageBroker messageBroker, ILogger<TestAdapterProxy> logger)
        {
            Logger = logger;
            MessageBroker = messageBroker;
        }
    }
}