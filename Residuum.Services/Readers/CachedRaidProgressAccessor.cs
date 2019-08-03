using Residuum.Services.Database;
using Residuum.Services.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Residuum.Services.Readers
{
    public class CachedRaidProgressAccessor
    {
        public IEnumerable<RaidProgress> GetRaidProgress(CacheContext context)
        {
            var cachedRaidProgress = context.RaidProgress.First();

            if (cachedRaidProgress != null)
            {
                return new List<RaidProgress> { cachedRaidProgress };
            }

            return Enumerable.Empty<RaidProgress>();
        }

        public void SetRaidProgress(CacheContext context, RaidProgress raidProgress)
        {
            var existingRaidProgress = context.RaidProgress.First();

            if (existingRaidProgress != null)
            {
                context.Entry(existingRaidProgress).CurrentValues.SetValues(raidProgress);
            }
            else
            {
                context.RaidProgress.Add(raidProgress);
            }

            context.SaveChanges();
        }
    }
}