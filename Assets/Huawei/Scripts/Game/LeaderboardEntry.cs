using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    public class LeaderboardEntry
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public LeaderboardEntry(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
