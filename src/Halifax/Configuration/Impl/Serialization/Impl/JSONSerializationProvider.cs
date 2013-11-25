using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Halifax.Configuration.Impl.Serialization.Impl
{
	/// <summary>
	/// Class created internall to avoid annoymous JSON types
	/// on deserialization.
	/// </summary>
	public class JSONPayload
	{
		public string Type { get; set; }
		public string Payload { get; set; }

		public string GetRawPayloadAssembly()
		{
			return this.Type.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries)[1];
		}

		public string GetRawPayloadType()
		{
			return this.Type.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0];
		}

		public object GetPayloadInstance()
		{
			Assembly asm = Assembly.Load(GetRawPayloadAssembly());
			return asm.CreateInstance(this.GetRawPayloadType());
		}
	}

	public interface IJSONSerializationProvider
	{
		string SerializeNative(object message);
	}

	public class JSONSerializationProvider : ISerializationProvider, IJSONSerializationProvider
	{
		private readonly JsonSerializerSettings settings;

		public JSONSerializationProvider()
		{
			settings = new JsonSerializerSettings
			           	{
			           		DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
			           	};
		}

		public void Initialize(IEnumerable<Type> types)
		{		
		}

		public object Deserialize(string type, string contents)
		{
			var payload = new JSONPayload
			              	{
			              		Type = type,
			              		Payload = contents
			              	};

			var result = DeserializeInternal(payload);

			return result;
		}

		public object Deserialize(string instance)
		{
			var payload = JsonConvert.DeserializeObject<JSONPayload>(instance, settings);
			return DeserializeInternal(payload);
		}

		public object Deserialize(Stream stream)
		{
			using(var reader = new StreamReader(stream))
			{
				var message = reader.ReadToEnd();
				return this.Deserialize(message);
			}
		}

		public object Deserialize(byte[] bytes)
		{
			var message = ASCIIEncoding.ASCII.GetString(bytes);
			return this.Deserialize(message);
		}

		public TMessage Deserialize<TMessage>(byte[] message) where TMessage : class
		{
			return this.Deserialize<TMessage>(ASCIIEncoding.ASCII.GetString(message));
		}

		public TMessage Deserialize<TMessage>(Stream message) where TMessage : class
		{
			TMessage result = default(TMessage);

			try
			{
				using (var stream = new MemoryStream())
				{
					var textconverter = new UTF8Encoding();
					var data = textconverter.GetString(stream.ToArray());
					result = this.Deserialize<TMessage>(data);
				}
			}
			catch
			{
				throw;
			}

			return result;
		}

		public TMessage Deserialize<TMessage>(string message) where TMessage : class
		{
			var payload = JsonConvert.DeserializeObject<JSONPayload>(message, settings);
			var result = DeserializeInternal(payload) as TMessage;
			return result;
		}

		public string SerializeNative(object message)
		{
			return JsonConvert.SerializeObject(message, Formatting.Indented, settings);
		}

		public string Serialize(object message)
		{
			var payload = new JSONPayload
			              	{
			              		Type = string.Concat(message.GetType().FullName, ",", message.GetType().Assembly.GetName().Name),
			              		Payload = JsonConvert.SerializeObject(message)
			              	};
			return JsonConvert.SerializeObject(payload, Formatting.Indented, settings);
		}

		public byte[] SerializeToBytes(object message)
		{
			var result = this.Serialize(message);
			return ASCIIEncoding.ASCII.GetBytes(result);
		}

		public void AddType(Type newType)
		{
			
		}

		public void AddTypes(ICollection<Type> newTypes)
		{
		
		}

		private object DeserializeInternal(JSONPayload payload)
		{
			var instance = payload.GetPayloadInstance();
			var result = JsonConvert.DeserializeObject(payload.Payload, instance.GetType(), settings);
			return result;
		}
	}
}