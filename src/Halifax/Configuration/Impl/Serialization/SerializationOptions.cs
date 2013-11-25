using Halifax.Configuration.Impl.Serialization.Impl;

namespace Halifax.Configuration.Impl.Serialization
{
	/// <summary>
	/// Marker for the serialization options.
	/// </summary>
	public class SerializationOptions
	{
		private readonly IContainer container;

		public SerializationOptions(IContainer container)
		{
			this.container = container;
		}

		public SerializationOptions UsingJSON()
		{
			var serializer = new JSONSerializationProvider();
			this.container.RegisterInstance<ISerializationProvider, JSONSerializationProvider>(serializer);
			return this;
		}

		public SerializationOptions UsingSharpSerializer()
		{
			var serializer = new SharpSerializerSerializationProvider();
			this.container.RegisterInstance<ISerializationProvider, SharpSerializerSerializationProvider>(serializer);
			return this;
		}

		public SerializationOptions UsingNetDataContract()
		{
			var serializer = new DataContractSerializationProvider();
			this.container.RegisterInstance<ISerializationProvider, DataContractSerializationProvider>(serializer);
			return this;
		}

	}
}