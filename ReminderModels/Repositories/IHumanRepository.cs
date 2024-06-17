using ReminderBot.ReminderModels.Models;
namespace ReminderBot.ReminderModels.Repositories;

public interface IHumanRepository
{
    Human GetById (long id);
    void Add(Human entity);

}