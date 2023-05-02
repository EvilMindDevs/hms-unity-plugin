using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HmsPlugin
{
    //This helper was written because Unity JsonUtility does not work directly with Collections only Arrays.
    public static class JsonHelper
    {
        public static List<T> FromJsonList<T>(string json) => JsonUtility.FromJson<Wrapper<T>>(json).Items.ToList();

        public static string ToJson<T>(List<T> list) => JsonUtility.ToJson(new Wrapper<T> { Items = list.ToArray() });

        public static string ToJson<T>(List<T> list, bool prettyPrint) => JsonUtility.ToJson(new Wrapper<T> { Items = list.ToArray() }, prettyPrint);

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}
