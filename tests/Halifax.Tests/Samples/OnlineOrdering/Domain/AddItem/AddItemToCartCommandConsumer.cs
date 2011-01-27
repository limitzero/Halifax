using Halifax.Commanding;
using Halifax.Storage.Aggregates;

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

        public override void Execute(IUnitOfWork session, AddItemToCartCommand command)
        {
            var cart = _repository.Find<ShoppingCart>(command.CartId);

            using (ITransactedSession txn = session.BeginTransaction(cart))
            {
                cart.AddItem(command.Username, command.SKU, command.Quantity);
            }
            
        }
    }
}