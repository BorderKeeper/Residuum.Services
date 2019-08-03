using System;
using System.Collections.Generic;
using System.Linq;
using Residuum.Services.Database;
using Residuum.Services.Entities;

namespace Residuum.Services.Readers
{
    public class CachedBestMythicAccessor
    {
        public IEnumerable<Mythic> GetBestMythic(CacheContext context, string name)
        {
            var run = context.BestMythicRuns.Find(name);

            if (run != null)
            {
                return new List<Mythic> { run.MythicRun };
            }

            return Enumerable.Empty<Mythic>();
        }

        public void SetBestMythic(CacheContext context, string name, Mythic mythic)
        {
            var newMythicRun = new BestMythicRun
            {
                MythicRun = mythic,
                Name = name
            };
         
            var existingCachedMythic = context.BestMythicRuns.Find(name);

            if (existingCachedMythic != null)
            {
                context.Entry(existingCachedMythic).CurrentValues.SetValues(existingCachedMythic);
            }
            else
            {
                context.BestMythicRuns.Add(existingCachedMythic);
            }

            context.SaveChanges();
        }
    }
}