using Newtonsoft.Json;
using System.Collections.Generic;

namespace Residuum.Services.Entities
{
    /// <summary>
    /// Hack class to make sure JSON can deserialize, we will then convert this to RaidProgress which can be stored in entityDB.
    /// If you know how to store a Dictionary in EntityDB or how to convert a JSON Dictionary into a list let me know.
    /// </summary>
    public class RaidProgressWithDictionary : RaidProgress
    {
        [JsonProperty("raid_progression")]
        public new Dictionary<string, ProgressionDetails> RaidInfo { get; set; }
    }
}