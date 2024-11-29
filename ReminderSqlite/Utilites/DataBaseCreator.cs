using System.Text;
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

    public void Run(bool pooling = true)
    {
        string connectionString = $"Data Source={_dbName};Foreign Keys=True;Pooling={pooling};";
        if (_password is not null)
        {
            connectionString += $"Password={_password};";
        }
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = 
            @"CREATE TABLE IF NOT EXISTS Human(
                Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
                TgId INTEGER
            );
            CREATE TABLE IF NOT EXISTS Reminder (
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