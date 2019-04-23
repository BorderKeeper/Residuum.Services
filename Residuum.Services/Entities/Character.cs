using System.Collections.Generic;

namespace Residuum.Services.Entities
{
    public class Character
    {
        public string Name { get; set; }

        public string Class { get; set; }

        public int Rank { get; set; }

        public Mythic BestMythic { get; set; }
    }
}