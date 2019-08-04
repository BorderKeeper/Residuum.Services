using System.ComponentModel.DataAnnotations;

namespace Residuum.Services.Entities
{
    public class BestMythicRun
    {
        [Key]
        public string Name { get; set; }

        public Mythic MythicRun { get; set; }
    }
}