using System;
using System.Runtime.Serialization;

namespace Halifax.Internals.Exceptions
{
    public class HalifaxException : ApplicationException
    {
        public HalifaxException()
        {
        }

        public HalifaxException(string message)
            : base(message)
        {
        }

        public HalifaxException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public HalifaxException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}