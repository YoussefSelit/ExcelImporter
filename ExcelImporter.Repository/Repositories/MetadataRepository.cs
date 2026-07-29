using ExcelImporter.Repository.Entities;
using ExcelImporter.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

public class MetadataRepository : IMetadataRepository
{
    private readonly string _connectionString;

    public MetadataRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public FileConfig GetFileConfigByPrefix(string prefix)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT ID,
                   PREFIX,
                   HEADERROW,
                   IGNOREFIRSTNOOFROWS,
                   IGNOREFIRSTNOOFCOLUMNS,
                   IGNORELASTNOOFROWS,
                   IGNORELASTNOOFCOLUMNS
            FROM FILECONFIG
            WHERE PREFIX = @prefix";

        cmd.Parameters.AddWithValue("@prefix", prefix);

        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapFileConfig(reader) : null;
    }

    private static FileConfig MapFileConfig(SqlDataReader reader)
    {
        return new FileConfig
        {
            Id = Convert.ToInt32(reader["ID"]),
            Prefix = reader["PREFIX"].ToString(),
            HeaderRow = reader["HEADERROW"] == DBNull.Value ? null : Convert.ToInt32(reader["HEADERROW"]),
            IgnoreFirstNoOfRows = reader["IGNOREFIRSTNOOFROWS"] == DBNull.Value ? null : Convert.ToInt32(reader["IGNOREFIRSTNOOFROWS"]),
            IgnoreFirstNoOfColumns = reader["IGNOREFIRSTNOOFCOLUMNS"] == DBNull.Value ? null : Convert.ToInt32(reader["IGNOREFIRSTNOOFCOLUMNS"]),
            IgnoreLastNoOfRows = reader["IGNORELASTNOOFROWS"] == DBNull.Value ? null : Convert.ToInt32(reader["IGNORELASTNOOFROWS"]),
            IgnoreLastNoOfColumns = reader["IGNORELASTNOOFCOLUMNS"] == DBNull.Value ? null : Convert.ToInt32(reader["IGNORELASTNOOFCOLUMNS"])
        };
    }

    public void LogFileImport(FileImportLog log)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
        INSERT INTO FILEIMPORTLOG
            (FILENAME, STARTTIME, ENDTIME)
        VALUES
            (@fileName, @startTime, @endTime)";

        cmd.Parameters.AddWithValue("@fileName", log.FileName);
        cmd.Parameters.AddWithValue("@startTime", log.StartTime);
        cmd.Parameters.AddWithValue("@endTime", log.EndTime);

        cmd.ExecuteNonQuery();
    }
}