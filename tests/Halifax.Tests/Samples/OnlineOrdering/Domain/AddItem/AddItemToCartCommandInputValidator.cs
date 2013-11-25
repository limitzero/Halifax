using System;
using Halifax.Commands;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.AddItem
{
	public class AddItemToCartCommandInputValidator : 
		CommandInputValidator.For<AddItemToCartCommand>
	{
		public override void Validate(AddItemToCartCommand command)
		{
			Inspect.WithRules(
					Rule.For(command, c => c.Quantity)
						.When(c => c.Quantity >= 0)
							.ShowMessage(string.Format("The quanity of the line item '{0}' must be more than zero when added to the cart.", command.SKU))
				);
		}
	}
}