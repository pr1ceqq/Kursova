using Microsoft.AspNetCore.Mvc;
using MyApi.Models;
using MyApi.Clients;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MangaPostController : ControllerBase
    {
        // POST api/<MangaController>
        [HttpPost]
        public void PushDictionary(int id, long namebd)
        {
            MangaClient client = new MangaClient();
            client.PostMangaId(id, namebd);
        }
    }
}
