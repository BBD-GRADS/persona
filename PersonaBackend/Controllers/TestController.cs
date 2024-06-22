using Microsoft.AspNetCore.Mvc;
using PersonaBackend.Authentication;
using PersonaBackend.Database.IRepositories;
using PersonaBackend.Database.Models;

namespace PersonaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IUserRepository userRepository;

        public TestController(IUserRepository usersRepository)
        {
            this.userRepository = usersRepository;
        }

        [HttpPost("Impregnate")]
        [ServiceAuthorization("HandOfZeus")]
        public async Task<IActionResult> Impregnate()
        {
            return Ok("Impregnated");
        }

        [HttpPost("GiveMoney")]
        [ServiceAuthorization("RetailBank")]
        public async Task<IActionResult> GiveMoney()
        {
            return Ok("you got money");
        }

        [HttpGet("Test")]
        [ServiceAuthorization("RetailBank", "HandOfZeus")]
        public async Task<IActionResult> Test()
        {
            return Ok("test");
        }

        [HttpGet("NoAuth")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserModel>))]
        public IActionResult NoAuth()
        {
            var users = userRepository.GetUsers();

            return Ok(users);
        }
    }
}