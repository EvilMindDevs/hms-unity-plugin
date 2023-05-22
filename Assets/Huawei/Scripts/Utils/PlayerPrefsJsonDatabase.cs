using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace HmsPlugin
{
    // This class is used to store the dynamic type data in the PlayerPrefs as a json string.
    // Also fundamental CRUD operations are implemented.
    public class PlayerPrefsJsonDatabase<T> where T : class
    {
        private const string TAG = "[HMS] PlayerPrefsJsonDatabase ";
        private string prefsKey { get; set; }

        public PlayerPrefsJsonDatabase(string prefsKey){
           this.prefsKey = prefsKey;
        }

        public void Insert(T data)
        {
            var isExists = PlayerPrefs.HasKey(prefsKey) && PlayerPrefs.GetString(prefsKey) != "{}";

            if (isExists)
            {
                var getJson = PlayerPrefs.GetString(prefsKey);
                var list = JsonHelper.FromJsonList<T>(getJson);
                list.Add(data);
                var newJson = JsonHelper.ToJson(list);
                PlayerPrefs.DeleteKey(prefsKey);
                PlayerPrefs.SetString(prefsKey, newJson);
            }
            else
            {
                var list = new List<T> { data };
                var newJson = JsonHelper.ToJson(list,true);
                PlayerPrefs.SetString(prefsKey, newJson);
                Debug.Log($"{TAG} {PlayerPrefs.GetString(prefsKey)}");
            }

            Debug.LogFormat(TAG + " Inserted {0} to {1}", data, prefsKey);
            PlayerPrefs.Save();
        }
        public void Delete(string Id)
        {
            var isExists = PlayerPrefs.HasKey(prefsKey) && PlayerPrefs.GetString(prefsKey) != "{}";

            if (!isExists)
            {
                Debug.LogFormat(TAG + "Key not found");
            }
            
            var getJson = PlayerPrefs.GetString(prefsKey);
            var list = JsonHelper.FromJsonList<T>(getJson);
            
            var newList = list.Where(item => !item.Equals(Id)).ToList();
            
            var newJson = JsonHelper.ToJson(newList,true);
            
            PlayerPrefs.DeleteKey(prefsKey);
            PlayerPrefs.SetString(prefsKey, newJson);
            PlayerPrefs.Save();
        }
        public void Update(T data)
        {
            var isExists = PlayerPrefs.HasKey(prefsKey) && PlayerPrefs.GetString(prefsKey) != "{}";

            if (!isExists)
            {
                Debug.LogFormat(TAG + "Key not found");
            }
            
            var getJson = PlayerPrefs.GetString(prefsKey);
            var list = JsonHelper.FromJsonList<T>(getJson);
            
            var newList = new List<T>();

            foreach (var item in list)
            {
                if (item.Equals(data))
                {
                    newList.Add(data);
                }
                else
                {
                    newList.Add(item);
                }
            }
            
            var newJson = JsonHelper.ToJson(newList);
            
            PlayerPrefs.DeleteKey(prefsKey);
            PlayerPrefs.SetString(prefsKey, newJson);
            PlayerPrefs.Save();
        }
        public T Find(string Id)
        {
            var isExists = PlayerPrefs.HasKey(prefsKey) && PlayerPrefs.GetString(prefsKey) != "{}";

            if (!isExists)
            {
                Debug.LogFormat(TAG + "Key not found");

            }
            
            var getJson = PlayerPrefs.GetString(prefsKey);
            var list = JsonHelper.FromJsonList<T>(getJson);
            
            var data = list.FirstOrDefault(item => item.Equals(Id));
            PlayerPrefs.Save();
            return data;
        }    
        public List<T> GetAll()
        {
            var isExists = PlayerPrefs.HasKey(prefsKey) && PlayerPrefs.GetString(prefsKey) != "{}";

            if (!isExists)
            {
                Debug.LogFormat(TAG + "Key not found");
                return Enumerable.Empty<T>().ToList();
            }
            
            var getJson = PlayerPrefs.GetString(prefsKey);
            var list = JsonHelper.FromJsonList<T>(getJson);
            PlayerPrefs.Save();
            return list;
        }
    }


}

