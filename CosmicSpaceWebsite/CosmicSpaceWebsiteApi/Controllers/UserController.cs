using System.Linq;
using CosmicSpaceWebsiteApi.Database;
using CosmicSpaceWebsiteDll;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CosmicSpaceWebsiteApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly cosmicspaceContext dbContext;

        public UserController(ILogger<UserController> logger, cosmicspaceContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        [HttpGet("GetUsers")]
        public Users GetUser(string username, string password)
        {
            return dbContext.Users
                .SingleOrDefault(o => o.Usernamehash.Equals(username) && o.Passwordhash.Equals(password));
        }

        [HttpGet("GetPilot")]
        public Pilots GetPilot(long id, string hash)
        {
            return dbContext.Pilots
                .SingleOrDefault(o => o.Userid.Equals(id));
        }
    }
}