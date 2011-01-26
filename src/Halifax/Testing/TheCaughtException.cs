using System;

namespace Halifax.Testing
{
    public class TheCaughtException
    {
        private readonly Exception _theException;

        public TheCaughtException(Exception theException)
        {
            _theException = theException;
        }

        /// <summary>
        /// This will inspect the currently caught exception and 
        /// determine if it is of the requested type.
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <returns></returns>
        public bool WillBeOfType<TException>()
        {
            return typeof (TException) == _theException.GetType();
        }

        /// <summary>
        /// This will inspect the currently caught exception 
        /// and determine if the exception message is 
        /// as expected.
        /// </summary>
        /// <param name="message">The current exception message expected.</param>
        /// <returns></returns>
        public bool WillHaveMessage(string message)
        {
            return _theException.Message == message;
        }
    }
}