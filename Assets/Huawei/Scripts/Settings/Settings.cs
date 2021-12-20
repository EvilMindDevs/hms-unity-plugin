using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    // an interface just for reading
    public interface IHMSSettings
    {
        event Action<string> OnValueChanged;

        string Get(string key, string defaultValue = null);
        int GetInt(string key, int defaultValue = 0);
        bool GetBool(string key, bool defaultValue = false);
    }

    [Serializable]
    public class HMSSettings : SerializableDictionary<string, string>, IHMSSettings
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

        public void SetLong(string key, long value)
        {
            Set(key, value.ToString());
        }

        public long GetLong(string key, long defaultValue = 0)
        {
            var stringValue = Get(key, null);
            if (stringValue != null)
            {
                long outNumber;
                if (long.TryParse(stringValue, out outNumber))
                {
                    return outNumber;
                }
            }
            return defaultValue;
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
