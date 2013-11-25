namespace Halifax.Tests.Samples.Insurance.Domain
{
	public class Phone
	{
		public string AreaCode { get; set; }
		public string Number { get; set; }
		public string Extension { get; set; }

		public Phone(string areaCode, string number, string extension = "")
		{
			AreaCode = areaCode;
			Number = number;
			Extension = extension;
		}
	}
}