using Halifax;
using Halifax.Commanding;
using Halifax.Storage.Aggregates;
using Halifax.Tests.Samples.OnlineOrdering.Domain;

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

        public override void Execute(IUnitOfWorkSession session,  CreateCartCommand command)
        {
            var cart = _repository.Create<ShoppingCart>();
            cart.Create(command.Username);
            session.Accept(cart);
        }
    }
}