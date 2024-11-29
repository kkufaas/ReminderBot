using Microsoft.Extensions.Configuration.UserSecrets;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Collections.Concurrent;
using ReminderBot.ReminderModels.Repositories;
using ReminderBot.ReminderModels.Models;
using System.Text;
using ReminderBot.Extensions;
using System.Data.SqlTypes;
using ReminderBot.ReminderSqlite.Repositories;
using Telegram.Bot.Types.ReplyMarkups;
using System.Globalization;

public class Bot:IDisposable
{
    private TelegramBotClient client;

    private CancellationTokenSource cancellationTokenSource;
    public CancellationTokenSource CancellationTokenSource=>cancellationTokenSource;
    // private ConcurrentDictionary<long, BotState> states;

    // private ConcurrentDictionary<long, DateTime> dates;

    // private ConcurrentDictionary<long, string> titles;

    IRepositoryProvider repositoryProvider;

    public Bot(IRepositoryProvider repositoryProvider, string token)
    {
        cancellationTokenSource = new CancellationTokenSource();
        client = new TelegramBotClient(token, cancellationToken: cancellationTokenSource.Token);
        client.OnMessage+=OnMessage;
        // states = new ConcurrentDictionary<long, BotState>();
        // dates = new ConcurrentDictionary<long, DateTime>();
        // titles = new ConcurrentDictionary<long, string>();
        this.repositoryProvider = repositoryProvider;
    }

    public void Dispose()
    {
        cancellationTokenSource.Dispose();
    }
    // method that handle messages received by the bot:
    async Task OnMessage(Message msg, UpdateType type)
    {

        if (msg.Text is null) return;
        if (msg.From is null) return;
        long tgId = msg.From.Id;
        Console.WriteLine(tgId);
        if (!repositoryProvider.HumanDataRepository.HasData(tgId))
        {
            repositoryProvider.HumanDataRepository.CreateData(tgId);
            AssignState(tgId, BotState.WaitMain);

        }
        if (msg.Text == "/start")
        {
            await client.SendTextMessageAsync(
                chatId: msg.Chat.Id, 
                text: "Select command", 
                replyMarkup: new ReplyKeyboardMarkup(
                    new KeyboardButton[][] {
                        new KeyboardButton[] {"Get reminders", "Add reminder"}
                    }
                ){
                    ResizeKeyboard = true
                },
                cancellationToken: cancellationTokenSource.Token
            );
            AssignState(tgId, BotState.WaitMain);
        }
        else 
        {
            await ProcessState(tgId, msg.Chat.Id, msg.Text);
        }
        
        //Console.WriteLine($"Received {type} '{msg.Text}' in {msg.Chat}");
        // await client.SendTextMessageAsync(msg.Chat, $"{msg.From} said: {msg.Text}");
    }

    private async Task ProcessState(long tgId, long chatId, string text)
    {
        if (text.ToLower() == "state"){
            await client.SendTextMessageAsync(
                chatId: chatId, 
                text: GetState(tgId).ToString(), 
                cancellationToken: cancellationTokenSource.Token
            );
            return;
        }
        if (text.ToLower() == "cancel")
        {
            await client.SendTextMessageAsync(
                chatId: chatId, 
                text: "Waiting for further commands", 
                replyMarkup: new ReplyKeyboardMarkup(
                    new KeyboardButton[][] {
                        new KeyboardButton[] {"Get reminders", "Add reminder"}
                    }
                ){
                    ResizeKeyboard = true
                },
                cancellationToken: cancellationTokenSource.Token
            );
            AssignState(tgId, BotState.WaitMain);
            return;
        }

        switch (GetState(tgId))
        {
            case BotState.WaitMain:
                await ProcessWaitMain(tgId, chatId, text);
            break;
            case BotState.WaitDateToAdd:
                await ProcessWaitDateToAdd(tgId,chatId, text);
            break;
            case BotState.WaitTitle:
                await ProcessWaitTitle(tgId, chatId, text);
            break;
            case BotState.WaitBody:
                await ProcessWaitBody(tgId, chatId, text);
            break;
            case BotState.WaitDateToReturn:
                await ProcessWaitDateToReturn(tgId, chatId, text);
            break;
        }
    }

    BotState GetState (long tgId)
    {
        return Enum.Parse<BotState>(repositoryProvider.HumanDataRepository.GetState(tgId));
    }

    void AssignState (long tgId, BotState state)
    {
        repositoryProvider.HumanDataRepository.AssignState(tgId, state.ToString());

    }

    void AssignTitle (long tgId, string title)
    {
        repositoryProvider.HumanDataRepository.AssignTitle (tgId, title);
    }

    string GetTitle (long tgId)
    {
        return repositoryProvider.HumanDataRepository.GetTitle(tgId);
    }

    DateTime GetDateTime (long tgId)
    {
        return repositoryProvider.HumanDataRepository.GetDateTime(tgId);
    }

    void AssignDate (long tgId, DateTime date)
    {
        repositoryProvider.HumanDataRepository.AssignDate(tgId, date);
    }

    private async Task ProcessWaitDateToReturn(long tgId, long chatId, string text)
    {
        Human human = new Human();
        if (!repositoryProvider.HumanRepository.Exists(tgId))
        {
            human.TgId = tgId;
            repositoryProvider.HumanRepository.Add(human);
        }
        human = repositoryProvider.HumanRepository.GetByTgId(tgId);
        if (DateTime.TryParse(text, out DateTime date))
        {
            var reminders = repositoryProvider.ReminderRepository.GetAllByDateAndUser(date, human.Id);
            StringBuilder sb = new StringBuilder();
            foreach (var reminder in reminders)
            {
                sb.AppendLine(reminder.Title.Bold())
                  .AppendLine(reminder.Description)
                  .AppendLine(reminder.DateTime.ToString().Italic())
                  .AppendLine();
            }
            if (sb.Length == 0)
            {
                sb.Append("No reminders for this date");
            }
            await client.SendTextMessageAsync(
                chatId:chatId, 
                replyMarkup: new ReplyKeyboardMarkup(
                    new KeyboardButton[][] {
                        new KeyboardButton[] {"Get reminders", "Add reminder"}
                    }
                ){
                    ResizeKeyboard = true
                },    
                text: sb.ToString(), 
                cancellationToken: cancellationTokenSource.Token,
                parseMode: ParseMode.Html
            );
            AssignState(tgId, BotState.WaitMain);
        }
        else
        {
            await client.SendTextMessageAsync(
                chatId:chatId, 
                text: "Incorrect date format. Add valid date", 
                cancellationToken: cancellationTokenSource.Token
            );
        }
    }

    private async Task ProcessWaitBody(long tgId, long chatId, string text)
    {   
        if (text.ToLower() == "back")
        {
            AssignState(tgId, BotState.WaitDateToAdd);
            await client.SendTextMessageAsync(
                chatId: chatId, 
                text: "Enter event name",
                replyMarkup: new ReplyKeyboardMarkup(
                    new KeyboardButton[][] {
                        new KeyboardButton[] {"Back", "Cancel"}
                    }
                ){
                    ResizeKeyboard = true
                },
                cancellationToken: cancellationTokenSource.Token
            );
            return;
        }
        Human human = new Human();
        if (!repositoryProvider.HumanRepository.Exists(tgId))
        {
            human.TgId = tgId;
            repositoryProvider.HumanRepository.Add(human);
        }
        human = repositoryProvider.HumanRepository.GetByTgId(tgId);
        Reminder reminder = new Reminder(GetTitle(tgId), human.Id);
        reminder.DateTime = GetDateTime(tgId);
        reminder.Description = text;
        repositoryProvider.ReminderRepository.Add(reminder);
        AssignState(tgId, BotState.WaitMain);
        await client.SendTextMessageAsync(
            chatId:chatId, 
            text: "Reminder is added!", 
            replyMarkup: new ReplyKeyboardMarkup(
                    new KeyboardButton[][] {
                        new KeyboardButton[] {"Get reminders", "Add reminder"}
                    }
            ){
                ResizeKeyboard = true
            },        
            cancellationToken: cancellationTokenSource.Token
        );    
    }



    private async Task ProcessWaitTitle(long tgId, long chatId, string text)
    {
        if (text.ToLower() == "back")
        {
            AssignState(tgId, BotState.WaitDateToAdd);
            await client.SendTextMessageAsync(
                chatId: chatId, 
                text: "Enter date and time to add",
                replyMarkup: new ReplyKeyboardMarkup(
                    new KeyboardButton[][] {
                        new KeyboardButton[] {"Cancel"}
                    }
                ){
                    ResizeKeyboard = true
                },
                cancellationToken: cancellationTokenSource.Token
            );
            return;
        }
        AssignTitle(tgId, text);
        AssignState(tgId, BotState.WaitBody);
        await client.SendTextMessageAsync(
            chatId:chatId, 
            text: "Add description", 
            replyMarkup: new ReplyKeyboardMarkup(
                new KeyboardButton[][] {
                    new KeyboardButton[] {"Back", "Cancel"}
                }
            ){
                ResizeKeyboard = true
            },
            cancellationToken: cancellationTokenSource.Token
        );    
    }


    private async Task ProcessWaitDateToAdd(long tgId, long chatId, string text)
    {
        if (DateTime.TryParseExact(text, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
        {
            if (date < DateTime.Now)
            {
                await client.SendTextMessageAsync(chatId: chatId, text: "Entered date is in the past", cancellationToken: cancellationTokenSource.Token);
            }
            else
            {
                AssignDate(tgId, date);
                AssignState(tgId, BotState.WaitTitle);
                await client.SendTextMessageAsync(
                    chatId: chatId, 
                    text: "Enter event name",
                    replyMarkup: new ReplyKeyboardMarkup(
                    new KeyboardButton[][] {
                        new KeyboardButton[] {"Back", "Cancel"}
                    }
                    ){
                        ResizeKeyboard = true
                    },
                    cancellationToken: cancellationTokenSource.Token
                );
            }
        }
        else
        {
            await client.SendTextMessageAsync(
                chatId: chatId, 
                text: "Incorrect Date format, try again. Expected format: dd.MM.yyyy HH:mm",
                cancellationToken: cancellationTokenSource.Token
            );
        }
    }

    async Task ProcessWaitMain(long tgId, long chatId, string text)
    {
        if (text == "Add reminder")
        {
            AssignState(tgId, BotState.WaitDateToAdd);
            await client.SendTextMessageAsync(
                chatId: chatId, 
                text: "Enter date and time to add", 
                replyMarkup: new ReplyKeyboardMarkup(
                    new KeyboardButton[][] {
                        new KeyboardButton[] {"Cancel"}
                    }
                ){
                    ResizeKeyboard = true
                },
                cancellationToken: cancellationTokenSource.Token
            );
        }
        else if (text == "Get reminders")
        {
            AssignState(tgId, BotState.WaitDateToReturn);
            await client.SendTextMessageAsync(
                chatId: chatId, 
                text: "Enter date to get", 
                replyMarkup: new ReplyKeyboardMarkup(
                    new KeyboardButton[][] {
                        new KeyboardButton[] {"Cancel"}
                    }
                ){
                    ResizeKeyboard = true
                },
                cancellationToken: cancellationTokenSource.Token
            );
        }
        else {
            await client.SendTextMessageAsync(
                chatId: chatId, 
                text: "Incorrect request, use 'add' or'get'", 
                cancellationToken: cancellationTokenSource.Token
            );
        }
    }
}
