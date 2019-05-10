using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArgentPonyWarcraftClient;
using Residuum.Services.Entities;
using Residuum.Services.QueryHandlers;
using Guild = ArgentPonyWarcraftClient.Guild;

namespace Residuum.Services
{
    public static class DataAccessLayer
    {
        private const int CacheForMinutes = 120;

        private static Timestamped<Guild> CachedGuild { get; set; }

        private static Timestamped<RaidProgress> CachedRaidProgress { get; set; }

        private static Dictionary<string, Timestamped<Mythic>> CachedBestMythics { get; set; }

        public static void Initialize()
        {
            CachedGuild = new Timestamped<Guild>();
            CachedRaidProgress = new Timestamped<RaidProgress>();
            CachedBestMythics = new Dictionary<string, Timestamped<Mythic>>();

            var guildUpdateTask = UpdateGuildCache(CachedGuild);
            var raidProgressUpdateTask = UpdateRaidProgress(CachedRaidProgress);
           
            Task.WaitAll(guildUpdateTask, raidProgressUpdateTask);
        }

        public static async Task<Guild> GetGuild()
        {
            if (ShouldUpdateCache(CachedGuild.LastUpdated))
            {
               UpdateGuildCache(CachedGuild);
            }

            return CachedGuild.Item;
        }

        public static async Task<RaidProgress> GetRaidProgress()
        {
            if (ShouldUpdateCache(CachedRaidProgress.LastUpdated))
            {
                UpdateRaidProgress(CachedRaidProgress);
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
                Task.Run(() =>
                {
                    var updatedBestMythic = GetBestMythicFromApi(realm, player);

                    Task.WaitAll(updatedBestMythic);

                    CachedBestMythics[player] = new Timestamped<Mythic>(updatedBestMythic.Result);
                });
            }

            return cachedItem.Item;
        }

        private static async Task UpdateGuildCache(Timestamped<Guild> CachedGuild)
        {
            var warcraftClient = new WarcraftClient(ServiceConfiguration.BlizzardAuthentication.ClientId,
                ServiceConfiguration.BlizzardAuthentication.Secret, Region.Europe, Locale.en_GB);

            RequestResult<Guild> guild = await warcraftClient.GetGuildAsync(ServiceConfiguration.RealmName,
                ServiceConfiguration.GuildName, GuildFields.Members);

            if (guild.Success)
            {
                CachedGuild.Item = guild.Value;
                CachedGuild.LastUpdated = DateTime.Now;
            }
            else
            {
                throw new InvalidOperationException("Guild could not be loaded");
            }
        }

        private static async Task UpdateRaidProgress(Timestamped<RaidProgress> CachedRaidProgress)
        {
            var client = new RaiderClient();

            var progression = await client.GetGuildProgress(ServiceConfiguration.RealmName, ServiceConfiguration.GuildName);

            CachedRaidProgress.Item = progression;
            CachedRaidProgress.LastUpdated = DateTime.Now;
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

            public Timestamped()
            {
                LastUpdated = DateTime.Now;       
            }

            public Timestamped(T item)
            {
                Item = item;
                LastUpdated = DateTime.Now;
            }
        }

        private static bool ShouldUpdateCache(DateTime lastUpdated)
        {
            return DateTime.Now.Subtract(lastUpdated).TotalMinutes > CacheForMinutes;
        }
    }
}