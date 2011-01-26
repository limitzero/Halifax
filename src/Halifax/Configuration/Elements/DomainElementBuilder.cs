using System.Reflection;
using Castle.Core.Configuration;
using Castle.MicroKernel.Registration;

namespace Halifax.Configuration.Elements
{
    public class DomainElementBuilder : AbstractElementBuilder
    {
        private const string _element = "domains";

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
                                    .BasedOn(typeof (AbstractAggregateRoot)));
            }
        }
    }
}