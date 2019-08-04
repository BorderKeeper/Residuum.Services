using Residuum.Services.Database;
using Residuum.Services.Entities;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Residuum.Services.Readers
{
    public class CachedRaidProgressAccessor
    {
        private CacheContext _context;

        public CachedRaidProgressAccessor(CacheContext context)
        {
            _context = context;
        }

        public IEnumerable<RaidProgress> GetRaidProgress()
        {
            var cachedRaidProgress = GetRaidProgressWithRaidInfo();

            if (cachedRaidProgress != null)
            {
                return new List<RaidProgress> { cachedRaidProgress };
            }

            return Enumerable.Empty<RaidProgress>();
        }

        public void SetRaidProgress(RaidProgress raidProgress)
        {
            if (_context.RaidProgress.Any())
            {
                var existingRaidProgress = _context.RaidProgress.First();

                _context.Entry(existingRaidProgress).CurrentValues.SetValues(raidProgress);
            }
            else
            {
                _context.RaidProgress.Add(raidProgress);
            }

            _context.SaveChanges();
        }

        private RaidProgress GetRaidProgressWithRaidInfo()
        {
            return _context.RaidProgress
                .Include(progress => progress.RaidInfo)
                .ThenInclude(raidinfo => raidinfo.Details)
                .First();
        }
    }
}