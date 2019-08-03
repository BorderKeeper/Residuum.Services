using Residuum.Services.Database;
using Residuum.Services.Entities;
using System.Collections.Generic;

namespace Residuum.Services.Readers
{
    public class CachedGuildMembersAccessor
    {
        public IEnumerable<GuildMember> GetGuildMembers(CacheContext context)
        {
            return context.GuildMembers;
        }

        public void SetGuildMembers(CacheContext context, IEnumerable<GuildMember> guildMembers)
        {
            context.GuildMembers.RemoveRange(context.GuildMembers);

            context.GuildMembers.AddRange(guildMembers);

            context.SaveChanges();
        }
    }
}