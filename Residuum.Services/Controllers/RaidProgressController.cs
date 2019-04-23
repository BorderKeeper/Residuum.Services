using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        [HttpGet]
        public async Task<string> Get()
        {
            var progression = await DataAccessLayer.GetRaidProgress();

            var filteredProgression = FilterProgression(progression);

            Response.Headers.Add("Access-Control-Allow-Origin", "*");

            return JsonConvert.SerializeObject(filteredProgression);
        }

        private List<OutputRaidInfo> FilterProgression(RaidProgress progression)
        {
            var selectedRaids = progression.RaidInfo.Where(raid => !IgnoreList.Contains(raid.Key));

            if (ServiceConfiguration.OverrideRaidProgressionSummary)
            {
                ReplaceDefaultSummarySectionWithHeroicProgression(selectedRaids);
            }

            return selectedRaids.Select(raid => new OutputRaidInfo { Key = raid.Key, Summary = raid.Value.Summary }).ToList();
        }

        private static void ReplaceDefaultSummarySectionWithHeroicProgression(IEnumerable<KeyValuePair<string, Progression>> selectedRaids)
        {
            foreach (KeyValuePair<string, Progression> selectedRaid in selectedRaids)
            {
                if (selectedRaid.Value.HeroicBossesKilled != 0)
                {
                    selectedRaid.Value.Summary = $"{selectedRaid.Value.HeroicBossesKilled}/{selectedRaid.Value.TotalBosses} H";
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