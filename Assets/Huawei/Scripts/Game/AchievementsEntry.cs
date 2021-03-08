using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    public class AchievementEntry
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public AchievementEntry(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
