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
        private readonly IProductEndpoint _productEndpoint;
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

        private UIProductModel _selectedProduct;

        public UIProductModel SelectedProduct
        {
            get { return _selectedProduct; }
            set 
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();

        public BindingList<CartItemModel> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        private string? _itemQuantity = "1";
        public string? ItemQuantity
        {
			get { return _itemQuantity; }
			set 
			{
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);
                NotifyOfPropertyChange(() => CanParseItemQuantityToInt);
            }
		}
       
        private string? _errorMessage;

        public string? ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => ErrorMessage);
                NotifyOfPropertyChange(() => IsErrorMessageVisible);
            }
        }
        public bool IsErrorMessageVisible
        {
            get
            {
                bool output = false;

                if (CanParseItemQuantityToInt == false)
                {
                    output = true;
                }

                return output;
            }
        }
        public int ItemQuantityToInt
        {
            get
            {
                int itemQuantity;
                bool canParseToInt = int.TryParse(ItemQuantity, out itemQuantity);
                return itemQuantity;
            }
        }

        public bool CanParseItemQuantityToInt
        {
            get
            {
                bool canParseToInt = true;

                if (ItemQuantity != null)
                {
                    canParseToInt = int.TryParse(ItemQuantity, out int itemQuantity);
                }

                return canParseToInt;
            }
        }

        public string? SubTotal
        {
            get 
            {
                decimal subTotal = 0;

                foreach (var item in Cart)
                {
                    subTotal += (item.Product.RetailPrice * item.QuantityInCart);
                }
                return subTotal.ToString("C");
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
                ErrorMessage = "";
                if (CanParseItemQuantityToInt == false)
                {
                    ErrorMessage = "Type a valid number";
                }

                bool output = false;

                //Something is checked
                //Something is in ItemQuantity
                if (CanParseItemQuantityToInt && ItemQuantityToInt > 0 && SelectedProduct?.QuantityInStock >= ItemQuantityToInt)
                {
                    output = true;
                }

                return output;
            }
        }
        
        public void AddToCart()
        {
            CartItemModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);

            if (existingItem != null)
            {
                existingItem.QuantityInCart += ItemQuantityToInt;

                //There should be a better code
                Cart.Remove(existingItem);
                Cart.Add(existingItem);
            }

            else
            {
                CartItemModel? CartItem = new CartItemModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantityToInt
                };
                Cart.Add(CartItem);
            }

            SelectedProduct.QuantityInStock -= ItemQuantityToInt;
            ItemQuantity = "1";

            NotifyOfPropertyChange(() => SubTotal);
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
            NotifyOfPropertyChange(() => SubTotal);
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
