using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Halifax.Storage.Internals.Reflection;
using Halifax.Storage.Internals.Serialization;

namespace Halifax.Configuration.Bootstrapper
{
    /// <summary>
    /// Bootstrapper to auto-register the aggregate roots
    /// </summary>
    public class AggregateRootsRegistrationBootstrapper : AbstractBootstrapper
    {
        public override void Configure()
        {
            string[] files = {};

            if (!string.IsNullOrEmpty(WorkingDirectory))
                files = Directory.GetFiles(WorkingDirectory, "*.dll");
            else
            {
                files = Directory.GetFiles(Environment.CurrentDirectory, "*.dll");
            }

            foreach (string file in files)
            {
                Assembly asm = Assembly.LoadFile(file);

                Type[] items = Kernel.Resolve<IReflection>()
                    .FindConcreteTypesImplementingInterface(typeof (AbstractAggregateRoot), asm);

                if (items.Count() == 0) continue;

                foreach (Type item in items)
                {
                    Kernel.AddComponent(item.Name, item);
                    Kernel.Resolve<ISerializationProvider>().AddType(item);
                }
            }
        }
    }
}