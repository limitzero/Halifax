using System.Reflection;
using Castle.Core.Configuration;
using Castle.MicroKernel.Registration;
using Halifax.Eventing;

namespace Halifax.Configuration.Elements
{
    /// <summary>
    /// Configuration element for registering event handlers.
    /// 
    /// Node Syntax:
    /// <event-handlers>
    ///    <add name="{full namespace of assembly for event handler}" />
    ///    ...
    /// </messages>
    /// </summary>
    public class EventHandlerElementBuilder : AbstractElementBuilder
    {
        private const string _element = "event-handlers";

        public override bool IsMatchFor(string name)
        {
            return _element.Trim() == name.Trim().ToLower();
        }

        public override void Build(IConfiguration configuration)
        {
            Assembly asm = null;

            for (int index = 0; index < configuration.Children.Count; index++)
            {
                IConfiguration msg = configuration.Children[index];
                string location = msg.Attributes["location"];
                string @namespace = msg.Attributes["name"];

                if (!string.IsNullOrEmpty(location))
                    asm = Assembly.LoadFile(location);

                if (string.IsNullOrEmpty(location))
                    if (!string.IsNullOrEmpty(@namespace))
                        asm = Assembly.Load(@namespace);

                if (!string.IsNullOrEmpty(@namespace))
                    asm = Assembly.Load(@namespace);

                Kernel.Register(AllTypes.FromAssembly(asm)
                                    .BasedOn(typeof (EventConsumer.For<>)).WithService.Base());
            }
        }
    }
}