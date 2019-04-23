using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ArgentPonyWarcraftClient;
using Residuum.Services.Entities;
using Residuum.Services.QueryHandlers;
using Guild = ArgentPonyWarcraftClient.Guild;

namespace Residuum.Services
{
    public static class DataAccessLayer
    {
        private const int CacheForMinutes = 60;

        private static Timestamped<Guild> CachedGuild { get; set; }

        private static Timestamped<RaidProgress> CachedRaidProgress { get; set; }

        private static Dictionary<string, Timestamped<Mythic>> CachedBestMythics { get; set; }

        public static void Initialize()
        {
            var guild = GetGuildFromApi();
            var raidProgress = GetRaidProgressFromApi();

            CachedGuild = new Timestamped<Guild>(guild.Result);
            CachedRaidProgress = new Timestamped<RaidProgress>(raidProgress.Result);
            CachedBestMythics = new Dictionary<string, Timestamped<Mythic>>();

            Task.WaitAll(guild, raidProgress);
        }

        public static async Task<Guild> GetGuild()
        {
            if (ShouldUpdateCache(CachedGuild.LastUpdated))
            {
                CachedGuild.Item = await GetGuildFromApi();
            }

            return CachedGuild.Item;
        }

        public static async Task<RaidProgress> GetRaidProgress()
        {
            if (ShouldUpdateCache(CachedRaidProgress.LastUpdated))
            {
                CachedRaidProgress.Item = await GetRaidProgressFromApi();
            }

            return CachedRaidProgress.Item;
        }

        public static async Task<Mythic> GetBestMythic(string realm, string player)
        {
            if (!CachedBestMythics.ContainsKey(player))
            {
                var bestMythic = await GetBestMythicFromApi(realm, player);

                CachedBestMythics.Add(player, new Timestamped<Mythic>(bestMythic));

                return bestMythic;
            }

            var cachedItem = CachedBestMythics[player];

            if (ShouldUpdateCache(cachedItem.LastUpdated))
            {
                CachedBestMythics[player] = new Timestamped<Mythic>(await GetBestMythicFromApi(realm, player));
            }

            return cachedItem.Item;
        }

        private static async Task<Guild> GetGuildFromApi()
        {
            var warcraftClient = new WarcraftClient(ServiceConfiguration.BlizzardAuthentication.ClientId,
                ServiceConfiguration.BlizzardAuthentication.Secret, Region.Europe, Locale.en_GB);

            RequestResult<Guild> guild = await warcraftClient.GetGuildAsync(ServiceConfiguration.RealmName,
                ServiceConfiguration.GuildName, GuildFields.Members);

            return guild.Value;
        }

        private static async Task<RaidProgress> GetRaidProgressFromApi()
        {
            var client = new RaiderClient();

            var progression = await client.GetGuildProgress(ServiceConfiguration.RealmName, ServiceConfiguration.GuildName);

            return progression;
        }

        public static async Task<Mythic> GetBestMythicFromApi(string realm, string player)
        {
            var client = new RaiderClient();

            var bestMythic = await client.GetBestMythic(realm, player);

            return bestMythic;
        }

        private class Timestamped<T>
        {
            public T Item { get; set; }

            public DateTime LastUpdated { get; set; }

            public Timestamped(T item)
            {
                LastUpdated = DateTime.Now;
                Item = item;             
            }
        }

        private static bool ShouldUpdateCache(DateTime lastUpdated)
        {
            return DateTime.Now.Subtract(lastUpdated).TotalMinutes > CacheForMinutes;
        }
    }
}