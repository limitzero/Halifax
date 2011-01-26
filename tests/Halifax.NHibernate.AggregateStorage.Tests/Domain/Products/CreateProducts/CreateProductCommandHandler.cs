using Axiom.Commanding;
using Axiom.Storage.Aggregates;

namespace Axiom.NHibernate.AggregateStorage.Tests.Domain.Products.CreateProducts
{
    public class CreateProductCommandHandler : 
        CommandConsumer.For<CreateProductCommand>
    {
        private readonly IDomainRepository _repository;

        public CreateProductCommandHandler(IDomainRepository repository)
        {
            _repository = repository;
        }

        public override void Execute(CreateProductCommand command)
        {
            var product = _repository.Create<Product>();
            product.Create(command);
        }
    }
}