using System;
using System.IO;
using System.Linq;
using Castle.Core.Configuration;
using Halifax.Bus.Eventing;
using Halifax.Bus.Eventing.Async;
using Halifax.Configuration.Builders;
using Halifax.Exceptions;

namespace Halifax.Configuration.Elements
{
    /// <summary>
    /// Configuration element for configuration of matching messages
    /// to storage locations for async event bus to create messaging 
    /// endpoints.
    /// 
    /// Node Syntax:
    /// <endpoints>
    ///    <add>
    ///            name="{namespace or partial namespace}" 
    ///            uri="{location for delivery}">      
    ///           <concurrency max="{1..10}"  wait="{1...n}" />
    ///           <scheduled interval="{1...n}" />
    ///     </add>
    ///     ...
    ///</endpoints>
    /// </summary>
    public class EndpointElementBuilder : AbstractElementBuilder
    {
        private const string _element = "endpoints";

        public override bool IsMatchFor(string name)
        {
            return _element.Trim() == name.Trim().ToLower();
        }

        public override void Build(IConfiguration configuration)
        {
            // only create the messages subscriptions and 
            // endpoints if the aync event bus has been declared:
            var eventBus = Kernel.Resolve<IStartableEventBus>();
            if (!typeof(AsyncEventBus).IsAssignableFrom(eventBus.GetType())) return;

            string[] files = { };

            if (!string.IsNullOrEmpty(WorkingDirectory))
                files = Directory.GetFiles(WorkingDirectory, "*.dll");
            else
            {
                files = Directory.GetFiles(Environment.CurrentDirectory, "*.dll");
            }

            // build the endpoint for the events:
            var builder = Kernel.Resolve<EventEndpointBuilder>();

            for (int index = 0; index < configuration.Children.Count; index++)
            {
                IConfiguration endpoint = configuration.Children[index];

                string @namespace = endpoint.Attributes["name"];

                string uri = endpoint.Attributes["uri"];


                IConfiguration concurrency = endpoint.Children["concurrency"];
                IConfiguration scheduled = endpoint.Children["scheduled"];


                FileInfo file = (from f in files
                                 let cf = new FileInfo(f)
                                 where cf.Name.Replace(".dll", string.Empty).
                                     ToLower().Trim().StartsWith(@namespace.ToLower().Trim())
                                 select cf).FirstOrDefault();

                if (concurrency != null && scheduled != null)
                {
                    string message = string.Format(
                            "The endpoint for '{0}' can not have both a concurrency and a scheduled configuration for inspecting the location '{1}'.",
                            @namespace, uri);
                    throw new Exception(message);
                }

                if (concurrency != null)
                {
                    int maxWorkers = 1;

                    try
                    {
                        maxWorkers = concurrency.Attributes["max"] != string.Empty
                                          ?
                                              Int32.Parse(concurrency.Attributes["max"])
                                          : 1;
                    }
                    catch
                    {
                    }

                    int wait = 1;

                    try
                    {
                        wait = concurrency.Attributes["wait"] != string.Empty
                                        ?
                                            Int32.Parse(concurrency.Attributes["wait"])
                                        : 1;
                    }
                    catch
                    {
                    }

                    if (maxWorkers > 10)
                        throw new ConfigurationElementExceededLimitException("concurrency", "max", 10.ToString());

                    builder.Build(uri,
                              Messages.Configure.FromAssemblyFile(file.FullName),
                              Scheduling.Configure.WithMulitpleConsumers(maxWorkers, wait));

                }
                else if (scheduled != null)
                {
                    int interval = 1;

                    try
                    {
                        interval = scheduled.Attributes["interval"] != string.Empty
                                          ?
                                              Int32.Parse(scheduled.Attributes["interval"])
                                          : 1;
                    }
                    catch
                    {
                    }

                    builder.Build(uri,
                             Messages.Configure.FromAssemblyFile(file.FullName),
                             Scheduling.Configure.WithPollingEvery(interval));
                }
                else
                {
                    // default: scheduled at every second
                    builder.Build(uri,
                                Messages.Configure.FromAssemblyFile(file.FullName),
                                Scheduling.Configure.WithPollingEvery(1));
                }

            }
        }
    }
}