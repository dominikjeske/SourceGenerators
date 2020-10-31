    public partial class MessageGenerator
    {
        public async System.Threading.Tasks.Task<HomeCenter.Model.Messages.Events.Event> PublishEvent(ActorMessage source, ActorMessage destination, HomeCenter.Model.Core.IMessageBroker messageBroker, HomeCenter.Broker.RoutingFilter routingFilter)
        {
            if (destination.Type == "DipswitchEvent")
            {
                var @event = new HomeCenter.Model.Messages.Events.Device.DipswitchEvent();
                @event.SetProperties(source);
                @event.SetProperties(destination);
                await messageBroker.Publish(@event, routingFilter);
                return @event;
            }
            else if (destination.Type == "InfraredEvent")
            {
                var @event = new HomeCenter.Model.Messages.Events.Device.InfraredEvent();
                @event.SetProperties(source);
                @event.SetProperties(destination);
                await messageBroker.Publish(@event, routingFilter);
                return @event;
            }
            else if (destination.Type == "MotionEvent")
            {
                var @event = new HomeCenter.Model.Messages.Events.Device.MotionEvent();
                @event.SetProperties(source);
                @event.SetProperties(destination);
                await messageBroker.Publish(@event, routingFilter);
                return @event;
            }
            else if (destination.Type == "PinValueChangedEvent")
            {
                var @event = new HomeCenter.Model.Messages.Events.Device.PinValueChangedEvent();
                @event.SetProperties(source);
                @event.SetProperties(destination);
                await messageBroker.Publish(@event, routingFilter);
                return @event;
            }
            else if (destination.Type == "PowerStateChangeEvent")
            {
                var @event = new HomeCenter.Model.Messages.Events.Device.PowerStateChangeEvent();
                @event.SetProperties(source);
                @event.SetProperties(destination);
                await messageBroker.Publish(@event, routingFilter);
                return @event;
            }
            else if (destination.Type == "PropertyChangedEvent")
            {
                var @event = new HomeCenter.Model.Messages.Events.Device.PropertyChangedEvent();
                @event.SetProperties(source);
                @event.SetProperties(destination);
                await messageBroker.Publish(@event, routingFilter);
                return @event;
            }
            else if (destination.Type == "SerialResultEvent")
            {
                var @event = new HomeCenter.Model.Messages.Events.Device.SerialResultEvent();
                @event.SetProperties(source);
                @event.SetProperties(destination);
                await messageBroker.Publish(@event, routingFilter);
                return @event;
            }
            else if (destination.Type == "ComponentStartedEvent")
            {
                var @event = new HomeCenter.Model.Messages.Events.Service.ComponentStartedEvent();
                @event.SetProperties(source);
                @event.SetProperties(destination);
                await messageBroker.Publish(@event, routingFilter);
                return @event;
            }
            else if (destination.Type == "SystemStartedEvent")
            {
                var @event = new HomeCenter.Model.Messages.Events.Service.SystemStartedEvent();
                @event.SetProperties(source);
                @event.SetProperties(destination);
                await messageBroker.Publish(@event, routingFilter);
                return @event;
            }

            {
                var ev = new HomeCenter.Model.Messages.Events.Event();
                ev.SetProperties(source);
                ev.SetProperties(destination);
                await messageBroker.Publish(ev, routingFilter);
                return ev;
            }
        }

        public HomeCenter.Model.Messages.Commands.Command CreateCommand(string message)
        {
            if (message == "AdjustPowerLevelCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Device.AdjustPowerLevelCommand();
            }
            else if (message == "CalibrateCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Device.CalibrateCommand();
            }
            else if (message == "InputSetCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Device.InputSetCommand();
            }
            else if (message == "ModeSetCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Device.ModeSetCommand();
            }
            else if (message == "MuteCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Device.MuteCommand();
            }
            else if (message == "PlayCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Device.PlayCommand();
            }
            else if (message == "RefreshCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Device.RefreshCommand();
            }
            else if (message == "RefreshLightCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Device.RefreshLightCommand();
            }
            else if (message == "SetPowerLevelCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Device.SetPowerLevelCommand();
            }
            else if (message == "StopCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Device.StopCommand();
            }
            else if (message == "SwitchPowerStateCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Device.SwitchPowerStateCommand();
            }
            else if (message == "TurnOffCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Device.TurnOffCommand();
            }
            else if (message == "TurnOnCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Device.TurnOnCommand();
            }
            else if (message == "UnmuteCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Device.UnmuteCommand();
            }
            else if (message == "VolumeDownCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Device.VolumeDownCommand();
            }
            else if (message == "VolumeSetCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Device.VolumeSetCommand();
            }
            else if (message == "VolumeUpCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Device.VolumeUpCommand();
            }
            else if (message == "SendCodeCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Device.SendCodeCommand();
            }
            else if (message == "HttpCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Service.HttpCommand();
            }
            else if (message == "I2cCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Service.I2cCommand();
            }
            else if (message == "StartSystemCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Service.StartSystemCommand();
            }
            else if (message == "TcpCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Service.TcpCommand();
            }
            else if (message == "UdpCommand")
            {
                return new HomeCenter.Model.Messages.Commands.Service.UdpCommand();
            }
            else if (message == "RegisterPinChangedCommand")
            {
                return new HomeCenter.Model.Messages.Queries.Service.RegisterPinChangedCommand();
            }
            else if (message == "RegisterSerialCommand")
            {
                return new HomeCenter.Model.Messages.Queries.Service.RegisterSerialCommand();
            }

            return new HomeCenter.Model.Messages.Commands.Command();
        }
    }