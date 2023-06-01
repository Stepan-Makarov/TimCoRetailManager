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
        private const string connectionStringName = "TRMDataSqlDb";

        public SqlData(IDataAccess db)
        {
            _db = db;
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
    }
}
