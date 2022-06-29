using MyApi.Clients;
using MyApi.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Exceptions;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace MyApi.Clients
{
    public class TelClient
    {
        public static string? LastMessage;
        public static int? Count;
        public static string? title;
        TelegramBotClient botClient = new TelegramBotClient(Const.telegramBotToken);
        CancellationToken cancellationToken = new CancellationToken();
        ReceiverOptions receiverOptions = new ReceiverOptions {AllowedUpdates = { }};

        public async Task Start()
        {
            botClient.StartReceiving(HandlerUpdateSync, HandlerErrorAsync, receiverOptions, cancellationToken);
            var Botme = await botClient.GetMeAsync();
            Console.WriteLine($"The Bot {Botme.Username} starts working!");
            Console.ReadKey();
        }

        private Task HandlerErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException =>
                    $"Error in the Telegram Bot API:\n {apiRequestException.ErrorCode}" +
                    $"\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        private async Task HandlerUpdateSync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandlerMessageAsync(botClient, update.Message);
            }
        }

        private async Task HandlerMessageAsync(ITelegramBotClient botClient, Message message)
        {
            string imagePath;
            if (message.Text == "/start")
            {

                await botClient.SendTextMessageAsync(message.Chat.Id,
                    $"Type '/description' to see bot`s description or type '/functions' to see fuctions of bot");
                return;
            }
            else if (message.Text == "/description")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "This bot can help you find some info about japanese books and can help you store it in your favourites. Give it a try!");
                return;
            }
            else if (message.Text == "/functions")
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new
                (
                    new[]
                    {
                        new KeyboardButton[]{ "Find By Id","Trending"},
                        new KeyboardButton[]{ "Show Fav`s" },
                        new KeyboardButton[]{ "Add to Favourites","Delete from Favourites"},     
                    }
                )
                {
                    ResizeKeyboard = true
                };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Choose a function", replyMarkup: replyKeyboardMarkup);
            }
            else if (message.Text == "Find By Id")
            {
                LastMessage = "Find By Id";
                await botClient.SendTextMessageAsync(message.Chat.Id, "Write an id:");
                return;
            }
            else if (message.Text == "Trending")
            {
                LastMessage = "Trending";
                MangaClient client = new MangaClient();
                TrendingModel list = new TrendingModel();
                list = client.GetTrending().Result;
                Count = client.GetTrending().Result.data.Count;
                string mangaa;
                for (int i = 0; i < Count; i++)
                {
                    imagePath = $"{list.data[i].attributes.posterImage.original}";
                    mangaa = $"\nTitle: {list.data[i].attributes.canonicalTitle}\nID: {list.data[i].id}\nDescription: {list.data[i].attributes.description}"+
                             $"\nStart date:{list.data[i].attributes.startDate}\nEnd Date: {list.data[i].attributes.endDate}\nStatus: {list.data[i].attributes.status}\n"+
                             $"Chapters: {list.data[i].attributes.chapterCount}\nImage: {imagePath}";
                    await botClient.SendTextMessageAsync(message.Chat.Id, mangaa);
                }
                return;
               
            }
            else if (message.Text == "Add to Favourites")
            {
                LastMessage = "Add to Favourites";
                await botClient.SendTextMessageAsync(message.Chat.Id, "Write an id to add:");
                return;
            }
            else if (message.Text == "Delete from Favourites")
            {
                LastMessage = "Delete from Favourites";
                await botClient.SendTextMessageAsync(message.Chat.Id, "Write an id to del:");
                return;
            }
            else if (message.Text == "Show Fav`s")
            {
                LastMessage = "Show Fav`s";
                MangaClient client = new MangaClient();
                List<MangaById> list = new List<MangaById>();
                list = client.FindFav(message.Chat.Id).Result;
                Count = list.Count;
                string mangaa;
                if (Count == null)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "You have no favs");
                }
                for (int i = 0; i < Count; i++)
                {
                    imagePath = $"{list[i].data.attributes.posterImage.original}";
                    mangaa = $"\nTitle: {list[i].data.attributes.canonicalTitle}\nID: {list[i].data.id}\nDescription: {list[i].data.attributes.description}"+
                             $"\nStart date:{list[i].data.attributes.startDate}\nEnd Date: {list[i].data.attributes.endDate}\nStatus: {list[i].data.attributes.status}\n"+
                             $"Chapters: {list[i].data.attributes.chapterCount}\nImage: {imagePath}";
                    await botClient.SendTextMessageAsync(message.Chat.Id, mangaa);
                }

                await botClient.SendTextMessageAsync(message.Chat.Id, "Here`s all favs");
                return;
            }
            else if (message.Text != null)
            {
                _ = ButtonAbilities(botClient, message);
                return;
            }
        }
        public async Task ButtonAbilities(ITelegramBotClient botClient, Message message)
        {
            string imagePath = null;
            string title = message.Text;
            int id = int.Parse(title);
            //if (Regex.IsMatch(message.Text, "[0-9]")){
                if (LastMessage == "Find By Id")
                {
                    MangaClient client = new MangaClient();
                    var MangaChar = client.GetMangaByid(id).Result;

                    string json = System.Text.Json.JsonSerializer.Serialize<MangaById>(MangaChar);
                    MangaById? mangas = System.Text.Json.JsonSerializer.Deserialize<MangaById>(json);
                    string manga = "";
                    imagePath = $"{mangas.data.attributes.posterImage.original}";
                    manga =
                        $"\nTitle: {mangas.data.attributes.canonicalTitle}\nID: {mangas.data.id}\nDescription: {mangas.data.attributes.description}" +
                        $"\nStart date:{mangas.data.attributes.startDate}\nEnd Date: {mangas.data.attributes.endDate}\nStatus: {mangas.data.attributes.status}\n" +
                        $"Chapters: {mangas.data.attributes.chapterCount}\nImage: {imagePath}";
                    if (manga == "")
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Does not exists");
                        return;
                    }

                    await botClient.SendTextMessageAsync(message.Chat.Id, manga);
                    return;
                }

                if (LastMessage == "Add to Favourites")
                {
                    MangaClient client = new MangaClient();
                    await client.PostMangaId(id, message.Chat.Id);
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Successfully added to Favours");
                    return;
                }

                if (LastMessage == "Delete from Favourites")
                {
                    MangaClient client = new MangaClient();
                    await client.DeleteFavManga(id, message.Chat.Id);
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Successfully deleted from Favours");
                    return;
                }
            //}
            //else
            //{
            //    await botClient.SendTextMessageAsync(message.Chat.Id, "Type your id correctly. Please, proceed to the start"); 
            //}
        }
        
    }
}

