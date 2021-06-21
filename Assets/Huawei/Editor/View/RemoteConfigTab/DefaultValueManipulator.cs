using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HmsPlugin
{
    public interface IDefaultValueManipulator : ICollectionManipulator
    {
        IEnumerable<DefaultValue> GetAllDefaultValues();
        void RemoveDefaultValue(DefaultValue value);
        AddDefaultValueResult AddDefaultValue(string keyName, string keyValue);
    }

    public enum AddDefaultValueResult
    {
        OK,
        AlreadyExists,
        Invalid
    }

    public class DefaultValueManipulator : IDefaultValueManipulator
    {
        public event Action OnRefreshRequired;

        private HMSSettings _settings;
        private List<DefaultValue> _defaultValues;

        public DefaultValueManipulator(HMSSettings settings)
        {
            _settings = settings;
            _defaultValues = new List<DefaultValue>();

            for (int i = 0; i < _settings.Keys.Count(); i++)
            {
                _defaultValues.Add(new DefaultValue(_settings.Keys.ElementAt(i), _settings.Values.ElementAt(i)));
            }
        }

        public AddDefaultValueResult AddDefaultValue(string keyName, string keyValue)
        {
            keyName = PreprocessValue(keyName);
            keyValue = PreprocessValue(keyValue);

            var canAdd = CanAdd(keyName);
            if (canAdd == AddDefaultValueResult.OK)
            {
                _defaultValues.Add(new DefaultValue(keyName, keyValue));
                _settings.Set(keyName, keyValue);
                RequireRefresh();
            }

            return canAdd;
        }

        public void Dispose()
        {
            OnRefreshRequired = null;
        }

        public IEnumerable<DefaultValue> GetAllDefaultValues()
        {
            return _defaultValues;
        }

        public void RemoveDefaultValue(DefaultValue value)
        {
            Debug.Assert(_defaultValues.Contains(value), "Failed to find " + value.Key + " in Remote Config Settings file!");
            _defaultValues.Remove(value);
            _settings.Remove(value.Key);
            RequireRefresh();
        }

        private string PreprocessValue(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            text.Trim(' ');
            text.Trim('\n');

            return text;
        }

        private AddDefaultValueResult CanAdd(string text)
        {
            if (string.IsNullOrEmpty(text)) return AddDefaultValueResult.Invalid;

            foreach (var defaultValueKey in _defaultValues)
            {
                if (defaultValueKey.Key.Equals(text))
                {
                    return AddDefaultValueResult.AlreadyExists;
                }
            }
            return AddDefaultValueResult.OK;
        }

        protected void RequireRefresh()
        {
            OnRefreshRequired.InvokeSafe();
        }
    }

    public class DefaultValue
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public DefaultValue(string keyName, string keyValue)
        {
            Key = keyName;
            Value = keyValue;
        }
    }
}
