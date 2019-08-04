using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Residuum.Services.Database;
using Residuum.Services.Entities;

namespace Residuum.Services.Readers
{
    public class CachedBestMythicAccessor
    {
        private CacheContext _context;

        public CachedBestMythicAccessor(CacheContext context)
        {
            _context = context;
        }

        public IEnumerable<Mythic> GetBestMythic(string name)
        {
            var run = FindRun(name);

            if (run != null)
            {
                return new List<Mythic> { run.MythicRun };
            }

            return Enumerable.Empty<Mythic>();
        }

        public void SetBestMythic(string name, Mythic mythic)
        {
            var newMythicRun = new BestMythicRun
            {
                MythicRun = mythic,
                Name = name
            };
         
            var existingCachedMythic = FindRun(name);

            if (existingCachedMythic != null)
            {
                _context.Entry(existingCachedMythic).CurrentValues.SetValues(newMythicRun);
            }
            else
            {
                _context.BestMythicRuns.Add(newMythicRun);
            }

            _context.SaveChanges();
        }

        private BestMythicRun FindRun(string name)
        {
            return _context.BestMythicRuns.Include(run => run.MythicRun).SingleOrDefault(run => run.Name.Equals(name));
        }
    }
}