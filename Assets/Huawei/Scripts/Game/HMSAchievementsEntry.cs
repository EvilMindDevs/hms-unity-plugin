using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    public class HMSAchievementEntry
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public HMSAchievementEntry(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
