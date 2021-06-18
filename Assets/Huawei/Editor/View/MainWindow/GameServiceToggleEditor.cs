using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HmsPlugin
{
    public class GameServiceToggleEditor : ToggleEditor, IDrawer
    {
        private Toggle.Toggle _toggle;
        private TabBar _tabBar;
        private TabView _tabView;
        private IDependentToggle _dependentToggle;

        public const string GameServiceEnabled = "GameService";

        public GameServiceToggleEditor(TabBar tabBar, IDependentToggle dependentToggle)
        {
            bool enabled = HMSMainEditorSettings.Instance.Settings.GetBool(GameServiceEnabled);
            _dependentToggle = dependentToggle;
            _tabView = HMSGameServiceTabFactory.CreateTab("Game Service");
            _tabBar = tabBar;
            _toggle = new Toggle.Toggle("Game Service*", enabled, OnStateChanged, true).SetTooltip("Game Service is dependent on AccountKit.");
        }

        private void OnStateChanged(bool value)
        {
            if (value)
            {
                _tabBar.AddTab(_tabView);
                CreateManagers();
                _dependentToggle.SetToggle();
            }
            else
            {
                DestroyManagers();
                _tabBar.RemoveTab(_tabView);
            }
            HMSMainEditorSettings.Instance.Settings.SetBool(GameServiceEnabled, value);
        }

        public void Draw()
        {
            _toggle.Draw();
        }

        public override void CreateManagers()
        {
            if (GameObject.FindObjectOfType<HMSGameManager>() == null)
            {
                GameObject obj = new GameObject("HMSGameManager");
                obj.AddComponent<HMSGameManager>();
                obj.AddComponent<HMSAchievementsManager>();
                obj.AddComponent<HMSLeaderboardManager>();
                obj.AddComponent<HMSSaveGameManager>();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            Enabled = true;
        }

        public override void DestroyManagers()
        {
            var gameKitManagers = GameObject.FindObjectsOfType<HMSGameManager>();
            if (gameKitManagers.Length > 0)
            {
                for (int i = 0; i < gameKitManagers.Length; i++)
                {
                    GameObject.DestroyImmediate(gameKitManagers[i].gameObject);
                }
            }
            Enabled = false;
        }
    }
}
