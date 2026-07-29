using System.Data;

public interface IRecordRepository
{
    //string GetTablePrefix(string filePath);
    bool TableExists(string tableName);
    void CreateTableFromDataTable(string tableName, DataTable table);
    void InsertDataTable(string tableName, DataTable table);
    void TruncateTable(string tableName);

}