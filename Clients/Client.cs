using MyApi.Models;
using Newtonsoft.Json;
using MongoDB.Bson;
using MongoDB.Driver;
namespace MyApi.Clients
{
    public class MangaClient
    {
        private HttpClient _client;
        private static string _address;
        string connectionString = "mongodb+srv://@favorites.i5xkzfp.mongodb.net/?retryWrites=true&w=majority";
        string databaseName = "favourites";
        public MangaClient()
        {
            _address = Const.adress;

            _client = new HttpClient();
            _client.BaseAddress = new Uri(_address);
        }
        public async Task<MangaById> GetMangaByid(int id)
        {
            var response = await _client.GetAsync($"manga/{id}");
            response.EnsureSuccessStatusCode();
            string responseData = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<MangaById>(responseData);
            return result;
        }
        public async Task<TrendingModel> GetTrending()
        {
            var response = await _client.GetAsync($"trending/manga");
            response.EnsureSuccessStatusCode();
            string responseData = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TrendingModel>(responseData);
            return result;
        }
        public async Task<MangaById> PostMangaId(int id, long namedb)
        {

            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var mongoclient = new MongoClient(settings);
            var database = mongoclient.GetDatabase(databaseName);
            long collectionName = namedb;
            string colName = collectionName.ToString();
            MangaClient client = new MangaClient();
            
            var collection = database.GetCollection<MangaById>(colName);

            MangaById favMan = client.GetMangaByid(id).Result;
            var filter = Builders<MangaById>.Filter.Eq("data.id", favMan.data.id);
            bool exists = await collection.Find(_ => _.data.id == favMan.data.id).AnyAsync();
            if (!exists && favMan.data.id != 0)
            {
                await collection.InsertOneAsync(favMan);
            }
            return favMan;
        }
        public async Task<List<MangaById>> FindFav(long namedb)
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var mongoclient = new MongoClient(settings);
            var database = mongoclient.GetDatabase(databaseName);
            MangaClient client = new MangaClient();
            long collectionName = namedb;
            string colName = collectionName.ToString();
            var collection = database.GetCollection<FavManga>(colName);
            var result = await collection.FindAsync(_ => true);

            List<MangaById> res = new List<MangaById>();

            foreach (var item in result.ToList())
            {
                res.Add(client.GetMangaByid(item.data.id).Result);
            }
            return res;
        }
        public async Task DeleteFavManga(int id, long namedb)
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var mongoclient = new MongoClient(settings);
            var database = mongoclient.GetDatabase(databaseName);
            
            long collectionName = namedb;
            string colName = collectionName.ToString();
            MangaClient client = new MangaClient();
            MangaById favManga = client.GetMangaByid(id).Result;
            var collection = database.GetCollection<MangaById>(colName);
            var filter = Builders<MangaById>.Filter.Eq("data.id", favManga.data.id);
            collection.DeleteOneAsync(filter);
        }

    }
}

