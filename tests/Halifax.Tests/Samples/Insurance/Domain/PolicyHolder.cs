namespace Halifax.Tests.Samples.Insurance.Domain
{
	/// <summary>
	/// A policy holder is the person that is recorded to an account for the insurance agency.
	/// </summary>
	public class PolicyHolder
	{
		public string SSN { get; set; }
		public string DOB { get; set; }
		public Name Name { get; set; }
		public Phone HomePhone { get; set; }
		public Phone WorkPhone { get; set; }
	}
}