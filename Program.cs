using Microsoft.VisualBasic;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botClient = new TelegramBotClient("5492951142:AAHOqs9esWnt4omwKk47XVHIURiR-Z54AZo");

using CancellationTokenSource cts = new();


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
    if (update.Message.Type == MessageType.Sticker)
    {
        await botClient.SendStickerAsync(
        chatId: update.Message.Chat.Id,
        sticker: update.Message.Sticker.FileId,
        cancellationToken: cancellationToken);
        return;
    }

    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    Console.WriteLine($"Received a '{message.Text}' message in chat {chatId}.");

    var people = new Dictionary<string, string>()
    {
        { "Привет" , "Привет" },
        { "Как дела" , "Супер" },
        { "Как ты" , "Все хорошо" }
    };

    string messageAnswerText = "";
    switch (message.Text)
    {
        case "Привет":
            messageAnswerText = "Привет";
            break;
        case "Как дела?":
            messageAnswerText = "Супер, а у тебя?";
            break;
        default:
            await botClient.SendPhotoAsync(
                chatId: chatId,
                photo: "https://www.meme-arsenal.com/memes/fa3ede4abb27c0f923e46373adf548b8.jpg",
                cancellationToken: cancellationToken);
            return;
    }

    // Echo received message text
    Message sentMessage = await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: messageAnswerText,
        cancellationToken: cancellationToken);
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
