using Microsoft.Data.Sqlite;

namespace ReminderBot.ReminderSqlite.Utilites;

public class DataBaseCreator
{
    private string _dbName;
    private string? _password;

    public DataBaseCreator(string dbName, string? password = null)
    {
        _dbName = dbName;
        _password = password;
    }

    public void Run()
    {
        string connectionString = $"Data Source={_dbName};Foreign Keys=True";
        if (_password is not null)
        {
            connectionString += $"Password={_password};";
        }
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = 
            @"CREATE TABLE Human(
                Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
                TgId INTEGER
            );
            CREATE TABLE Reminder (
                Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
                CreationDateTime TEXT NOT NULL,
                DateTime TEXT NOT NULL,
                Title TEXT NOT NULL,
                Description TEXT,
                UserId INTEGER NOT NULL,
                FOREIGN KEY(UserId) REFERENCES Human(Id) ON DELETE CASCADE ON UPDATE CASCADE
            );";
        command.ExecuteNonQuery();
        connection.Close();
    }
}