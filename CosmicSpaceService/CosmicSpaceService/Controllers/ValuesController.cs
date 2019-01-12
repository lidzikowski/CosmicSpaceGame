using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmicSpaceService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CosmicSpaceService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<object>> Get()
        {
            using (var ctx = new CosmicSpaceContext())
            {
                //User user = new User()
                //{
                //    Username = "login",
                //    Password = "haslo",
                //    Email = "adres email",
                //    EmailNewsletter = false,
                //    AcceptRules = true
                //};
                //ctx.Users.Add(user);
                //ctx.SaveChanges();

                //List<Ammunition> ammunitions = new List<Ammunition>();
                //for (int i = 0; i < 10; i++)
                //{
                //    ammunitions.Add(new Ammunition()
                //    {
                //        Name = "ammunition" + i,

                //    });
                //}
                //ctx.Ammunitions.AddRange(ammunitions);
                //ctx.SaveChanges();

                //Random random = new Random();

                //ctx.Pilots.Add(new Pilot()
                //{
                //    UserId = user.UserId,
                //    Nickname = "test",
                //    Map = new Map()
                //    {
                //        Name = "map" + ctx.Maps.Count()
                //    },
                //    Ship = new Ship()
                //    {
                //        Name = "ship" + ctx.Ships.Count(),
                //        Reward = new Reward()
                //    },
                //    Ammunitions = new List<AmmunitionPilot>
                //    {
                //        new AmmunitionPilot()
                //        {
                //            PilotId = user.UserId,
                //            Ammunition = ammunitions[random.Next(0, ammunitions.Count-1)],
                //            Count = random.Next(0, 100)
                //        }
                //    }
                //});
                //ctx.SaveChanges();

                Pilot pilot = ctx.Pilots
                    .Include(o => o.Map)
                    .Include(o => o.Ship)
                    .Include(o => o.Ammunitions)
                    .FirstOrDefault(o => o.UserId.Equals(1));
                
                return new object[] { pilot };
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
