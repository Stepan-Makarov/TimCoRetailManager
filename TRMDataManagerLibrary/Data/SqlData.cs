using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManagerLibrary.Databases;
using TRMDataManagerLibrary.Models;

namespace TRMDataManagerLibrary.Data
{
    public class SqlData
    {
        private readonly IDataAccess _db;
        private readonly IConfiguration _config;
        private const string connectionStringName = "TRMDataSqlDb";

        public SqlData(IDataAccess db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }
        public UserModel GetUserById(string Id)
        {
            var output =  _db.LoadData<UserModel, dynamic>("dbo.spUser_LookUp",
                                                        new { Id = @Id },
                                                        connectionStringName).FirstOrDefault();
            return output;
        }

        public List<ProductModel> GetAllProducts()
        {
            var output = _db.LoadData<ProductModel, dynamic>("dbo.spProduct_GetAll",
                                                        new { },
                                                        connectionStringName);
            return output;
        }

        public ProductModel GetProductById(int productId)
        {
            var output = _db.LoadData<ProductModel, dynamic>("dbo.spProduct_GetById",
                                                        new { Id = productId },
                                                        connectionStringName).FirstOrDefault();
            return output;
        }

        public void SaveSale(SaleModel? saleInfo, string? cashierId)
        {
            //ToDo Make it SOLID

            //Get avaliable information about Sale Detail
            List<SaleDetailDbModel> details = new List<SaleDetailDbModel>();

            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDbModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                var productInfo = GetProductById(item.ProductId);

                if (productInfo == null)
                {
                    throw new Exception($"The product Id of {item.ProductId} could not be found in the data base");
                }

                detail.PurchasePrice = productInfo.RetailPrice * detail.Quantity;

                decimal taxRate = _config.GetValue<decimal>("taxRate") / 100;

                if (productInfo.IsTaxable)
                {
                    detail.Tax = (detail.PurchasePrice * taxRate);
                }

                details.Add(detail);
            }
            
            // Get All information about Sale
            SaleDbModel sale = new SaleDbModel
            {
                CashierId = cashierId,
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = details.Sum(x => x.Tax),
            };

            sale.Total = sale.SubTotal + sale.Tax;

            // Save Sale information to Data base
            _db.SaveData("dbo.spSale_Insert",
                        new { sale.CashierId, sale.SaleDate, sale.SubTotal, sale.Tax, sale.Total },
                        connectionStringName);

            // Load Sale Id for Sale Details
            sale.Id = _db.LoadData<int, dynamic>("dbo.spSale_GetId",
                                       new { CashierId = sale.CashierId, SaleDate = sale.SaleDate },
                                       connectionStringName).FirstOrDefault();

            // Add Sale Id for each Sale Detail Model
            foreach (var detail in details)
            {
                detail.SaleId = sale.Id;

                // Save Sale Detail Model to Data Base
                _db.SaveData("dbo.spSaleDetail_Insert", detail, connectionStringName);
            }
        }
    }
}
