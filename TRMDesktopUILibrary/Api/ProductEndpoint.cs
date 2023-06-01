using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUILibrary.Models;

namespace TRMDesktopUILibrary.Api
{
    public class ProductEndpoint : IProductEndpoint
    {
        private readonly IAPIHelper _apiHelper;

        public ProductEndpoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<List<UIProductModel>> GetAllProducts()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/Product"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<UIProductModel>>();

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
