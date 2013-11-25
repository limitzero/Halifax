namespace Halifax.Tests.Samples.Insurance.Domain
{
	public class Name
	{
		public string FirstName { get; private set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }

		public Name()
		{
		}

		public Name(string firstName, string middleName, string lastName)
		{
			FirstName = firstName;
			MiddleName = middleName;
			LastName = lastName;
		}
	}
}