using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

Bot bot = new Bot(new RepositoryProviderSqlite("testDb"), "6682435246:AAEZeD6id2HFIiOkkZiKWEzhq5APrOhYO4E");
Console.ReadLine();
bot.CancellationTokenSource.Cancel(); // stop the bot
