using UnityEngine;

namespace HmsPlugin
{
    public class SettingsScriptableObject : ScriptableObject
    {
        public Settings settings = new Settings();

        public void Save()
        {
            ScriptableHelper.Save(this);
        }
    }
}
