﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArgentPonyWarcraftClient;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Residuum.Services.QueryHandlers;
using Character = Residuum.Services.Entities.Character;

namespace Residuum.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]   
    public class GuildRosterController : ControllerBase
    {
        private const int RaiderRank = 4;

        [HttpGet]
        public async Task<string> Get()
        {
            var nonAltsGuildMembers = await GetGuildMembers();

            var guildMembers = new List<Character>();

            var raiderClient = new RaiderClient();

            foreach (GuildMember member in nonAltsGuildMembers)
            {
                var bestMythic = await DataAccessLayer.GetBestMythic(member.Character.Realm, member.Character.Name);

                guildMembers.Add(new Character
                {
                    Name = member.Character.Name,
                    Class = member.Character.Class.ToString(),
                    Rank = member.Rank,
                    BestMythic = bestMythic
                });
            }

            Response.Headers.Add("Access-Control-Allow-Origin", "*");

            return JsonConvert.SerializeObject(guildMembers);
        }

        private async Task<IEnumerable<GuildMember>> GetGuildMembers()
        {
            RequestResult<Guild> guild = await DataAccessLayer.GetGuild();

            var nonAltsGuildMembers = guild.Value.Members.Where(member => member.Rank <= RaiderRank);

            return nonAltsGuildMembers;
        }
    }
}