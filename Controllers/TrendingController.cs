using Microsoft.AspNetCore.Mvc;
using MyApi.Models;
using MyApi.Clients;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrendingController : ControllerBase
    {
        [HttpGet]
        public void GetTrending()
        {
            MangaClient client = new MangaClient();
            client.GetTrending();
        }
    }
}
