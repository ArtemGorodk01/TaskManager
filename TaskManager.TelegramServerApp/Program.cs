using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using TaskManager.TelegramServerApp;

namespace TelegramBotTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("config.json")
                    .Build();
                var server = new Server(config.GetSection("TelegramApiToken").Value);
                server.Start();
                Console.ReadLine();
                server.Stop();
            }
            catch
            {
                Console.WriteLine("Error while starting application.");
            }
        }
    }
}