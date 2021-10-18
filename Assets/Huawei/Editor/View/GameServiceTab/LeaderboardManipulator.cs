using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HmsPlugin
{
    public interface ILeaderboardManipulator : ICollectionManipulator
    {
        IEnumerable<HMSLeaderboardEntry> GetAllLeaderboards();
        void RemoveLeaderboard(HMSLeaderboardEntry value);
        AddLeaderboardResult AddLeaderboard(string leaderboardName, string id);
    }

    public enum AddLeaderboardResult
    {
        OK,
        AlreadyExists,
        Invalid
    }


    public class LeaderboardManipulator : ILeaderboardManipulator
    {
        public event Action OnRefreshRequired;
        private HMSSettings _settings;
        private List<HMSLeaderboardEntry> _leaderboardList;

        public LeaderboardManipulator(HMSSettings settings)
        {
            _settings = settings;
            _leaderboardList = new List<HMSLeaderboardEntry>();
            for (int i = 0; i < settings.Keys.Count(); i++)
            {
                _leaderboardList.Add(new HMSLeaderboardEntry(_settings.Keys.ElementAt(i), _settings.Values.ElementAt(i)));
            }
        }

        public AddLeaderboardResult AddLeaderboard(string leaderboardName, string id)
        {
            id = id.PreprocessValue();
            leaderboardName = leaderboardName.RemoveSpecialCharacters();

            var canAdd = CanAdd(id);
            if (canAdd == AddLeaderboardResult.OK)
            {
                _leaderboardList.Add(new HMSLeaderboardEntry(id, leaderboardName));
                _settings.Set(id, leaderboardName);
                RequireRefresh();
            }

            return canAdd;
        }

        private void RequireRefresh()
        {
            OnRefreshRequired.InvokeSafe();
        }

        private AddLeaderboardResult CanAdd(string id)
        {
            if (string.IsNullOrEmpty(id)) return AddLeaderboardResult.Invalid;

            foreach (var leaderboard in _leaderboardList)
            {
                if (leaderboard.Id.Equals(id))
                {
                    return AddLeaderboardResult.AlreadyExists;
                }
            }
            return AddLeaderboardResult.OK;
        }

        public void Dispose()
        {
            OnRefreshRequired = null;
        }

        public void RemoveLeaderboard(HMSLeaderboardEntry value)
        {
            Debug.Assert(_leaderboardList.Contains(value), "Failed to find " + value.Id + " in Leaderboard Settings file!");
            _leaderboardList.Remove(value);
            _settings.Remove(value.Id);
            RequireRefresh();
        }

        public IEnumerable<HMSLeaderboardEntry> GetAllLeaderboards()
        {
            return _leaderboardList;
        }
    }
}
