using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core.Configuration;
using Halifax.Commanding;
using Halifax.Eventing;
using Halifax.Internals.Serialization;

namespace Halifax.Configuration.Elements
{
    /// <summary>
    /// Configuration element for loading messages into 
    /// the serializer
    /// 
    /// Node Syntax:
    /// <messages>
    ///    <add name="{full namespace of assembly for messages}" />
    ///    ...
    /// </messages>
    /// </summary>
    public class MessagesElementBuilder : AbstractElementBuilder
    {
        private const string _element = "messages";

        public override bool IsMatchFor(string name)
        {
            return _element.Trim() == name.Trim().ToLower();
        }

        public override void Build(IConfiguration configuration)
        {
            var messages = new List<Type>();
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

                //build the serializer for the messages:
                Type[] sourceMessages = (from message in asm.GetTypes()
                                         where message.IsClass
                                               && message.IsAbstract == false
                                         select message).ToArray();

                Type[] publishableMessages = (from message in sourceMessages
                                              where (typeof (DomainEvent).IsAssignableFrom(message) ||
                                                     typeof (Command).IsAssignableFrom(message) ||
                                                     typeof (Enum).IsAssignableFrom(message))
                                              select message).ToArray();


                messages.AddRange(publishableMessages);
            }

            Kernel.Resolve<ISerializationProvider>().Initialize(messages);
        }
    }
}