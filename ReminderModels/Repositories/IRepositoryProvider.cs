namespace ReminderBot.ReminderModels.Repositories;

public interface IRepositoryProvider
{
    IReminderRepository ReminderRepository { get; }
    IHumanRepository HumanRepository { get; }
    IHumanDataRepository HumanDataRepository{ get; }

}