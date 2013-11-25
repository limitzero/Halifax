using System;
using System.Collections.Generic;

namespace Halifax.Tests.Samples.Insurance.Domain.Auto
{
	public class CreateAutoPolicyCommand : AccountCommand
	{
		public Guid PolicyId { get; set; }
		public Vehicle Vehicle { get; set; }
		public Name PrimaryDriver { get; set; }
		public List<Name> Drivers { get; set; }
	}
}