using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Residuum.Services.Entities
{
    public class RaidProgress
    {
        public RaidProgress()
        {
            LastUpdated = DateTime.Now;
        }

        [Key]
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("faction")]
        public string Faction { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("realm")]
        public string Realm { get; set; }

        [JsonProperty("profile_url")]
        public string URL { get; set; }

        [JsonProperty("raid_progression")]
        public List<Progression> RaidInfo { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}