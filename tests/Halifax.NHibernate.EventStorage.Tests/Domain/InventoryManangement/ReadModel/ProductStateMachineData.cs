using System;
using Halifax.StateMachine;

namespace Halifax.NHibernate.Tests.Domain.InventoryManangement.ReadModel
{
	public class ProductStateMachineData : IStateMachineData
	{
		public Guid Id { get; set; }
		public string State { get; set; }
		public string Name { get; set; }
		public int Occurrences { get; set; }
	}
}