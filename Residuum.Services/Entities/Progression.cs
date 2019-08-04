using System.ComponentModel.DataAnnotations;

namespace Residuum.Services.Entities
{
    public class Progression
    {
        [Key]
        public string RaidName { get; set; }

        public ProgressionDetails Details { get; set; }
    }
}