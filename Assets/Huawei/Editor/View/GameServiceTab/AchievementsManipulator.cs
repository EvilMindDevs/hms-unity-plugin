using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HmsPlugin
{
    public interface IAchievementsManipulator : ICollectionManipulator
    {
        IEnumerable<HMSAchievementEntry> GetAllAchievements();
        void RemoveAchievement(HMSAchievementEntry value);
        AddAchievementResult AddAchievement(string achievementName, string id);
    }

    public enum AddAchievementResult
    {
        OK,
        AlreadyExists,
        Invalid
    }


    public class AchievementManipulator : IAchievementsManipulator
    {
        public event Action OnRefreshRequired;
        private HMSSettings _settings;
        private List<HMSAchievementEntry> _achievementList;

        public AchievementManipulator(HMSSettings settings)
        {
            _settings = settings;
            _achievementList = new List<HMSAchievementEntry>();
            for (int i = 0; i < settings.Keys.Count(); i++)
            {
                _achievementList.Add(new HMSAchievementEntry(_settings.Keys.ElementAt(i), _settings.Values.ElementAt(i)));
            }
        }

        public AddAchievementResult AddAchievement(string achievementName, string id)
        {
            id = id.PreprocessValue();
            achievementName = achievementName.RemoveSpecialCharacters();

            var canAdd = CanAdd(id);
            if (canAdd == AddAchievementResult.OK)
            {
                _achievementList.Add(new HMSAchievementEntry(id, achievementName));
                _settings.Set(id, achievementName);
                RequireRefresh();
            }

            return canAdd;
        }

        private void RequireRefresh()
        {
            OnRefreshRequired.InvokeSafe();
        }

        private AddAchievementResult CanAdd(string id)
        {
            if (string.IsNullOrEmpty(id)) return AddAchievementResult.Invalid;

            foreach (var achievement in _achievementList)
            {
                if (achievement.Id.Equals(id))
                {
                    return AddAchievementResult.AlreadyExists;
                }
            }
            return AddAchievementResult.OK;
        }

        public void Dispose()
        {
            OnRefreshRequired = null;
        }

        public void RemoveAchievement(HMSAchievementEntry value)
        {
            Debug.Assert(_achievementList.Contains(value), "Failed to find " + value.Id + " in Achievement Settings file!");
            _achievementList.Remove(value);
            _settings.Remove(value.Id);
            RequireRefresh();
        }

        public IEnumerable<HMSAchievementEntry> GetAllAchievements()
        {
            return _achievementList;
        }
    }
}
