using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot_kot
{
    internal class BotAnswerLogic
    {
        private readonly TelegramBotClient telegramBotClient;
        private readonly Dictionary<string, string> botAnswerDictionary;
        public BotAnswerLogic(TelegramBotClient telegramBotClient, Dictionary<string, string> botAnswerDictionary)
        {
            this.telegramBotClient = telegramBotClient;
            this.botAnswerDictionary = botAnswerDictionary;
        }

        public async Task ReceiveAsync(Update update, CancellationToken cancellationToken)
        {
            if (update.Message.Type == MessageType.Sticker)
            {
                await telegramBotClient.SendStickerAsync(
                chatId: update.Message.Chat.Id,
                sticker: update.Message.Sticker.FileId,
                cancellationToken: cancellationToken);
                return;
            }
            string answer = "";
            string textMessage = update.Message.Text.ToLower().Trim();
            foreach (var value in botAnswerDictionary.Keys)
            {
                if (textMessage.Contains(value))
                {
                    answer = botAnswerDictionary.GetValueOrDefault(value);
                    break;
                };
            }
            if (string.IsNullOrEmpty(answer))
            {
                await telegramBotClient.SendPhotoAsync(
                chatId: update.Message.Chat.Id,
                photo: "https://www.meme-arsenal.com/memes/fa3ede4abb27c0f923e46373adf548b8.jpg",
                cancellationToken: cancellationToken);
                return;
            }
            else
            {
                await telegramBotClient.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: answer,
                cancellationToken: cancellationToken);
            }
        }


    }
}
