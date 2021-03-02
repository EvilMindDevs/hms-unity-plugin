using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    // an interface just for reading
    public interface ISettings
    {
        event Action<string> OnValueChanged;

        string Get(string key, string defaultValue = null);
        int GetInt(string key, int defaultValue = 0);
        bool GetBool(string key, bool defaultValue = false);
    }

    [Serializable]
    public class Settings : SerializableDictionary<string, string>, ISettings
    {
        public int GetInt(string key, int defaultValue = 0)
        {
            var stringValue = Get(key, null);
            if (stringValue != null)
            {
                int outNumber;
                if (int.TryParse(stringValue, out outNumber))
                {
                    return outNumber;
                }
            }
            return defaultValue;
        }

        public void SetInt(string key, int value)
        {
            Set(key, value.ToString());
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            var stringValue = Get(key, null);
            if (stringValue != null)
            {
                bool outValue;
                if (bool.TryParse(stringValue, out outValue))
                {
                    return outValue;
                }
            }
            return defaultValue;
        }

        public void SetBool(string key, bool value)
        {
            Set(key, value.ToString());
        }
    }
}
