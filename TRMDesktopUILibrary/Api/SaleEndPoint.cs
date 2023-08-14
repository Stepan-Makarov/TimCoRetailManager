using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUILibrary.Models;

namespace TRMDesktopUILibrary.Api
{
    public class SaleEndPoint : ISaleEndPoint
    {
        private readonly IAPIHelper _apiHelper;

        public SaleEndPoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task PostSale(SaleUIModel sale)
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/Sale", sale))
            {
                if (response.IsSuccessStatusCode)
                {
                    //log successfull code?
                }

                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<decimal> GetTaxRate()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/Sale/GetTaxRate"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<decimal>();

                    return result;
                }

                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
