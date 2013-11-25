using System;
using Halifax.Events;
using Halifax.NHibernate.Tests.Domain.InventoryManangement.Domain.CreateProducts;

namespace Halifax.NHibernate.Tests.Domain.InventoryManangement.Domain
{
	public class SampleProductStateMachine : 
		StateMachine.StateMachine<ReadModel.ProductStateMachineData>, 
		EventConsumer.For<CreateProducts.ProductCreated>
	{
		public SampleProductStateMachine()
		{
			CorrelatedBy<ProductCreated>(m =>m.Name, s=>s.Name);	
		}

		public void Handle(ProductCreated @event)
		{
			if(this.Data.Occurrences == 2) return;

			this.Data.Occurrences++;
		}
	}
}