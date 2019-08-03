using Microsoft.EntityFrameworkCore;
using Residuum.Services.Entities;

namespace Residuum.Services.Database
{
    public class CacheContext : DbContext
    {
        public CacheContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<GuildMember> GuildMembers { get; set; }

        public DbSet<RaidProgress> RaidProgress { get; set; }

        public DbSet<BestMythicRun> BestMythicRuns { get; set; }
    }
}