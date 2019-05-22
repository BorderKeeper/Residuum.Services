using ArgentPonyWarcraftClient;
using Residuum.Services.Entities;
using System.Data.Entity;

namespace Residuum.Services.Database
{
    public class CacheContext : DbContext
    {
        public CacheContext() : base("CacheContext")
        {
        }

        public DbSet<Timestamped<Guild>> Guilds { get; set; }

        public DbSet<Timestamped<RaidProgress>> RaidProgress { get; set; }

        public DbSet<BestMythicRun> BestMythicRuns { get; set; }
    }
}