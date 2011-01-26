using Halifax;
using Halifax.Commanding;
using Halifax.Storage.Aggregates;
using Halifax.Tests.Samples.OnlineOrdering.Domain;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.RemoveItem
{
    public class RemoveItemFromCartCommandConsumer : 
        CommandConsumer.For<RemoveItemFromCartCommand>
    {
        private readonly IDomainRepository _repository;

        public RemoveItemFromCartCommandConsumer(IDomainRepository repository)
        {
            _repository = repository;
        }

        public override void Execute(IUnitOfWorkSession session, RemoveItemFromCartCommand command)
        {
            var cart = _repository.Find<ShoppingCart>(command.CartId);
            cart.RemoveItem(command.SKU);
            session.Accept(cart);
        }
    }
}