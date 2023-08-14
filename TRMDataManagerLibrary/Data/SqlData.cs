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

        public decimal GetTaxRate()
        {
            decimal taxRate = 0;

            try
            {
                taxRate = _config.GetValue<decimal>("TaxRate");
            }
            catch (Exception)
            {

                throw new Exception("The tax rate is not set up properly");
            }

            decimal output = taxRate / 100;

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

                decimal taxRate = GetTaxRate();

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

            // Should be a better way 
            try
            {
                _db.StartTransaction(connectionStringName);

                // Save Sale information to Data base
                _db.SaveDataInTransaction("dbo.spSale_Insert",
                        new { sale.CashierId, sale.SaleDate, sale.SubTotal, sale.Tax, sale.Total });

                // Load Sale Id for Sale Details
                sale.Id = _db.LoadDataInTransaction<int, dynamic>("dbo.spSale_GetId",
                                        new { CashierId = sale.CashierId, SaleDate = sale.SaleDate })
                                        .FirstOrDefault();

                // Add Sale Id for each Sale Detail Model
                foreach (var detail in details)
                {
                    detail.SaleId = sale.Id;

                    // Save Sale Detail Model to Data Base
                    _db.SaveDataInTransaction("dbo.spSaleDetail_Insert", detail);
                }

                _db.CommitTransaction();
            }
            catch
            {
                _db.RollbackTransaction();
                throw;
            }
        }

        public List<SaleReportModel> GetSalesReport()
        {
            var output = _db.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport",
                                                        new { },
                                                        connectionStringName);
            return output;
        }

        public List<InventoryModel> GetInventory()
        {
            var output = _db.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll",
                                                        new { },
                                                        connectionStringName);
            return output;
        }

        public void InsertInventory(InventoryModel item)
        {
            var insertedItem = new {ProductId = item.ProductId,
                                    Quantity = item.Quantity,
                                    PurchasePrice = item.PurchasePrice,
                                    PurchaseDate = item.PurchaseDate};

            _db.SaveData("dbo.spInventory_Insert", insertedItem, connectionStringName);
        }
    }
}
