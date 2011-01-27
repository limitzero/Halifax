using Halifax.Commanding;
using Halifax.Storage.Aggregates;

namespace Halifax.NHibernate.EventStorage.Tests.Domain.Products.CreateProducts
{
    public class CreateProductCommandHandler : 
        CommandConsumer.For<CreateProductCommand>
    {
        private readonly IDomainRepository _repository;

        public CreateProductCommandHandler(IDomainRepository repository)
        {
            _repository = repository;
        }

        public override void Execute(IUnitOfWork session, CreateProductCommand command)
        {
            var product = _repository.Create<Product>();

            using (ITransactedSession txn = session.BeginTransaction(product))
            {
                product.Create(command);
            }

        }
    }
}