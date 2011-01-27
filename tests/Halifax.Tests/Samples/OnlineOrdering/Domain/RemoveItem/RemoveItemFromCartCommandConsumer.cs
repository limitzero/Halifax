using Halifax.Commanding;
using Halifax.Storage.Aggregates;

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

        public override void Execute(IUnitOfWork session, RemoveItemFromCartCommand command)
        {
            var cart = _repository.Find<ShoppingCart>(command.CartId);

            using (ITransactedSession txn = session.BeginTransaction(cart))
            {
                cart.RemoveItem(command.SKU);
            }
            
        }
    }
}