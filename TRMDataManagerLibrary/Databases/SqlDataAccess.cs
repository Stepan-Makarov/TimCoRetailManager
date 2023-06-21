using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDataManagerLibrary.Databases
{
    public class SqlDataAccess : IDataAccess, IDisposable
    {
        private readonly IConfiguration _config;

        public SqlDataAccess(IConfiguration config)
        {
            _config = config;
        }

        public List<T> LoadData<T, U>(string sqlStatement,
                                      U parameters,
                                      string connectionStringName)
        {
            string? connectionString = _config.GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                List<T> rows = connection.Query<T>(sqlStatement, parameters,
                    commandType: CommandType.StoredProcedure).ToList();

                return rows;
            }
        }

        public void SaveData<T>(string sqlStatement,
                                T parameters,
                                string connectionStringName)
        {
            string? connectionString = _config.GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Execute(sqlStatement, parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        private IDbConnection? _connection;
        private IDbTransaction? _transaction;
        private bool isClosed = false;
        
        public void StartTransaction(string connectionStringName)
        {
            string? connectionString = _config.GetConnectionString(connectionStringName);

            _connection = new SqlConnection(connectionString);
            _connection.Open();

            _transaction = _connection.BeginTransaction();

            isClosed = false;
        }

        public List<T> LoadDataInTransaction<T, U>(string sqlStatement,
                                                  U parameters)
        {
            List<T> rows = _connection.Query<T>(sqlStatement, parameters,
                commandType: CommandType.StoredProcedure, transaction: _transaction).ToList();

            return rows;
        }

        public void SaveDataInTransaction<T>(string sqlStatement,
                                            T parameters)
        {
            _connection.Execute(sqlStatement, parameters,
                commandType: CommandType.StoredProcedure, transaction: _transaction);
        }

        public void CommitTransaction()
        {
            _transaction.Commit();
            _transaction.Dispose();

            isClosed = true;
        }

        public void RollbackTransaction()
        {
            _transaction.Rollback();
            _transaction.Dispose();

            isClosed = true;
        }

        public void Dispose()
        {
            if (isClosed == false && _transaction != null)
            {
                try
                {
                    CommitTransaction();
                }
                catch
                {
                    //Log Issue
                }
            }

            _connection = null;
            _transaction = null;
        }
    }
}
