using Halifax.Commanding;
using Halifax.Storage.Aggregates;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.CreateCart
{
    public class CreateCartCommandConsumer 
        : CommandConsumer.For<CreateCartCommand>
    {
        private readonly IDomainRepository _repository;

        public CreateCartCommandConsumer(IDomainRepository repository)
        {
            _repository = repository;
        }

        public override void Execute(IUnitOfWork session,  CreateCartCommand command)
        {
            var cart = _repository.Create<ShoppingCart>();

            using (ITransactedSession txn = session.BeginTransaction(cart))
            {
                cart.Create(command.Username);
            }
            
        }
    }
}