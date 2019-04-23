using Newtonsoft.Json;

namespace Residuum.Services.Entities
{
    public class Progression
    {
        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("total_bosses")]
        public int TotalBosses { get; set; }

        [JsonProperty("normal_bosses_killed")]
        public int NormalBossesKilled { get; set; }

        [JsonProperty("heroic_bosses_killed")]
        public int HeroicBossesKilled { get; set; }

        [JsonProperty("mythic_bosses_killed")]
        public int MythicBossesKilled { get; set; }
    }
}