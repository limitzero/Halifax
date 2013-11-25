using System.Linq;
using Halifax.NHibernate.EventStore;
using Halifax.NHibernate.ReadModel;
using Halifax.NHibernate.Tests.Domain.InventoryManangement.Domain.CreateProducts;
using Halifax.NHibernate.Tests.Domain.InventoryManangement.ReadModel;
using Xunit;
using Product = Halifax.NHibernate.Tests.Domain.InventoryManangement.Domain.Product;

namespace Halifax.NHibernate.Tests
{
	public class when_a_product_is_created : 
		BaseNHibernateAggregateRootTestFixture<Product, CreateProduct>
	{
		private INHibernateReadModelSchemaManager read_model_schema;
		private INHibernateEventStoreSchemaManager event_store_schema;
		private string name = "Windex";
		private string description = "Window Cleaner";

		public override void Initially()
		{
			read_model_schema = this.Configuration
				.CurrentContainer().Resolve<INHibernateReadModelSchemaManager>();

			event_store_schema = this.Configuration
				.CurrentContainer().Resolve<INHibernateEventStoreSchemaManager>();

			read_model_schema.CreateSchema();
			event_store_schema.CreateSchema();
		}

		public override CreateProduct When()
		{
			return new CreateProduct(name, description);
		}

		[Fact]
		public void it_should_denote_that_event_is_producted_indicating_a_new_product_has_been_created()
		{
			Assert.True(PublishedEvents.Latest<ProductCreated>().Description.Equals(this.description));
			Assert.True(PublishedEvents.Latest<ProductCreated>().Name.Equals(this.name));
		}

		[Fact]
		public void it_should_return_the_listing_with_the_newly_created_product_from_the_read_model()
		{
			var query = new AllProductsQuery();
			ExecuteQueryOverReadModel(query);
			Assert.Equal(1, query.Result.Count());
		}
	}
}
