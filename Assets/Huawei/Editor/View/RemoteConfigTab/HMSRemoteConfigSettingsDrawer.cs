using HmsPlugin.List;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace HmsPlugin
{
    internal class HMSRemoteConfigSettingsDrawer : VerticalSequenceDrawer
    {
        private HMSSettings _defaultValueSettings;
        private HMSSettings _remoteConfigSettings;

        private Foldout _defaultValuesFoldout = new Foldout("Default Value List");
        private Toggle.Toggle _applyDefaultValuesToggle;
        private Toggle.Toggle _developerModeToggle;

        private DefaultValueManipulator _defaultValueManipulator;

        public HMSRemoteConfigSettingsDrawer()
        {
            _defaultValueSettings = HMSRemoteDefaultValueSettings.Instance.Settings;
            _remoteConfigSettings = HMSRemoteConfigSettings.Instance.Settings;
            _defaultValueManipulator = new DefaultValueManipulator(_defaultValueSettings);
            _developerModeToggle = new Toggle.Toggle("Developer Mode", _remoteConfigSettings.GetBool(HMSRemoteConfigSettings.DeveloperMode), OnDeveloperModeToggle);
            _applyDefaultValuesToggle = new Toggle.Toggle("Apply Default Values*", _remoteConfigSettings.GetBool(HMSRemoteConfigSettings.ApplyDefaultValues), OnApplyDefaultValuesToggle).SetTooltip("This will apply default values on start");

            _defaultValueManipulator.OnRefreshRequired += OnDefaultValueChanged;

            OnDefaultValueChanged();
            SetupSequence();
        }

        private void OnApplyDefaultValuesToggle(bool value)
        {
            _remoteConfigSettings.SetBool(HMSRemoteConfigSettings.ApplyDefaultValues, value);
        }

        private void OnDeveloperModeToggle(bool value)
        {
            _remoteConfigSettings.SetBool(HMSRemoteConfigSettings.DeveloperMode, value);
        }

        ~HMSRemoteConfigSettingsDrawer()
        {
            _defaultValueManipulator.OnRefreshRequired -= OnDefaultValueChanged;
            _defaultValueManipulator.Dispose();
            _defaultValueSettings.Dispose();
            _remoteConfigSettings.Dispose();
        }

        private void OnDefaultValueChanged()
        {
            _defaultValuesFoldout.RemoveAllDrawers();
            _defaultValuesFoldout.AddDrawer(CreateDefaultValuesListDrawer(_defaultValueManipulator.GetAllDefaultValues()));
        }

        private void SetupSequence()
        {
            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Default Values").SetBold(true), new HorizontalLine()));
            AddDrawer(new Space(3));
            AddDrawer(_defaultValuesFoldout);
            AddDrawer(new Space(10));
            AddDrawer(new HMSRemoteConfigAdderDrawer(_defaultValueManipulator));
            AddDrawer(_applyDefaultValuesToggle);
            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.Button("Create Constant Classes", CreateRemoteConfigConstants).SetWidth(250), new Spacer()));
            AddDrawer(new HorizontalLine());
            AddDrawer(new Space(20));
            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Utilities").SetBold(true), new HorizontalLine()));
            AddDrawer(_developerModeToggle);
            AddDrawer(new HorizontalLine());
        }

        private void CreateRemoteConfigConstants()
        {
            if (_defaultValueSettings.Keys.Count() > 0)
            {
                using (var file = File.CreateText(Application.dataPath + "/Huawei/Scripts/Utils/HMSRemoteConfigConstants.cs"))
                {
                    file.WriteLine("public class HMSRemoteConfigConstants\n{");
                    for (int i = 0; i < _defaultValueSettings.Keys.Count(); i++)
                    {
                        file.WriteLine($"\tpublic const string {_defaultValueSettings.Keys.ElementAt(i).Replace(".", "").Replace(" ", "")} = \"{_defaultValueSettings.Values.ElementAt(i)}\";");
                    }
                    file.WriteLine("}");
                }
                AssetDatabase.Refresh();
            }
        }

        private IDrawer CreateDefaultValuesListDrawer(IEnumerable<DefaultValue> defaultValues)
        {
            //TODO make that 25 button width value as a constant somewhere.
            return ListDrawer<DefaultValue>.CreateButtonedLabelList(defaultValues, s => "Key: " + s.Key + " | Value: " + s.Value, null, new List<Button.ButtonInfo<DefaultValue>> { new Button.ButtonInfo<DefaultValue>("x", 25, OnRemovePressed) }).SetEmptyDrawer(new Label.Label("No Default Values Found."));
        }

        private void OnRemovePressed(DefaultValue defaultValue)
        {
            _defaultValueManipulator.RemoveDefaultValue(defaultValue);
        }
    }
}
