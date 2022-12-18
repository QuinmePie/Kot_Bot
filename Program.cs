using Bot_kot;
using Bot_kot.Models;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botClient = new TelegramBotClient("5492951142:AAHOqs9esWnt4omwKk47XVHIURiR-Z54AZo");

using CancellationTokenSource cts = new();
var BotKnowledge = new Dictionary<string, ActionModel>()
    {
        { "привет" , new ActionModel { ActionType = ActionTypes.AnswerMessage, Value = " Привет! "} },
        { "как дела" , new ActionModel{ ActionType = ActionTypes.AnswerMessage, Value = " Супер " } },
        { "как ты" , new ActionModel{ ActionType = ActionTypes.AnswerMessage, Value = " Хорошо " } },
        { "пришли кота", new ActionModel { ActionType = ActionTypes.AnswerRandomStiker, Value = "JackalCats" } },
        { "пришли кошку", new ActionModel { ActionType = ActionTypes.AnswerRandomStiker, Value = "thatfuckingcats" } },
        { "пришли собаку", new ActionModel { ActionType = ActionTypes.AnswerStiker, Value = "CAACAgIAAxkBAAM0Y5Y1nD6febO73NUGMOztiGMlZ2UAAqsQAAItXDlLYNmwIzwm6qorBA" } },
        { "пришли песика", new ActionModel { ActionType = ActionTypes.AnswerStiker, Value = "CAACAgIAAxkBAAM0Y5Y1nD6febO73NUGMOztiGMlZ2UAAqsQAAItXDlLYNmwIzwm6qorBA" } },
        { "пришли киску", new ActionModel { ActionType = ActionTypes.AnswerPicture, Value = "https://www.meme-arsenal.com/memes/fa3ede4abb27c0f923e46373adf548b8.jpg" } },
        { "скинь киску", new ActionModel { ActionType = ActionTypes.AnswerPicture, Value = "https://www.meme-arsenal.com/memes/fa3ede4abb27c0f923e46373adf548b8.jpg" } },
        { "покажи киску", new ActionModel { ActionType = ActionTypes.AnswerPicture, Value = "https://www.meme-arsenal.com/memes/fa3ede4abb27c0f923e46373adf548b8.jpg" } }
};

var TelegramBotLogic = new BotAnswerLogic(botClient, BotKnowledge);
// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    Console.WriteLine($"Received a '{update.Message.Text}' message in chat {update.Message.Chat.Id}.");
    await TelegramBotLogic.ReceiveAsync(update, cancellationToken);
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}
