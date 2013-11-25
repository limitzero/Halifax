using System;
using Halifax.Events;

namespace Halifax.Tests.Samples.Insurance.Domain
{
	/// <summary>
	/// Parent event denoting changes to the aggregate <seealso cref="Account"/>
	/// </summary>
	[Serializable]
	public class AccountChangedEvent : Event
	{
		/// <summary>
		/// Gets or sets the account number that the event will be recorded for.
		/// </summary>
		public string AccountNumber { get; set; }
	}
}