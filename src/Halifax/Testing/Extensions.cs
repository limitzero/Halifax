using System;

namespace Halifax.Testing
{
    public static class Extentions
    {
        public static void WillBeOfType<TTYPE>(this ThePublishedEvents current) where TTYPE : class
        {
            if (current.GetType() != typeof(TTYPE))
                throw new Exception(string.Format("Expected: {0}, Actual: {1}", current.GetType().FullName,
                                                  typeof(TTYPE).GetType().FullName));
        }

        public static void WillNotBeOfType<TTYPE>(this ThePublishedEvents current) where TTYPE : class
        {
            if (current.GetType() == typeof(TTYPE))
                throw new Exception(string.Format("Expected: {0}, Actual: {1}", current.GetType().FullName,
                                                  typeof(TTYPE).GetType().FullName));
        }

        public static void WillBeOfType<TTYPE>(this object current) where TTYPE : class
        {
            if (current.GetType() != typeof(TTYPE))
                throw new Exception(string.Format("Expected: {0}, Actual: {1}", current.GetType().FullName,
                                                  typeof(TTYPE).GetType().FullName));
        }

        public static void WillNotBeOfType<TTYPE>(this object current) where TTYPE : class
        {
            if (current.GetType() == typeof(TTYPE))
                throw new Exception(string.Format("Expected: {0}, Actual: {1}", current.GetType().FullName,
                                                  typeof(TTYPE).GetType().FullName));
        }

        public static void WillHaveMessage(this Exception current, string value)
        {
            WillBe(current.Message, value);
        }

        public static void WillBe(this string current, string value)
        {
            if (string.Compare(current, value) != 0)
                throw new Exception(string.Format("Expected: {0}, Actual: {1}", current, value));
        }

        public static void WillBe(this int current, int value)
        {
            if (current != value)
                throw new Exception(string.Format("Expected: {0}, Actual: {1}", current, value));
        }

        public static void WillNotBe(this int current, int value)
        {
            if (current == value)
                throw new Exception(string.Format("Expected: {0}, Actual: {1}", current, value));
        }

        public static void WillBe(this decimal current, decimal value)
        {
            if (decimal.Compare(current, value) != 0)
                throw new Exception(string.Format("Expected: {0}, Actual: {1}", current, value));
        }

        public static void WillNotBe(this decimal current, decimal value)
        {
            if (decimal.Compare(current, value) > 0 || decimal.Compare(current, value) < 0)
                throw new Exception(string.Format("Expected: {0}, Actual: {1}", current, value));
        }

        public static void WillBe(this Guid current, Guid value)
        {
            if (current.ToString().ToLower() != value.ToString().ToLower())
                throw new Exception(string.Format("Expected: {0}, Actual: {1}", current, value));
        }

        public static void WillNotBe(this Guid current, Guid value)
        {
            if (current.ToString().ToLower() == value.ToString().ToLower())
                throw new Exception(string.Format("Expected: {0}, Actual: {1}", current, value));
        }

    }
}