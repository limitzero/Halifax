using System.Reflection;
using Castle.Core.Configuration;
using Castle.MicroKernel.Registration;
using Halifax.Commanding;

namespace Halifax.Configuration.Elements
{
    /// <summary>
    /// Configuration element for registering event handlers.
    /// 
    /// Node Syntax:
    /// <command-handlers>
    ///    <add name="{full namespace of assembly for command handler}" />
    ///    ...
    /// </messages>
    /// </summary>
    public class CommandHandlerElementBuilder : AbstractElementBuilder
    {
        private const string _element = "command-handlers";

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

                Kernel.Register(AllTypes.FromAssembly(asm)
                                    .BasedOn(typeof (CommandConsumer.For<>)).WithService.Base());
            }
        }
    }
}