using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUILibrary.Models;

namespace TRMDesktopUILibrary.Api
{
    public class UserEndPoint : IUserEndPoint
    {
        private readonly IAPIHelper _apiHelper;

        public UserEndPoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<List<UserUIModel>> GetAllUsers()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/User/Admin/GetAll"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<UserUIModel>>();

                    return result;
                }

                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
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
