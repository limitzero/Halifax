using Halifax.Domain;
using Halifax.Events;
using Halifax.Read;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.AddItem
{
    public class ItemAddedToCartEventConsumer :
        EventConsumer.For<ItemAddedToCart>
    {
    	private readonly IReadModelRepository<ReadModel.ShoppingCartItem> repository;

    	public ItemAddedToCartEventConsumer(IReadModelRepository<ReadModel.ShoppingCartItem> repository)
    	{
    		this.repository = repository;
    	}

    	public void Handle(ItemAddedToCart @event)
        {
			this.repository.Insert(new ReadModel.ShoppingCartItem()
			                       	{
			                       		Id =  @event.ItemId,
										ShoppingCartId = @event.EventSourceId, 
										Quantity =  @event.Quantity, 
										SKU =  @event.SKU
			                       	});
        }
    }
}