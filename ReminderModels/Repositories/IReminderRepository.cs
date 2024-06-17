using System.Collections;
using ReminderBot.ReminderModels.Models;

namespace ReminderBot.ReminderModels.Repositories;

public interface IReminderRepository
{
    IEnumerable<Reminder> GetAll();
    IEnumerable<Reminder> GetAllByDate(DateTime date);
    IEnumerable<Reminder> GetAllByDateAndUser(DateTime date, long userId);
    IEnumerable<Reminder> GetAllByUser(long userId);
    void Add(Reminder reminder);
}