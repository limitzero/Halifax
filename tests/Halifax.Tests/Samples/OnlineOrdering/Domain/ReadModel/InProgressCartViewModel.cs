namespace Halifax.Tests.Samples.OnlineOrdering.Domain.ReadModel
{
    public class InProgressCartViewModel
    {
        public InProgressCartViewModel(string username, string sku, int quantity)
        {
            Username = username;
            SKU = sku;
            Quantity = quantity;
        }

        public string Username { get; set; }
        public string SKU { get; set; }
        public int Quantity { get; set; }
    }
}