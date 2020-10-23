[ProxyClass]
    public class ComponentProxy : Component
    {
        protected async override Task ReceiveAsyncInternal(Proto.IContext context)
        {
            if (await HandleSystemMessages(context))
                return;
            var msg = FormatMessage(context.Message);
            if (msg is HomeCenter.Model.Messages.ActorMessage ic)
            {
                ic.Context = context;
            }

            ;
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

            if (msg is SystemStartedEvent event_0)
            {
                await OnSystemStarted(event_0).ConfigureAwait(false);
                return;
            }
            else if (msg is Event event_1)
            {
                await Handle(event_1).ConfigureAwait(false);
                return;
            }
            else if (msg is SystemStartedEvent event_2)
            {
                await OnSystemStarted(event_2).ConfigureAwait(false);
                return;
            }

            await UnhandledMessage(msg);
        }

        public ComponentProxy(IMessageBroker messageBroker, ILogger<ComponentProxy> logger)
        {
            Logger = logger;
            MessageBroker = messageBroker;
        }
    }