using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUILibrary.Models
{
    public class CartItemModel
    {
        public UIProductModel? Product { get; set; }
        public int QuantityInCart { get; set; }
    }
}
