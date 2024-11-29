using ReminderBot.ReminderModels.Models;
namespace ReminderBot.ReminderModels.Repositories;

public interface IHumanRepository
{
    Human GetById (long id);
    Human GetByTgId (long tgId);
    void Add(Human entity);
    bool Exists (long tgId);
}