using System;
using Halifax.Commands;

namespace Halifax.Tests.Samples.Insurance.Domain
{
	public class AccountCommand : Command
	{
		public Guid AccountNumber { get; set; }
	}
}