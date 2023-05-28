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
    }
}