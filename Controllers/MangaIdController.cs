using Microsoft.AspNetCore.Mvc;
using MyApi.Models;
using MyApi.Clients;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MangaIdController : ControllerBase
    {
        

        // GET api/<MangaController>/5
        [HttpGet("{id}")]
        public MangaById GetMangaById(int id)
        {
            MangaClient client = new MangaClient();
            return client.GetMangaByid(id).Result;
        }
    }
}
