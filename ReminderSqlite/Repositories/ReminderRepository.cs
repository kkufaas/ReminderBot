using ReminderBot.ReminderModels.Models;
using ReminderBot.ReminderModels.Repositories;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Reflection.Metadata;

namespace ReminderBot.ReminderSqlite.Repositories;

public class ReminderRepository : IReminderRepository
{

    private readonly string _connectionString;

    public ReminderRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Add(Reminder reminder)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = 
        @$"INSERT INTO Reminder(
            CreationDateTime, DateTime, Title, Description, UserId
        ) VALUES ('{reminder.CreationDateTime.ToString("O", CultureInfo.InvariantCulture)}', 
        '{reminder.DateTime.ToString("O", CultureInfo.InvariantCulture)}', '{reminder.Title}', '{reminder.Description}', {reminder.UserId})
        RETURNING Id;";
        var reader = command.ExecuteReader();
        reader.Read();
        reminder.Id = reader.GetInt64(0);
        connection.Close();
    }


    public IEnumerable<Reminder> GetAll()
    {
        return GetByQuery(@$"SELECT * FROM Reminder;");
    }


    public IEnumerable<Reminder> GetAllByDate(DateTime date)
    {
        return GetByQuery(@$"SELECT * FROM Reminder WHERE DateTime >= '{date.ToString("O", CultureInfo.InvariantCulture)}' AND DateTime < '{date.AddDays(1).ToString("O", CultureInfo.InvariantCulture)}';");
    }

    public IEnumerable<Reminder> GetAllByDateAndUser(DateTime date, long userId)
    {
        return GetByQuery(@$"SELECT * FROM Reminder WHERE DateTime >= '{date.ToString("O", CultureInfo.InvariantCulture)}' AND DateTime < '{date.AddDays(1).ToString("O", CultureInfo.InvariantCulture)}' AND UserId = {userId};");
    }

    public IEnumerable<Reminder> GetAllByUser(long userId)
    {
        return GetByQuery(@$"SELECT * FROM Reminder WHERE UserId = {userId};");
    }

    private IEnumerable<Reminder> GetByQuery(string query)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = query;
        var reader = command.ExecuteReader();
        var result = ExtractFrom(reader);
        connection.Close();
        return result;
    }

    private IEnumerable<Reminder> ExtractFrom(SqliteDataReader reader)
    {
        List<Reminder> reminders = new List<Reminder>();
        while (reader.Read())
        {
            Reminder entity = new Reminder(reader.GetString(4), reader.GetInt64(1));
            entity.Id = reader.GetInt64(0);
            entity.CreationDateTime = DateTime.Parse(reader.GetString(2), CultureInfo.InvariantCulture);
            entity.DateTime = DateTime.Parse(reader.GetString(3), CultureInfo.InvariantCulture);
            entity.Description = reader.GetString(5);
            reminders.Add(entity);
        }
        return reminders;
    }
}