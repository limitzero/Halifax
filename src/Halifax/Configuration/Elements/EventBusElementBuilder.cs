using Castle.Core.Configuration;
using Castle.MicroKernel.Registration;
using Halifax.Bus.Eventing;
using Halifax.Bus.Eventing.Async;

namespace Halifax.Configuration.Elements
{
    /// <summary>
    /// Builder for the element 
    /// <event-bus isAsync="false|true" />
    /// </summary>
    public class EventBusElementBuilder : AbstractElementBuilder
    {
        private const string _element = "event-bus";

        public override bool IsMatchFor(string name)
        {
            return _element.Trim() == name.Trim().ToLower();
        }

        public override void Build(IConfiguration configuration)
        {
            string isAsync = configuration.Attributes["isAsync"] ?? "false";

            if (isAsync.Trim().ToLower() == "true")
                Kernel.Register(Component.For<IStartableEventBus>()
                                    .ImplementedBy<AsyncEventBus>());
            else
            {
                Kernel.Register(Component.For<IStartableEventBus>()
                                    .ImplementedBy<InProcessEventBus>());
            }
        }
    }
}