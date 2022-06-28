using Microsoft.AspNetCore.Mvc;
using MyApi.Models;
using MyApi.Clients;
using MongoDB.Bson;
using MongoDB.Driver;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FindFavManga : ControllerBase
    {
        [HttpGet]
        public Task<List<MangaById>> FindFavour(long namedb)
        {
            MangaClient client = new MangaClient();
            return client.FindFav(namedb);
        }
    }
}
