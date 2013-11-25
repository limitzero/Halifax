using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using Halifax.Commands;
using Halifax.Configuration.Impl.Serialization;
using Halifax.Configuration.Impl.Serialization.Impl;
using Halifax.SignalR.Tests.given;
using Machine.Specifications;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Halifax.Configuration;
using Moq;
using Command = Microsoft.AspNet.SignalR.Messaging.Command;
using It = Machine.Specifications.It;

namespace Halifax.SignalR.Tests
{
	public class TestableHailfaxHub : HalifaxHub
	{
		public TestableHailfaxHub(IConfiguration configuration) : 
			base(configuration)
		{
			const string connectionId = "1234";
			const string hubName = "halifax";
			var mockConnection = new Mock<IConnection>();
			var mockUser = new Mock<IPrincipal>();
			//var mockCookies = new Mock<IRequestCookieCollection>();
			/*
			var mockRequest = new Mock<IRequest>();
			mockRequest.Setup(r => r.User).Returns(mockUser.Object);
			mockRequest.Setup(r => r.Cookies).Returns(mockCookies.Object);

			Clients = new ClientAgent(mockConnection.Object, hubName);
			Context = new HubCallerContext(mockRequest.Object, connectionId);

			var trackingDictionary = new TrackingDictionary();
			Caller = new StatefulSignalAgent(mockConnection.Object, connectionId, hubName, trackingDictionary);  
			 */ 
		}

	}

	[Subject("dispatching commands from client to hub")]
	public class when_a_command_is_dispatched_to_hub 
	{

	}

	[Subject("dispatching native json payload to infrastructure")]
	public class when_a_javascript_JSON_payload_is_dispatched_to_infrastructure : IDisposable
	{
		private static IConfiguration configuration;
		private  readonly static string payload = "{firstName: 'joe', lastName: 'smith'}";

		public when_a_javascript_JSON_payload_is_dispatched_to_infrastructure()
		{
			this.BuildConfiguration();
		}

		It will_deconstruct_the_json_payload_into_a_dictionary = () =>
		{
			var nameValuePairs = DeconstructJSONToDictionary(payload);
			nameValuePairs.Count.ShouldEqual(2);
		};

		It will_create_the_dot_NET_corresponding_object_will_all_properties_hydrated_from_type_name = () =>
		{
			var type = string.Format("{0},{1}",
									 typeof(RegisterCommand).FullName, typeof(RegisterCommand).Assembly.GetName().Name);

			var nameValuePairs = DeconstructJSONToDictionary(payload);

			var jsonPayload = ReMapJavascriptPropertyNotationToObjectPropertyNotation(type, payload, nameValuePairs);

			var serializer = configuration.CurrentContainer()
				.Resolve<ISerializationProvider>();

			// issue with serializer here...need  native method!!!
			string customPayload = ((IJSONSerializationProvider)serializer).SerializeNative(jsonPayload);

			var command = serializer.Deserialize<RegisterCommand>(customPayload);

			command.ShouldNotBeNull();
			command.ShouldBeOfType<RegisterCommand>();
			command.FirstName.ShouldBeEqualIgnoringCase("joe");
			command.LastName.ShouldBeEqualIgnoringCase("smith");
		};

		private static Dictionary<string,string> DeconstructJSONToDictionary(string json)
		{
			Dictionary<string, string> contents = new Dictionary<string, string>();

			string message = json.Replace("{", string.Empty).Replace("}", string.Empty);

			string[] valuePairs = message.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);

			if (valuePairs == null || valuePairs.Length == 0) return contents;

			string[] splitOn = new string[] {":"};

			foreach (var valuePair in valuePairs)
			{
				string name = valuePair.Split(splitOn, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
				string value =  valuePair.Split(splitOn, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
				contents.Add(name, value);
			}

			return contents;
		}

		private static JSONPayload ReMapJavascriptPropertyNotationToObjectPropertyNotation(string type, string json, Dictionary<string, string> map)
		{
			string remappedJSON = json;
			var instanceType = GetTypeFromAssembly(type);
			var properties = instanceType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
		
			foreach (var property in properties)
			{
				foreach (var kvp in map)
				{
					if(property.Name.ToLower().Equals(kvp.Key.ToLower()))
					{
						remappedJSON = remappedJSON.Replace(kvp.Key, property.Name);
					}
				}
			}

			return new JSONPayload {Payload = remappedJSON, Type = type};
		}

		private static Type GetTypeFromAssembly(string type)
		{
			var parts = type.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
			Array.Reverse(parts);

			Assembly asm = Assembly.Load(parts[0]);

			var selectedType = asm.GetTypes()
				.Where(t => t.FullName.Equals(parts[1]))
				.FirstOrDefault();

			return selectedType;
		}

		private void BuildConfiguration()
		{
			configuration = new Halifax.Configuration.Impl.Configuration();

			// build the volatile version of the elements for testing:
			configuration
				.Container(c => c.UsingCastleWindsor())
				.Eventing(ev => ev.Synchronous())
				.EventStore(es => es.UsingInMemoryStorage())
				.Serialization(s => s.UsingJSON())
				.ReadModel(rm => rm.UsingInMemoryRepository())
				//.Web(web => web.ConfigureAsSinglePageApplication().UsingSignalR())
				.Configure(this.GetType().Assembly);
		}

		public void Dispose()
		{
			if(configuration != null)
			{
				configuration.Dispose();
			}
			configuration = null;
		}
	}

	public interface IClientMessagingPipeline
	{
		/// <summary>
		/// This will conduct the process of handling the message from 
		/// the client and applying the neccessary parts in delegating 
		/// the message to be handled and the corresponding response.
		/// </summary>
		/// <param name="type">>.NET fully qualified type name of the command to be executed</param>
		/// <param name="message">JSON representation of the command message as sent from client</param>
		/// <returns></returns>
		CommandResult Execute(string type, string message);
	}

	public class RegisterCommand : Command
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
	}
}
