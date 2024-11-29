using ReminderBot.ReminderModels.Repositories;
using ReminderBot.ReminderSqlite.Repositories;
using Microsoft.Data.Sqlite;

public class RepositoryProviderSqlite : IRepositoryProvider
{
    private string _dbName;
    private string? _password;

    public RepositoryProviderSqlite(string dbName, bool pooling = true, string? password = null)
    {
        _dbName = dbName;
        _password = password;

        string connectionString = $"Data Source={_dbName};Foreign Keys=True;Pooling={pooling};";
        if (_password is not null)
        {
            connectionString += $"Password={_password};";
        }
        CreateTables(connectionString);

        HumanRepository = new HumanRepository(connectionString);
        ReminderRepository = new ReminderRepository(connectionString);
        HumanDataRepository = new HumanDataRepository(connectionString);
    }

    public IReminderRepository ReminderRepository 
    {
        get; private set;
    }

    public IHumanRepository HumanRepository
    {
        get; private set;
    }

    public IHumanDataRepository HumanDataRepository
    {
        get; private set;
    }

    private void CreateTables (string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = 
            @"CREATE TABLE IF NOT EXISTS Human (
                Id INTEGER PRIMARY KEY, 
                TgId INTEGER UNIQUE
            );
            CREATE TABLE IF NOT EXISTS Reminder (
                Id INTEGER PRIMARY KEY, 
                UserId INTEGER NOT NULL, 
                CreationDateTime TEXT NOT NULL, 
                DateTime TEXT NOT NULL, 
                Title TEXT NOT NULL, 
                Description TEXT NOT NULL, 
                FOREIGN KEY (UserId) REFERENCES Human (Id)
            );
            CREATE TABLE IF NOT EXISTS Data (
                Title TEXT NOT NULL, 
                TgId INTEGER UNIQUE, 
                State TEXT NOT NULL, 
                DateTime TEXT NOT NULL
            );
            ";
        command.ExecuteNonQuery();
    }
}
