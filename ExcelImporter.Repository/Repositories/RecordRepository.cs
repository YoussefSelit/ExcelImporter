//using Oracle.ManagedDataAccess.Client;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.IO;
//using System.Linq;

//public class RecordRepository : IRecordRepository
//{
//    private readonly string _connectionString;
//    //private const int BatchSize = 500;

//    public RecordRepository(string connectionString)
//    {
//        _connectionString = connectionString;
//    }

//    public string GetTablePrefix(string filePath)
//    {
//        string fileName = Path.GetFileNameWithoutExtension(filePath);
//        return fileName.Split('_')[0].ToUpper();
//    }

//    public bool TableExists(string tableName)
//    {
//        using var connection = new OracleConnection(_connectionString);
//        connection.Open();

//        using var cmd = connection.CreateCommand();

//        cmd.CommandText =
//            @"SELECT COUNT(*) 
//          FROM user_tables 
//          WHERE table_name = :tableName";

//        cmd.Parameters.Add(
//            new OracleParameter("tableName", tableName.ToUpper()));

//        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
//    }

//    //public bool TableExists(string tableName)
//    //{
//    //    using var connection = new OracleConnection(_connectionString);
//    //    connection.Open();

//    //    using var cmd = connection.CreateCommand();

//    //    cmd.CommandText =
//    //        @"SELECT COUNT(*) 
//    //      FROM user_objects 
//    //      WHERE object_name = :objectName";

//    //    cmd.Parameters.Add(
//    //        new OracleParameter("objectName", tableName));

//    //    int count = Convert.ToInt32(cmd.ExecuteScalar());

//    //    return count > 0;
//    //}

//    //public bool TableExists(string tableName)
//    //{
//    //    using var connection = new OracleConnection(_connectionString);
//    //    connection.Open();

//    //    using var cmd = connection.CreateCommand();
//    //    cmd.CommandText = "SELECT COUNT(*) FROM user_tables WHERE table_name = :tableName"; //3ashan neshouf lw fi data fel table wala mafish
//    //    cmd.Parameters.Add(new OracleParameter("tableName", tableName.ToUpper()));

//    //    int count = Convert.ToInt32(cmd.ExecuteScalar()); //executes the query and returns the count
//    //    return count > 0; //true (table exists) lw akbar mn 0, false lw la2
//    //}

//    //public void CreateTableFromDataTable(string tableName, DataTable table)
//    //{
//    //    using var connection = new OracleConnection(_connectionString);
//    //    connection.Open();

//    //    var columnDefs = new List<string>();
//    //    foreach (DataColumn col in table.Columns)
//    //    {
//    //        string oracleType = MapToSQLType(col.DataType); //defines the data type (for oracle) the column holds 
//    //        columnDefs.Add($"{col.ColumnName.ToUpper()} {oracleType}");  //takes column names ma3 each type
//    //    }

//    //    string sql = $"CREATE TABLE {tableName.ToUpper()} ({string.Join(", ", columnDefs)})";

//    //    using var cmd = connection.CreateCommand();
//    //    cmd.CommandText = sql;
//    //    cmd.ExecuteNonQuery();
//    //}

//    public void CreateTableFromDataTable(string tableName, DataTable table)
//    {
//        using var connection = new OracleConnection(_connectionString);
//        connection.Open();

//        var columnDefs = new List<string>();

//        foreach (DataColumn col in table.Columns)
//        {
//            string oracleType = MapToSQLType(col.DataType);

//            columnDefs.Add($"{col.ColumnName.ToUpper()} {oracleType}");
//        }

//        string sql = $"CREATE TABLE {tableName.ToUpper()} ({string.Join(", ", columnDefs)})";

//        Console.WriteLine(sql);   // ADD THIS

//        using var cmd = connection.CreateCommand();
//        cmd.CommandText = sql;
//        cmd.ExecuteNonQuery();
//    }


//    //public void InsertDataTable(string tableName, DataTable table)
//    //{
//    //    if (table.Rows.Count == 0)
//    //        return;

//    //    using var connection = new OracleConnection(_connectionString);
//    //    connection.Open();

//    //    var allRows = table.Rows.Cast<DataRow>().ToList();

//    //    foreach (var batch in allRows.Chunk(BatchSize))
//    //    {
//    //        InsertBatch(tableName, table.Columns, batch.ToList(), connection);
//    //    }
//    //}

//    public void InsertDataTable(string tableName, DataTable table)
//    {
//        if (table.Rows.Count == 0)
//            return;

//        Console.WriteLine($"Rows: {table.Rows.Count}");
//        Console.WriteLine($"Columns: {table.Columns.Count}");

//        foreach (DataRow row in table.Rows.Cast<DataRow>().Take(3))
//        {
//            Console.WriteLine(string.Join(" | ", row.ItemArray));
//        }

//        using var connection = new OracleConnection(_connectionString);
//        connection.Open();

//        using var bulkCopy = new OracleBulkCopy(
//            connection,
//            OracleBulkCopyOptions.UseInternalTransaction)
//        {
//            DestinationTableName = tableName.ToUpper(),
//            BatchSize = 5000
//        };

//        foreach (DataColumn column in table.Columns)
//        {
//            bulkCopy.ColumnMappings.Add(
//              column.ColumnName,
//              column.ColumnName.ToUpper());
//        }

//        try
//        {
//            bulkCopy.WriteToServer(table);
//        }
//        catch (Exception ex)
//        {
//            throw new Exception($"Bulk insert into {tableName} failed.", ex);
//        }
//    }

//    //private void InsertBatch(string tableName, DataColumnCollection columns, List<DataRow> batch, OracleConnection connection)
//    //{
//    //    using var transaction = connection.BeginTransaction();
//    //    try
//    //    {
//    //        using var cmd = connection.CreateCommand();
//    //        cmd.Transaction = transaction;

//    //        var columnNames = columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
//    //        var paramNames = columnNames.Select(c => ":" + c).ToList();

//    //        cmd.CommandText =
//    //            $"INSERT INTO {tableName} ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", paramNames)})";

//    //        cmd.ArrayBindCount = batch.Count;

//    //        foreach (DataColumn col in columns)
//    //        {
//    //            var values = batch.Select(r => r[col.ColumnName] ?? DBNull.Value).ToArray();
//    //            var oracleType = MapToOracleDbType(col.DataType);
//    //            cmd.Parameters.Add(":" + col.ColumnName, oracleType, values, ParameterDirection.Input);
//    //        }

//    //        cmd.ExecuteNonQuery();
//    //        transaction.Commit();
//    //    }
//    //    catch
//    //    {
//    //        transaction.Rollback();
//    //        throw;
//    //    }
//    //}

//    private string MapToSQLType(Type type)
//    {
//        if (type == typeof(int)) return "NUMBER";
//        if (type == typeof(decimal)) return "NUMBER(18,2)";
//        if (type == typeof(DateTime)) return "DATE";
//        return "VARCHAR2(500)";
//    }

//    //private OracleDbType MapToOracleDbType(Type type)
//    //{
//    //    if (type == typeof(int)) return OracleDbType.Int32;
//    //    if (type == typeof(decimal)) return OracleDbType.Decimal;
//    //    if (type == typeof(DateTime)) return OracleDbType.Date;
//    //    return OracleDbType.Varchar2;
//    //}
//}


using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

public class RecordRepository : IRecordRepository
{
    private readonly string _connectionString;
    //private const int BatchSize = 500;

    public RecordRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public string GetTablePrefix(string filePath)
    {
        string fileName = Path.GetFileNameWithoutExtension(filePath);
        return fileName.Split('_')[0];
    }

    public bool TableExists(string tableName)
    {
        using var connection = new OracleConnection(_connectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();

        cmd.CommandText =
            @"SELECT COUNT(*) 
          FROM user_tables 
          WHERE table_name = :tableName";

        cmd.Parameters.Add(
            new OracleParameter("tableName", tableName.ToUpper()));

        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    //public bool TableExists(string tableName)
    //{
    //    using var connection = new OracleConnection(_connectionString);
    //    connection.Open();

    //    using var cmd = connection.CreateCommand();

    //    cmd.CommandText =
    //        @"SELECT COUNT(*) 
    //      FROM user_objects 
    //      WHERE object_name = :objectName";

    //    cmd.Parameters.Add(
    //        new OracleParameter("objectName", tableName));

    //    int count = Convert.ToInt32(cmd.ExecuteScalar());

    //    return count > 0;
    //}

    //public bool TableExists(string tableName)
    //{
    //    using var connection = new OracleConnection(_connectionString);
    //    connection.Open();

    //    using var cmd = connection.CreateCommand();
    //    cmd.CommandText = "SELECT COUNT(*) FROM user_tables WHERE table_name = :tableName"; //3ashan neshouf lw fi data fel table wala mafish
    //    cmd.Parameters.Add(new OracleParameter("tableName", tableName.ToUpper()));

    //    int count = Convert.ToInt32(cmd.ExecuteScalar()); //executes the query and returns the count
    //    return count > 0; //true (table exists) lw akbar mn 0, false lw la2
    //}

    public void CreateTableFromDataTable(string tableName, DataTable table)
    {
        using var connection = new OracleConnection(_connectionString);
        connection.Open();

        var columnDefs = new List<string>();
        foreach (DataColumn col in table.Columns)
        {
            string oracleType = MapToSQLType(col.DataType); //defines the data type (for oracle) the column holds 
            columnDefs.Add($"\"{col.ColumnName.ToUpper()}\" {oracleType}"); ;  //takes column names ma3 each type
        }

        string sql = $"CREATE TABLE {tableName.ToUpper()} ({string.Join(", ", columnDefs)})";

        Console.WriteLine(sql);
        using var cmd = connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    //public void InsertDataTable(string tableName, DataTable table)
    //{
    //    if (table.Rows.Count == 0)
    //        return;

    //    using var connection = new OracleConnection(_connectionString);
    //    connection.Open();

    //    var allRows = table.Rows.Cast<DataRow>().ToList();

    //    foreach (var batch in allRows.Chunk(BatchSize))
    //    {
    //        InsertBatch(tableName, table.Columns, batch.ToList(), connection);
    //    }
    //}

    public void InsertDataTable(string tableName, DataTable table)
    {
        if (table.Rows.Count == 0)
        {
            Console.WriteLine("DataTable is empty. No rows to insert.");
            return;
        }
        Console.WriteLine($"Rows: {table.Rows.Count}");
        Console.WriteLine($"Columns: {table.Columns.Count}");

        foreach (DataRow row in table.Rows.Cast<DataRow>().Take(3))
        {
            Console.WriteLine(string.Join(" | ", row.ItemArray));
        }

        using var connection = new OracleConnection(_connectionString);
        connection.Open();

        using var bulkCopy = new OracleBulkCopy(
            connection,
            OracleBulkCopyOptions.UseInternalTransaction)
        {
            DestinationTableName = tableName.ToUpper(),
            BatchSize = 5000
        };

        foreach (DataColumn column in table.Columns)
        {
            bulkCopy.ColumnMappings.Add(
            column.ColumnName,
            column.ColumnName);
        }

        try
        {
            Console.WriteLine("Destination Table: " + bulkCopy.DestinationTableName);

            foreach (OracleBulkCopyColumnMapping map in bulkCopy.ColumnMappings)
            {
                Console.WriteLine($"{map.SourceColumn} -> {map.DestinationColumn}");
            }
            bulkCopy.WriteToServer(table);
        }
        catch (Exception ex)
        {
            throw new Exception($"Bulk insert into {tableName} failed.", ex);
        }
    }

    //private void InsertBatch(string tableName, DataColumnCollection columns, List<DataRow> batch, OracleConnection connection)
    //{
    //    using var transaction = connection.BeginTransaction();
    //    try
    //    {
    //        using var cmd = connection.CreateCommand();
    //        cmd.Transaction = transaction;

    //        var columnNames = columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
    //        var paramNames = columnNames.Select(c => ":" + c).ToList();

    //        cmd.CommandText =
    //            $"INSERT INTO {tableName} ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", paramNames)})";

    //        cmd.ArrayBindCount = batch.Count;

    //        foreach (DataColumn col in columns)
    //        {
    //            var values = batch.Select(r => r[col.ColumnName] ?? DBNull.Value).ToArray();
    //            var oracleType = MapToOracleDbType(col.DataType);
    //            cmd.Parameters.Add(":" + col.ColumnName, oracleType, values, ParameterDirection.Input);
    //        }

    //        cmd.ExecuteNonQuery();
    //        transaction.Commit();
    //    }
    //    catch
    //    {
    //        transaction.Rollback();
    //        throw;
    //    }
    //}

    private string MapToSQLType(Type type)
    {
        if (type == typeof(long))
            return "NUMBER"; if (type == typeof(decimal)) return "NUMBER(18,2)";
        if (type == typeof(DateTime)) return "DATE";
        return "VARCHAR2(500)";
    }

    //private OracleDbType MapToOracleDbType(Type type)
    //{
    //    if (type == typeof(int)) return OracleDbType.Int32;
    //    if (type == typeof(decimal)) return OracleDbType.Decimal;
    //    if (type == typeof(DateTime)) return OracleDbType.Date;
    //    return OracleDbType.Varchar2;
    //}
}