using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArgentPonyWarcraftClient;
using Residuum.Services.Entities;
using Residuum.Services.QueryHandlers;
using Residuum.Services.Readers;
using Guild = ArgentPonyWarcraftClient.Guild;

namespace Residuum.Services
{
    public static class DataAccessLayer
    {
        private const int CacheForMinutes = 120;

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
               _ = UpdateGuildCache(CachedGuild);
            }

            return CachedGuild.Item;
        }

        public static async Task<RaidProgress> GetRaidProgress()
        {
            if (ShouldUpdateCache(CachedRaidProgress.LastUpdated))
            {
                _ = UpdateRaidProgress(CachedRaidProgress);
            }

            return CachedRaidProgress.Item;
        }

        public static async Task<Mythic> GetBestMythic(string realm, string player)
        {
            var accessor = new CachedBestMythicAccessor();

            var cachedBestMythic = accessor.GetBestMythic(player);

            if (!cachedBestMythic.Any())
            {
                var bestMythic = await GetBestMythicFromApi(realm, player);

                accessor.SetBestMythic(player, bestMythic);

                return bestMythic;
            }

            var cachedItem = cachedBestMythic.Single();

            if (ShouldUpdateCache(cachedItem.LastUpdated))
            {
                _ = UpdateBestMythic(realm, player);
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

        private static async Task UpdateBestMythic(string realm, string player)
        {
            var updatedBestMythic = await GetBestMythicFromApi(realm, player);

            CachedBestMythics[player] = new Timestamped<Mythic>(updatedBestMythic);
        }

        public static async Task<Mythic> GetBestMythicFromApi(string realm, string player)
        {
            var client = new RaiderClient();

            var bestMythic = await client.GetBestMythic(realm, player);

            return bestMythic;
        }

        private static bool ShouldUpdateCache(DateTime lastUpdated)
        {
            return DateTime.Now.Subtract(lastUpdated).TotalMinutes > CacheForMinutes;
        }
    }
}