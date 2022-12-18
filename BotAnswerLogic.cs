using Bot_kot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot_kot
{
    internal class BotAnswerLogic
    {
        private readonly TelegramBotClient telegramBotClient;
        private readonly Dictionary<string, ActionModel> botAnswerDictionary;
        public BotAnswerLogic(TelegramBotClient telegramBotClient, Dictionary<string, ActionModel> botAnswerDictionary)
        {
            this.telegramBotClient = telegramBotClient;
            this.botAnswerDictionary = botAnswerDictionary;
        }

        public async Task ReceiveAsync(Update update, CancellationToken cancellationToken)
        {

            if (update.Message.Type == MessageType.Sticker)
            {
                var stickerSet = await telegramBotClient.GetStickerSetAsync(update.Message.Sticker.SetName);
                var stickers = stickerSet.Stickers;
                Random random = new Random();
                var selectSticker = stickers[random.NextInt64(0, stickers.Length)];
                Console.WriteLine(update.Message.Sticker.FileId);
                await telegramBotClient.SendStickerAsync(
                chatId: update.Message.Chat.Id,
                sticker: selectSticker.FileId,
                cancellationToken: cancellationToken);
                return;
            }
            ActionModel answer = null;
            string textMessage = update.Message.Text.ToLower().Trim();
            foreach (var value in botAnswerDictionary.Keys)
            {
                if (textMessage.Contains(value))
                {
                    answer = botAnswerDictionary.GetValueOrDefault(value);
                    break;
                };
            }
            if (answer == null)
            {
                var emojiService = new Emoji();
                var rand = new Random();
                string randomEmogi = emojiService.EmojiList
                    .ElementAt(rand.Next(0, emojiService.EmojiList.Count())).Code;
                await telegramBotClient.SendTextMessageAsync(
                  chatId: update.Message.Chat.Id,
                  text: randomEmogi,
                  cancellationToken: cancellationToken);
                return;
            }
            else
            {
                AnswerAsync(answer, update);
            }
        }

        private async Task AnswerAsync(ActionModel actionModel, Update update)
        {
            switch (actionModel.ActionType)
            {
                case ActionTypes.AnswerMessage:
                    await telegramBotClient.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: actionModel.Value);
                    break;
                case ActionTypes.AnswerStiker:
                    await telegramBotClient.SendStickerAsync(
                    chatId: update.Message.Chat.Id,
                    sticker: actionModel.Value);
                    break;
                case ActionTypes.AnswerPicture:
                    await telegramBotClient.SendPhotoAsync(
                    chatId: update.Message.Chat.Id,
                    photo: actionModel.Value);
                    break;
                case ActionTypes.AnswerRandomStiker:
                    var stickerSet = await telegramBotClient.GetStickerSetAsync(actionModel.Value);
                    var stickers = stickerSet.Stickers;
                    Random random = new Random();
                    var selectSticker = stickers[random.NextInt64(0, stickers.Length)];
                    await telegramBotClient.SendStickerAsync(
                    chatId: update.Message.Chat.Id,
                    sticker: selectSticker.FileId);
                    break;
                case ActionTypes.AnswerEmoji:

                default:
                    break;
            }
        }
    }
}
