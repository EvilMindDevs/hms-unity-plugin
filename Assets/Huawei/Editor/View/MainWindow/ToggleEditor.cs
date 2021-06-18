using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    public abstract class ToggleEditor
    {
        public bool Enabled { get; set; }
        public abstract void CreateManagers();
        public abstract void DestroyManagers();
    }
}
