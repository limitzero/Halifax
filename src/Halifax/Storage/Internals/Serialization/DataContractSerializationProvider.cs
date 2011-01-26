using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Halifax.Storage.Internals.Serialization
{
    public class DataContractSerializationProvider : ISerializationProvider
    {
        private DataContractSerializer _serializer;
        private List<Type> _types;

        public DataContractSerializationProvider()
        {
            if (_types == null)
                _types = new List<Type>();
        }

        #region ISerializationProvider Members

        public object Deserialize(string instance)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(instance);
            return Deserialize(bytes);
        }

        public object Deserialize(Stream stream)
        {
            return _serializer.ReadObject(stream);
        }

        public object Deserialize(byte[] bytes)
        {
            object retval = null;

            try
            {
                using (var stream = new MemoryStream(bytes))
                {
                    retval = _serializer.ReadObject(stream);
                }
            }
            catch (Exception exception)
            {
                throw;
            }

            return retval;
        }

        public TMessage Deserialize<TMessage>(byte[] message) where TMessage : class
        {
            return (TMessage) Deserialize(message);
        }

        public TMessage Deserialize<TMessage>(Stream message) where TMessage : class
        {
            return (TMessage) Deserialize(message);
        }

        public TMessage Deserialize<TMessage>(string message) where TMessage : class
        {
            return (TMessage) Deserialize(message);
        }

        public string Serialize(object message)
        {
            string retval = string.Empty;

            if (!_types.Contains(message.GetType()))
            {
                _types.Add(message.GetType());
                Initialize(_types);
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    _serializer.WriteObject(stream, message);
                    stream.Seek(0, SeekOrigin.Begin);

                    var textconverter = new UTF8Encoding();
                    retval = textconverter.GetString(stream.ToArray());
                }
            }
            catch (Exception exception)
            {
                throw;
            }

            return retval;
        }

        public byte[] SerializeToBytes(object message)
        {
            string retval = Serialize(message);
            return Encoding.ASCII.GetBytes(retval);
        }

        public void Initialize(IEnumerable<Type> types)
        {
            if (_types.Count > 0) return;

            _types = new List<Type>(types);
            _serializer = new DataContractSerializer(typeof (object), _types.ToArray());
        }

        public void AddType(Type newType)
        {
            _types.Add(newType);
            Initialize(_types);
        }

        public void AddTypes(ICollection<Type> newTypes)
        {
            _types.AddRange(newTypes);
            Initialize(_types);
        }

        #endregion
    }
}