using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Residuum.Services.Entities;

namespace Residuum.Services.QueryHandlers
{
    public class RaiderClient
    {
        public async Task<RaidProgress> GetGuildProgress(string realmName, string guildName)
        {
            var fieldName = "raid_progression";
            var url = $"https://raider.io/api/v1/guilds/profile?region=eu&realm={realmName}&name={guildName}&fields={fieldName}";

            return await RetrieveData<RaidProgress>(url);
        }

        public async Task<Mythic> GetBestMythic(string realmName, string playerName)
        {
            var fieldName = "mythic_plus_highest_level_runs";
            var url = $"https://raider.io/api/v1/characters/profile?region=eu&realm={realmName}&name={playerName}&fields={fieldName}";

            try
            {
                var data = await RetrieveData<MythicRaiderData>(url);

                var bestMythicRun = data.BestMythicRuns.Any() ? data.BestMythicRuns.First() : Mythic.EmptyMythic;
                bestMythicRun.ProfileUri = data.ProfileUri;

                return bestMythicRun;
            }
            catch (HttpRequestException ex)
            {
                return Mythic.EmptyMythic;
            }
        }

        private async Task<T> RetrieveData<T>(string baseUrl)
            => await Task.Run(async () => JsonConvert.DeserializeObject<T>(await GetRawData(baseUrl)));

        private async Task<string> GetRawData(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage message = await client.GetAsync(url);
                message.EnsureSuccessStatusCode();
                return await message.Content.ReadAsStringAsync();
            }
        }

        internal class MythicRaiderData
        {
            [JsonProperty("profile_url")]
            public string ProfileUri { get; set; }

            [JsonProperty("mythic_plus_highest_level_runs")]
            public List<Mythic> BestMythicRuns { get; set; }
        }
    }
} 