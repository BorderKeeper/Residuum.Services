using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Residuum.Services.Database;
using Residuum.Services.Entities;
using Residuum.Services.QueryHandlers;
using Character = Residuum.Services.Entities.Character;

namespace Residuum.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]   
    public class GuildRosterController : ControllerBase
    {
        private DataCache _cache;

        private const int RaiderRank = 6;

        public GuildRosterController(CacheContext context)
        {
            _cache = new DataCache(context);
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var nonAltsGuildMembers = await GetGuildMembers();

            var guildMembers = new List<Character>();

            var raiderClient = new RaiderClient();

            foreach (GuildMember member in nonAltsGuildMembers)
            {
                var bestMythic = await _cache.GetBestMythic(member.Realm, member.Name);

                guildMembers.Add(new Character
                {
                    Name = member.Name,
                    Class = member.Class,
                    Rank = member.Rank,
                    BestMythic = bestMythic
                });
               
            }

            guildMembers = guildMembers.OrderByDescending(member => member.BestMythic.Difficulty).ToList();

            Response.Headers.Add("Access-Control-Allow-Origin", "*");

            return JsonConvert.SerializeObject(guildMembers);
        }

        private async Task<IEnumerable<GuildMember>> GetGuildMembers()
        {
            List<GuildMember> guildMembers = (await _cache.GetGuildMembers()).ToList();

            var nonAltsGuildMembers = guildMembers.Where(member => member.Rank <= RaiderRank);

            return nonAltsGuildMembers;
        }
    }
}