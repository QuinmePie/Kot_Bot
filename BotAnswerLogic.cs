using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

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

        public async Task ReceiveAsync ( Update update, CancellationToken cancellationToken)
        {

        }


    }
}
