using Halifax;
using Halifax.Commanding;
using Halifax.Storage.Aggregates;
using Halifax.Tests.Samples.OnlineOrdering.Domain;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.AddItem
{
    public class AddItemToCartCommandConsumer 
        : CommandConsumer.For<AddItemToCartCommand>
    {
        private readonly IDomainRepository _repository;

        public AddItemToCartCommandConsumer(IDomainRepository repository)
        {
            _repository = repository;
        }

        public override void Execute(IUnitOfWorkSession session, AddItemToCartCommand command)
        {
            var cart = _repository.Find<ShoppingCart>(command.CartId);
            cart.AddItem(command.Username, command.SKU, command.Quantity);
            session.Accept(cart);
        }
    }
}