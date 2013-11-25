namespace Halifax.Tests.Samples.Insurance.Domain.Marketing
{
	public class CreateAccountCommand : AccountCommand
	{
		public string Agent { get; set; }
		public PolicyHolder PolicyHolder { get; set; }
	}
}