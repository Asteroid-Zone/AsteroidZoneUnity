using System.Collections.Generic;
using UnityEngine;

namespace PlayGame.Stats {
    public static class StatsManager {

        public static readonly List<PlayerStats> PlayerStatsList = new List<PlayerStats>();
        // todo public GameStats gameStats;

        public static PlayerStats GetPlayerStats(string nickname) {
            foreach (PlayerStats playerStats in PlayerStatsList) {
                if (playerStats.playerName.Equals(nickname)) return playerStats;
            }

            Debug.Log("Player stats not found");
            return null;
        }
        
    }
}