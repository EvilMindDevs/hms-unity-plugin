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
    public class HMSGameServiceSettingsDrawer : VerticalSequenceDrawer
    {
        private HMSSettings _leaderboardSettings;
        private HMSSettings _achievementsSettings;

        private Foldout _leaderboardFoldout = new Foldout("Leaderboard List");
        private Foldout _achievementsFoldout = new Foldout("Achievements List");

        private LeaderboardManipulator _leaderboardManipulator;
        private AchievementManipulator _achievementManipulator;

        public HMSGameServiceSettingsDrawer()
        {
            _leaderboardSettings = HMSLeaderboardSettings.Instance.Settings;
            _achievementsSettings = HMSAchievementsSettings.Instance.Settings;
            _leaderboardManipulator = new LeaderboardManipulator(_leaderboardSettings);
            _achievementManipulator = new AchievementManipulator(_achievementsSettings);

            _leaderboardManipulator.OnRefreshRequired += OnLeaderboardListChanged;
            _achievementManipulator.OnRefreshRequired += OnAchievementListChanged;

            OnLeaderboardListChanged();
            OnAchievementListChanged();
            SetupSequence();
        }

        ~HMSGameServiceSettingsDrawer()
        {
            _leaderboardManipulator.OnRefreshRequired -= OnLeaderboardListChanged;
            _leaderboardManipulator.Dispose();
            _leaderboardSettings.Dispose();
        }

        private void SetupSequence()
        {
            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Leaderboard List").SetBold(true), new HorizontalLine()));
            AddDrawer(new Space(3));
            AddDrawer(_leaderboardFoldout);
            AddDrawer(new Space(10));
            AddDrawer(new HMSLeaderboardAdderDrawer(_leaderboardManipulator));
            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.Button("Create Constant Classes", CreateLeaderboardConstants).SetWidth(250), new Spacer()));
            AddDrawer(new HorizontalLine());

            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Achievement List").SetBold(true), new HorizontalLine()));
            AddDrawer(new Space(3));
            AddDrawer(_achievementsFoldout);
            AddDrawer(new Space(10));
            AddDrawer(new HMSAchievementsAdderDrawer(_achievementManipulator));
            AddDrawer(new HorizontalSequenceDrawer(new Spacer(), new Button.Button("Create Constant Classes", CreateAchievementsConstants).SetWidth(250), new Spacer()));
            AddDrawer(new HorizontalLine());

            AddDrawer(new HorizontalSequenceDrawer(new HorizontalLine(), new Label.Label("Utilities").SetBold(true), new HorizontalLine()));
            AddDrawer(new Space(3));
            AddDrawer(new Toggle.Toggle("Initialize On Start*", HMSGameServiceSettings.Instance.Settings.GetBool(HMSGameServiceSettings.InitializeOnStart), OnToggleChanged, false).SetTooltip("Game Service will be initialized on Start function."));
            AddDrawer(new HorizontalLine());
        }

        private void OnToggleChanged(bool value)
        {
            HMSGameServiceSettings.Instance.Settings.SetBool(HMSGameServiceSettings.InitializeOnStart, value);
        }

        private void CreateLeaderboardConstants()
        {
            if (_leaderboardSettings.Keys.Count() > 0)
            {
                using (var file = File.CreateText(Application.dataPath + "/Huawei/Scripts/Utils/HMSLeaderboardConstants.cs"))
                {
                    file.WriteLine("public class HMSLeaderboardConstants\n{");
                    for (int i = 0; i < _leaderboardSettings.Keys.Count(); i++)
                    {
                        file.WriteLine($"\tpublic const string {_leaderboardSettings.Values.ElementAt(i)} = \"{_leaderboardSettings.Keys.ElementAt(i)}\";");
                    }
                    file.WriteLine("}");
                }
                AssetDatabase.Refresh();
            }
        }

        private void CreateAchievementsConstants()
        {
            if (_achievementsSettings.Keys.Count() > 0)
            {
                using (var file = File.CreateText(Application.dataPath + "/Huawei/Scripts/Utils/HMSAchievementConstants.cs"))
                {
                    file.WriteLine("public class HMSAchievementConstants\n{");
                    for (int i = 0; i < _achievementsSettings.Keys.Count(); i++)
                    {
                        file.WriteLine($"\tpublic const string {_achievementsSettings.Values.ElementAt(i)} = \"{_achievementsSettings.Keys.ElementAt(i)}\";");
                    }
                    file.WriteLine("}");
                }
                AssetDatabase.Refresh();
            }
        }

        private void OnLeaderboardListChanged()
        {
            _leaderboardFoldout.RemoveAllDrawers();
            _leaderboardFoldout.AddDrawer(CreateLeaderboardListDrawer(_leaderboardManipulator.GetAllLeaderboards()));
        }

        private void OnAchievementListChanged()
        {
            _achievementsFoldout.RemoveAllDrawers();
            _achievementsFoldout.AddDrawer(CreateAchievementListDrawer(_achievementManipulator.GetAllAchievements()));
        }

        private IDrawer CreateAchievementListDrawer(IEnumerable<HMSAchievementEntry> achievements)
        {
            return ListDrawer<HMSAchievementEntry>.CreateButtonedLabelList(achievements, s => "Name: " + s.Name + " | ID: " + s.Id, null, new List<Button.ButtonInfo<HMSAchievementEntry>> { new Button.ButtonInfo<HMSAchievementEntry>("x", 25, OnRemoveAchievementsPressed) }).SetEmptyDrawer(new Label.Label("No Achievement Found."));
        }

        private IDrawer CreateLeaderboardListDrawer(IEnumerable<HMSLeaderboardEntry> leaderboards)
        {
            return ListDrawer<HMSLeaderboardEntry>.CreateButtonedLabelList(leaderboards, s => "Name: " + s.Name + " | ID: " + s.Id, null, new List<Button.ButtonInfo<HMSLeaderboardEntry>> { new Button.ButtonInfo<HMSLeaderboardEntry>("x", 25, OnRemoveLeaderboardPressed) }).SetEmptyDrawer(new Label.Label("No Leaderboard Found."));
        }

        private void OnRemoveLeaderboardPressed(HMSLeaderboardEntry leaderboard)
        {
            _leaderboardManipulator.RemoveLeaderboard(leaderboard);
        }

        private void OnRemoveAchievementsPressed(HMSAchievementEntry achievement)
        {
            _achievementManipulator.RemoveAchievement(achievement);
        }
    }
}
