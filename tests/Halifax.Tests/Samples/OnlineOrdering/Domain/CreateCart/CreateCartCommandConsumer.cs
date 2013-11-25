using Halifax.Commands;
using Halifax.Domain;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.CreateCart
{
    public class CreateCartCommandConsumer 
        : CommandConsumer.For<CreateCartCommand>
    {
        private readonly IAggregateRootRepository repository;

        public CreateCartCommandConsumer(IAggregateRootRepository repository)
        {
            this.repository = repository;
        }

        public override AggregateRoot Execute(CreateCartCommand command)
        {
            var cart = repository.Get<ShoppingCart>(CombGuid.NewGuid());
			cart.Create(command.Username);
        	return cart;
        }
    }
}