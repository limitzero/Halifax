using System.Collections.Generic;
using System.Linq;

namespace Halifax.Tests.Samples.OnlineOrdering.Domain.ReadModel
{
    public class ReadModelDB
    {
        private static List<InProgressCartViewModel> _model;

        static ReadModelDB()
        {
            if (_model == null)
                _model = new List<InProgressCartViewModel>();
        }

        public static void Refresh()
        {
           _model.Clear();
        }

        public static void CreateCartItem(string username, string sku, int quantity)
        {
            var cartViewModel = new InProgressCartViewModel(username, sku, quantity);
            _model.Add(cartViewModel);
        }

        public static void RemoveCartItem(string username, string sku)
        {
            var model = (from item in _model
                         where item.Username == username &&
                               item.SKU == sku
                         select item).FirstOrDefault();

            if (model != null)
                if (_model.Contains(model))
                    _model.Remove(model);
        }

        public static ICollection<InProgressCartViewModel> GetCurrentCart(string username)
        {
            var model = (from item in _model
                         where item.Username == username
                         select item).ToList();
            return model;
        }

 
    }
}