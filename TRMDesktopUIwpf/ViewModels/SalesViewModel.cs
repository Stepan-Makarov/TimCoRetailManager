﻿using AutoMapper;
using Caliburn.Micro;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUILibrary.Api;
using TRMDesktopUILibrary.Models;
using TRMDesktopUIwpf.Models;

namespace TRMDesktopUIwpf.ViewModels
{
    public class SalesViewModel : Screen
    {
        private readonly IProductEndpoint _productEndpoint;
        private readonly ISaleEndPoint _saleEndPoint;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public SalesViewModel(IProductEndpoint productEndpoint,ISaleEndPoint saleEndPoint, 
            IConfiguration config, IMapper mapper)
        {
            _productEndpoint = productEndpoint;
            _saleEndPoint = saleEndPoint;
            _config = config;
            _mapper = mapper;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            var productList = await _productEndpoint.GetAllProducts();
            var products = _mapper.Map<List<ProductDisplayModel>>(productList);
            Products = new BindingList<ProductDisplayModel>(products);
        }

        private BindingList<ProductDisplayModel>? _products;
		public BindingList<ProductDisplayModel>? Products
        {
			get { return _products; }
			set 
			{
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
		}

        
        private ProductDisplayModel? _selectedProduct;
        public ProductDisplayModel? SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        private CartItemDisplayModel? _selectedCartItem;
        public CartItemDisplayModel? SelectedCartItem
        {
            get { return _selectedCartItem; }
            set
            {
                _selectedCartItem = value;
                NotifyOfPropertyChange(() => SelectedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
            }
        }

        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();
        public BindingList<CartItemDisplayModel> Cart
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
                NotifyOfPropertyChange(() => CanRemoveFromCart);
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
                return CalculateSubTotal().ToString("C");
            }
        }

        private decimal CalculateSubTotal()
        {
            decimal subTotal = 0;

            foreach (var item in Cart)
            {
                subTotal += (item.Product.RetailPrice * item.QuantityInCart);
            }
            return subTotal;
        }

        private decimal CalculateTax()
        {
            decimal taxRate = _config.GetValue<decimal>("taxRate") / 100;
            decimal taxAmount = 0;

            taxAmount = Cart.Where(x => x.Product.IsTaxable).Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate);

            return taxAmount;
        }
        public string? Tax
        {
            get
            {
                return CalculateTax().ToString("C");
            }
        }
        public string? Total
        {
            get
            {
                decimal total = CalculateSubTotal() + CalculateTax();

                return total.ToString("C");
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
            CartItemDisplayModel? existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);

            if (existingItem != null)
            {
                existingItem.QuantityInCart += ItemQuantityToInt;
            }

            else
            {
                CartItemDisplayModel? CartItem = new CartItemDisplayModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantityToInt
                };
                Cart.Add(CartItem);
            }

            SelectedProduct.QuantityInStock -= ItemQuantityToInt;
            ItemQuantity = "1";

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;
                
                if (CanParseItemQuantityToInt && ItemQuantityToInt > 0 && SelectedCartItem?.QuantityInCart > 0)
                {
                    output = true;
                }

                return output;
            }
        }

        public void RemoveFromCart()
        {
            //Remove From Cart
            if (SelectedCartItem.QuantityInCart > ItemQuantityToInt)
            {
                SelectedCartItem.QuantityInCart -= ItemQuantityToInt;
                SelectedCartItem.Product.QuantityInStock += ItemQuantityToInt;
            }
            else
            {
                SelectedCartItem.Product.QuantityInStock += SelectedCartItem.QuantityInCart;
                Cart.Remove(SelectedCartItem);
            }

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                if (Cart.Count > 0)
                {
                    output = true;
                }

                return output;
            }
        }

        public async Task CheckOut()
        {
            SaleUIModel sale = new SaleUIModel();

            foreach (var item in Cart)
            {
                var detail = new SaleDetailUIModel
                {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                };

                sale.SaleDetails.Add(detail);
            }

            await _saleEndPoint.PostSale(sale);
        }
    }
}
