using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Residuum.Services.Entities
{
    public class Mythic
    {
        public Mythic()
        {
            LastUpdated = DateTime.Now;
        }

        [Key]
        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        [JsonProperty("dungeon")]
        public string Name { get; set; }

        [JsonProperty("mythic_level")]
        public int Difficulty { get; set; }

        [JsonProperty("num_keystone_upgrades")]
        public int Upgrades { get; set; }
        
        public string ProfileUri { get; set; }

        public DateTime LastUpdated { get; set; }

        public static Mythic EmptyMythic
        {
            get { return new Mythic { Difficulty = 0, Upgrades = 0, Name = "None", ShortName = "N", ProfileUri = string.Empty, LastUpdated = DateTime.Now }; }
        }
    }
}