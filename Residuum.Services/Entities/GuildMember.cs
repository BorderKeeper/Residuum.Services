using System;
using System.ComponentModel.DataAnnotations;

namespace Residuum.Services.Entities
{
    public class GuildMember
    {        
        public GuildMember()
        {
            LastUpdated = DateTime.Now;
        }
       
        [Key]
        public string Name { get; set; }

        public string Realm { get; set; }

        public string Class { get; set; }

        public int Rank { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}