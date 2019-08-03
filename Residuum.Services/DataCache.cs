﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArgentPonyWarcraftClient;
using Newtonsoft.Json;
using Residuum.Services.Database;
using Residuum.Services.Entities;
using Residuum.Services.QueryHandlers;
using Residuum.Services.Readers;
using Guild = ArgentPonyWarcraftClient.Guild;
using GuildMember = Residuum.Services.Entities.GuildMember;
using Progression = ArgentPonyWarcraftClient.Progression;

namespace Residuum.Services
{
    public class DataCache
    {
        private const int CacheForMinutes = 120;

        private readonly RaiderClient _raiderClient;

        private readonly CachedGuildMembersAccessor _cachedGuildMembersAccessor;
        private readonly CachedRaidProgressAccessor _cachedRaidProgressAccessor;
        private readonly CachedBestMythicAccessor _cachedBestMythicAccessor;

        public DataCache()
        {
            _raiderClient = new RaiderClient();
            _cachedGuildMembersAccessor = new CachedGuildMembersAccessor();
            _cachedRaidProgressAccessor = new CachedRaidProgressAccessor();
            _cachedBestMythicAccessor = new CachedBestMythicAccessor();
        }

        public void Initialize(CacheContext context)
        {
            var guildUpdateTask = UpdateGuildMembersCache(context);
            var raidProgressUpdateTask = UpdateRaidProgress(context);

            Task.WaitAll(raidProgressUpdateTask);
        }

        public async Task<IEnumerable<GuildMember>> GetGuildMembers(CacheContext context)
        {
            var guildMembers = _cachedGuildMembersAccessor.GetGuildMembers(context);

            if (!guildMembers.Any())
            {
                var loadedGuildMembers = await GetGuildMembersFromApi();

                _cachedGuildMembersAccessor.SetGuildMembers(context, loadedGuildMembers);

                return loadedGuildMembers;
            }

            if (guildMembers.Any(member => ShouldUpdateCache(member.LastUpdated)))
            {
                _ = UpdateGuildMembersCache(context);
            }

            return guildMembers;
        }
        
        public async Task<RaidProgress> GetRaidProgress(CacheContext context)
        {
            var raidProgressEntry = _cachedRaidProgressAccessor.GetRaidProgress(context);

            if (!raidProgressEntry.Any())
            {
                var loadedRaidProgress = await GetRaidProgressFromApi();

                _cachedRaidProgressAccessor.SetRaidProgress(context, loadedRaidProgress);

                return loadedRaidProgress;
            }

            var raidProgess = raidProgressEntry.Single();

            if (ShouldUpdateCache(raidProgess.LastUpdated))
            {
                _ = UpdateRaidProgress(context);
            }

            return raidProgess;
        }

        public async Task<Mythic> GetBestMythic(CacheContext context, string realm, string player)
        {
            var bestMythicEntry = _cachedBestMythicAccessor.GetBestMythic(context, player);

            if (!bestMythicEntry.Any())
            {
                var loadedBestMythic = await GetBestMythicFromApi(realm, player);

                _cachedBestMythicAccessor.SetBestMythic(context, player, loadedBestMythic);

                return loadedBestMythic;
            }

            var bestMythic = bestMythicEntry.Single();

            if (ShouldUpdateCache(bestMythic.LastUpdated))
            {
                _ = UpdateBestMythic(realm, player);
            }

            return bestMythic;
        }

        private async Task UpdateGuildMembersCache(CacheContext context)
        {
            var guildMembers = await GetGuildMembersFromApi();

            _cachedGuildMembersAccessor.SetGuildMembers(context, guildMembers);
        }

        private async Task<IEnumerable<GuildMember>> GetGuildMembersFromApi()
        {
            var warcraftClient = new WarcraftClient(ServiceConfiguration.BlizzardAuthentication.ClientId,
                ServiceConfiguration.BlizzardAuthentication.Secret, Region.Europe, Locale.en_GB);

            RequestResult<Guild> guild = await warcraftClient.GetGuildAsync(ServiceConfiguration.RealmName,
                ServiceConfiguration.GuildName, GuildFields.Members);

            if (!guild.Success)
            {
                throw new InvalidOperationException($"Guild could not be loaded. Error: {guild.Error}");
            }

            return guild.Value.Members.Select(MapGuildMember);
        }

        private GuildMember MapGuildMember(ArgentPonyWarcraftClient.GuildMember argentPonyGuildMember)
        {
            return new GuildMember
            {
                Class = argentPonyGuildMember.Character.Class.ToString(),
                Name = argentPonyGuildMember.Character.Name,
                Rank = argentPonyGuildMember.Rank,
                Realm = argentPonyGuildMember.Character.Realm
            };
        }

        private async Task UpdateRaidProgress(CacheContext context)
        {
            RaidProgress progression = await GetRaidProgressFromApi();

            _cachedRaidProgressAccessor.SetRaidProgress(context, progression);
        }

        private async Task<RaidProgress> GetRaidProgressFromApi()
        {
            RaidProgressWithDictionary raidProgressWithDictionary = await _raiderClient.GetGuildProgress(ServiceConfiguration.RealmName, ServiceConfiguration.GuildName);

            var raidProgress = (RaidProgress)raidProgressWithDictionary;
            ConvertRaidProgressDictionaryIntoList(raidProgressWithDictionary, raidProgress);

            return raidProgress;
        }

        private static void ConvertRaidProgressDictionaryIntoList(RaidProgressWithDictionary raidProgressWithDictionary, RaidProgress raidProgress)
        {
            raidProgress.RaidInfo = raidProgressWithDictionary.RaidInfo.Select(progressPair =>
                new Entities.Progression { RaidName = progressPair.Key, Details = progressPair.Value }).ToList();
        }

        private async Task UpdateBestMythic(string realm, string player)
        {
            var updatedBestMythic = await GetBestMythicFromApi(realm, player);

            /*using (var context = new CacheContext())
            {
                var bestMythic = context.BestMythicRuns.Find(player).MythicRun;

                bestMythic.Item = updatedBestMythic;
                bestMythic.LastUpdated = DateTime.Now;

                await context.SaveChangesAsync();
            }*/
        }

        public async Task<Mythic> GetBestMythicFromApi(string realm, string player)
        {
            var bestMythic = await _raiderClient.GetBestMythic(realm, player);

            return bestMythic;
        }

        private bool ShouldUpdateCache(DateTime lastUpdated)
        {
            return DateTime.Now.Subtract(lastUpdated).TotalMinutes > CacheForMinutes;
        }
    }
}