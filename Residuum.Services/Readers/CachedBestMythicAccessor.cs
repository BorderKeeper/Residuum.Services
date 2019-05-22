using System;
using System.Collections.Generic;
using System.Linq;
using Residuum.Services.Database;
using Residuum.Services.Entities;

namespace Residuum.Services.Readers
{
    public class CachedBestMythicAccessor
    {
        public IEnumerable<Timestamped<Mythic>> GetBestMythic(string name)
        {
            using(var cacheDatabase = new CacheContext())
            {
                var run = cacheDatabase.BestMythicRuns.Find(name);

                if (run != null)
                {
                    return new List<Timestamped<Mythic>> { run.MythicRun };
                }

                return Enumerable.Empty<Timestamped<Mythic>>();
            }
        }

        public void SetBestMythic(string name, Mythic mythic)
        {
            var newMythicRun = new BestMythicRun
            {
                MythicRun = new Timestamped<Mythic>(mythic) { LastUpdated = DateTime.Now },
                Name = name
            };
         
            using (var cacheDatabase = new CacheContext())
            {
                var existingCachedMythic = cacheDatabase.BestMythicRuns.Find(name);

                if (existingCachedMythic != null)
                {
                    cacheDatabase.Entry(existingCachedMythic).CurrentValues.SetValues(existingCachedMythic);
                }
                else
                {
                    cacheDatabase.BestMythicRuns.Add(existingCachedMythic);
                }

                cacheDatabase.SaveChanges();
            }
        }
    }
}