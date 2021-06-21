using UnityEngine;

namespace HmsPlugin
{
    public class SettingsScriptableObject : ScriptableObject
    {
        public HMSSettings settings = new HMSSettings();

        public void Save()
        {
            ScriptableHelper.Save(this);
        }
    }
}
