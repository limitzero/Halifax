using System;

namespace Halifax
{
	public static class ObjectExtentions
	{
		public static void NullCheck(this object value)
		{
			NullCheck(value, string.Empty);
		}

		public static void NullCheck(this object value, string message)
		{
			ArgumentNullException exception = new ArgumentNullException();

			if(value == null)
			{
				if(string.IsNullOrEmpty(message) == false)
				{
					exception = new ArgumentNullException(message);
				}
			}
			throw exception;
		}

		public static bool IsEqualTo(this int value, int other)
		{
			return value.Equals(other);
		}

		public static bool IsEqualTo(this  decimal value, decimal other)
		{
			return value.Equals(other);
		}

		public static bool IsEqualTo(this  double value, double other)
		{
			return value.Equals(other);
		}

		public static bool IsGreaterThan(this int value, int other)
		{
			return value > other;
		}

		public static bool IsGreaterThan(this decimal value, decimal other)
		{
			return value > other;
		}

		public static bool IsGreaterThan(this double value, double other)
		{
			return value > other;
		}

		public static bool IsGreaterThanOrEqualTo(this int value, int other)
		{
			return value >= other;
		}

		public static bool IsGreaterThanOrEqualTo(this decimal value, decimal other)
		{
			return value >= other;
		}

		public static bool IsGreaterThanOrEqualTo(this double value, double other)
		{
			return value >= other;
		}

		public static bool IsLessThan(this int value, int other)
		{
			return value < other;
		}

		public static bool IsLessThan(this decimal value, decimal other)
		{
			return value < other;
		}

		public static bool IsLessThan(this double value, double other)
		{
			return value < other;
		}

		public static bool IsLessThanOrEqualTo(this int value, int other)
		{
			return value <= other;
		}

		public static bool IsLessThanOrEqualTo(this decimal value, decimal other)
		{
			return value <= other;
		}

		public static bool IsLessThanOrEqualTo(this double value, double other)
		{
			return value <= other;
		}
	}
}