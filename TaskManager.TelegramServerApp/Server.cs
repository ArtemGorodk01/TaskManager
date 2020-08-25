using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TaskManager.TelegramServerApp
{
    public class Server
    {
        private readonly TelegramBotClient telegramBotClient;
        private Dictionary<long, DialogHandler> chatHandlers =
            new Dictionary<long, DialogHandler>();
        private DialogHandler defaultDialogHandler;

        public Server(string token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            telegramBotClient = new TelegramBotClient(token);
            ConfigureTelegramBotClient();
        }

        public void Start()
        {
            telegramBotClient.StartReceiving();
        }

        public void Stop()
        {
            telegramBotClient.StartReceiving();
        }

        private void ConfigureTelegramBotClient()
        {
            telegramBotClient.OnMessage += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            ThreadPool.QueueUserWorkItem(async o =>
            {
                var chatId = messageEventArgs.Message.Chat.Id;
                if (!chatHandlers.ContainsKey(chatId))
                {
                    chatHandlers.Add(chatId, CreateDefaultDialogHandler());
                }

                await chatHandlers[chatId].Handle(messageEventArgs.Message);
            });
        }

        private DialogHandler CreateDefaultDialogHandler()
        {
            return new DialogHandler()
                .AddFlow("/AddPlan", async m =>
                {
                    if (!m.Text.Equals("/AddPlan"))
                    {
                        return false;
                    }

                    await telegramBotClient.SendTextMessageAsync(
                        chatId: m.Chat.Id,
                        text: "Enter list of actions, please."
                        );
                    return true;
                })
                .AddMessageHandler(async m =>
                {
                    if (m.Text != "next")
                    {
                        return false;
                    }

                    await telegramBotClient.SendTextMessageAsync(
                        chatId: m.Chat.Id,
                        text: "Well done."
                        );
                    return true;
                })
                .AddMessageHandler(async m =>
                {
                    //TO DO process listof actions
                    await telegramBotClient.SendTextMessageAsync(
                        chatId: m.Chat.Id,
                        text: "Well done."
                        );
                    return true;
                });
        }
    }
}
