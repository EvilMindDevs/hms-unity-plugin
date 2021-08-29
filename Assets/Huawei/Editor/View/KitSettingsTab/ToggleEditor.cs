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
        public virtual void CreateManagers() 
        {
            if (!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true))
                return;
        }
        public abstract void DestroyManagers();
        public abstract void DisableManagers();
    }
}
