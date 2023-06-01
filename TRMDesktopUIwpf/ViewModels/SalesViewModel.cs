using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUILibrary.Api;
using TRMDesktopUILibrary.Models;

namespace TRMDesktopUIwpf.ViewModels
{
    public class SalesViewModel : Screen
    {
        public SalesViewModel(IProductEndpoint productEndpoint)
        {
            _productEndpoint = productEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            var productList = await _productEndpoint.GetAllProducts();
            Products = new BindingList<UIProductModel>(productList);
        }

        private BindingList<UIProductModel>? _products;

		public BindingList<UIProductModel>? Products
        {
			get { return _products; }
			set 
			{
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
		}

        private BindingList<UIProductModel>? _cart;

        public BindingList<UIProductModel>? Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        private int _itemQuantity;
        private readonly IProductEndpoint _productEndpoint;

        public int ItemQuantity
        {
			get { return _itemQuantity; }
			set 
			{
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
            }
		}

        public string? SubTotal
        {
            get 
            {
                //Replace weth colculations
                return "0.00 ₽";
            }
        }
        public string? Tax
        {
            get
            {
                //Replace weth colculations
                return "0.00 ₽";
            }
        }
        public string? Total
        {
            get
            {
                //Replace weth colculations
                return "0.00 ₽";
            }
        }

        public bool CanAddToCart
        {
            get
            {
                bool output = false;

                //Something is checked
                //Something is in ItemQuantity

                return output;
            }
        }

        public void AddToCart()
        {
            //Add To Cart
        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;

                //Something is checked

                return output;
            }
        }

        public void CheckOut()
        {
            //Remove From Cart
        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                //Something is checked

                return output;
            }
        }

        public void RemoveFromCart()
        {
            //Check Out
        }
    }
}
