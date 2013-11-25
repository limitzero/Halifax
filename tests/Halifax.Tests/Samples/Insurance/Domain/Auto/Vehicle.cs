namespace Halifax.Tests.Samples.Insurance.Domain.Auto
{
	public class Vehicle
	{
		public string Make { get; set; }
		public string Model { get; set; }
		public int Year { get; set; }
		public long Miles { get; set; }

		public Vehicle(string make, string model, int year, long miles = 0)
		{
			Make = make;
			Model = model;
			Year = year;
			Miles = miles;
		}
	}
}