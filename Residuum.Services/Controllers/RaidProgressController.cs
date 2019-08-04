using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Residuum.Services.Database;
using Residuum.Services.Entities;

namespace Residuum.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RaidProgressController : ControllerBase
    {
        private readonly List<string> IgnoreList = new List<string>()
        {
            "antorus-the-burning-throne",
            "the-emerald-nightmare",
            "the-nighthold",
            "tomb-of-sargeras",
            "trial-of-valor"
        };

        private readonly DataCache _cache;

        public RaidProgressController(CacheContext context)
        {
            _cache = new DataCache(context);
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var progression = await _cache.GetRaidProgress();

            var filteredProgression = FilterProgression(progression);

            Response.Headers.Add("Access-Control-Allow-Origin", "*");

            return JsonConvert.SerializeObject(filteredProgression);
        }

        private List<OutputRaidInfo> FilterProgression(RaidProgress progression)
        {
            var selectedRaids = progression.RaidInfo.Where(raid => !IgnoreList.Contains(raid.RaidName));

            if (ServiceConfiguration.OverrideRaidProgressionSummary)
            {
                ReplaceDefaultSummarySectionWithHeroicProgression(selectedRaids);
            }

            return selectedRaids.Select(raid => new OutputRaidInfo { Key = raid.RaidName, Summary = raid.Details.Summary }).ToList();
        }

        private static void ReplaceDefaultSummarySectionWithHeroicProgression(IEnumerable<Progression> selectedRaids)
        {
            foreach (Progression selectedRaid in selectedRaids)
            {
                if (selectedRaid.Details.HeroicBossesKilled != 0)
                {
                    selectedRaid.Details.Summary = $"{selectedRaid.Details.HeroicBossesKilled}/{selectedRaid.Details.TotalBosses} H";
                }               
            }
        }

        internal class OutputRaidInfo
        {
            public string Key { get; set; }

            public string Summary { get; set; }
        }
    }
}