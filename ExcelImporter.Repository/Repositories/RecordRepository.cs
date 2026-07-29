using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace ExcelImporter.Repository.Repositories;

public class RecordRepository : IRecordRepository
{
    private readonly string _connectionString;

    public RecordRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    //public string GetTablePrefix(string filePath)
    //{
    //    string fileName = Path.GetFileNameWithoutExtension(filePath);
    //    return fileName.Split('_')[0];
    //}

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
            BatchSize = 1
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

    private string MapToSQLType(Type type)
    {
        if (type == typeof(long))
            return "NUMBER"; if (type == typeof(decimal)) return "NUMBER(18,2)";
        if (type == typeof(DateTime)) return "DATE";
        return "VARCHAR2(500)";
    }

    public void TruncateTable(string tableName)
    {
        using var connection = new OracleConnection(_connectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = $"TRUNCATE TABLE {tableName.ToUpper()}";
        cmd.ExecuteNonQuery();
    }
}