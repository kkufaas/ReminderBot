using ReminderBot.ReminderModels.Models;
using ReminderBot.ReminderModels.Repositories;
using Microsoft.Data.Sqlite;

namespace ReminderBot.ReminderSqlite.Repositories;

public class HumanRepository : IHumanRepository
{
    private readonly string _connectionString;

    public HumanRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Add(Human entity)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = 
            @$"INSERT INTO Human(
                TgId
            ) VALUES ({entity.TgId})
            RETURNING Id;";
        var reader = command.ExecuteReader();
        reader.Read();
        entity.Id = reader.GetInt64(0);
        connection.Close();
    }

    public Human GetById(long id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = 
            @$"SELECT * FROM Human WHERE Id = {id};";
        var reader = command.ExecuteReader();
        reader.Read();
        Human entity = new()
        {
            Id = reader.GetInt64(0),
            TgId = reader.GetInt64(1)
        };
        connection.Close();
        return entity;
    }
}