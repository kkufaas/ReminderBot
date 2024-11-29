using System.Globalization;
using System.Xml.XPath;
using Microsoft.Data.Sqlite;

namespace ReminderBot.ReminderSqlite.Repositories;

public class HumanDataRepository : IHumanDataRepository
{
    private readonly string _connectionString;

    public HumanDataRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void AssignDate(long tgId, DateTime date)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = 
        @$"UPDATE Data SET DateTime = '{date.ToString("O", CultureInfo.InvariantCulture)}' WHERE TgId = {tgId};";
        command.ExecuteNonQuery();
    }

    public void AssignState(long tgId, string state)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = 
        @$"UPDATE Data SET State = '{state}' WHERE TgId = {tgId};";
        command.ExecuteNonQuery();
    }

    public void AssignTitle(long tgId, string title)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = 
        @$"UPDATE Data SET Title = '{title}' WHERE TgId = {tgId};";
        command.ExecuteNonQuery();
    }

    public void CreateData(long tgId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = 
        @$"INSERT INTO Data(
            Title, TgId, State, DateTime
        ) VALUES ('',{tgId},'','');";
        command.ExecuteNonQuery();
    }

    public DateTime GetDateTime(long tgId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = 
        $"SELECT DateTime FROM Data WHERE TgId = {tgId};";
        var reader = command.ExecuteReader();
        reader.Read();
        string result = reader.GetString(0);
        connection.Close();
        return DateTime.Parse(result, CultureInfo.InvariantCulture);
    }

    public string GetState(long tgId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = 
        $"SELECT State FROM Data WHERE TgId = {tgId};";
        var reader = command.ExecuteReader();
        reader.Read();
        string result = reader.GetString(0);
        connection.Close();
        return result;
    }

    public string GetTitle(long tgId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = 
        $"SELECT Title FROM Data WHERE TgId = {tgId};";
        var reader = command.ExecuteReader();
        reader.Read();
        string result = reader.GetString(0);
        connection.Close();
        return result;
    }

    public bool HasData(long tgId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = 
        $"SELECT * FROM Data WHERE TgId = {tgId};";
        var reader = command.ExecuteReader();
        bool result = reader.Read();
        connection.Close();
        return result;
    }
}