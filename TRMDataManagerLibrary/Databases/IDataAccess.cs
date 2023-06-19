namespace TRMDataManagerLibrary.Databases
{
    public interface IDataAccess
    {
        List<T> LoadData<T, U>(string sqlStatement,
                              U parameters,
                              string connectionStringName);
        void SaveData<T>(string sqlStatement,
                        T parameters,
                        string connectionStringName);
        void StartTransaction(string connectionStringName);
        public List<T> LoadDataInTransaction<T, U>(string sqlStatement,
                                                  U parameters);
        public void SaveDataInTransaction<T>(string sqlStatement,
                                            T parameters);
        public void CommitTransaction();
        public void RollbackTransaction();
        public void Dispose();
    }
}