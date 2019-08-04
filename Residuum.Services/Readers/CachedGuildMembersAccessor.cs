using Residuum.Services.Database;
using Residuum.Services.Entities;
using System.Collections.Generic;

namespace Residuum.Services.Readers
{
    public class CachedGuildMembersAccessor
    {
        private CacheContext _context;

        public CachedGuildMembersAccessor(CacheContext context)
        {
            _context = context;
        }

        public IEnumerable<GuildMember> GetGuildMembers()
        {
            return _context.GuildMembers;
        }

        public void SetGuildMembers(IEnumerable<GuildMember> guildMembers)
        {
            _context.GuildMembers.RemoveRange(_context.GuildMembers);

            _context.GuildMembers.AddRange(guildMembers);

            _context.SaveChanges();
        }
    }
}