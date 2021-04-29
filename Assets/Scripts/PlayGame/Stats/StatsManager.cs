using System.Collections.Generic;
using PlayGame.Player;
using UnityEngine;

namespace PlayGame.Stats {
    public static class StatsManager {

        public enum EndRole {
            Commander,
            Miner,
            Hunter,
            Defender,
            Support
        }

        public static readonly List<PlayerStats> PlayerStatsList = new List<PlayerStats>();
        public static readonly GameStats GameStats = new GameStats();

        public static PlayerStats GetPlayerStats(int id) {
            foreach (PlayerStats playerStats in PlayerStatsList) {
                if (playerStats.photonID.Equals(id)) return playerStats;
            }

            Debug.Log("Player stats not found");
            return null;
        }
        
        public static int GetPlayerScore(int id) {
            PlayerStats playerStats = GetPlayerStats(id);
            
            return playerStats.role == Role.Miner ? GetMinerScore(playerStats) : GetCommanderScore();
        }

        private static int GetCommanderScore() {
            int score = 0;
            
            if (GameStats.victory) score += 1500; // 1500 points for victory
            score += (int) (GameStats.gameTime * 1.5); // 1.5 points for each second survived

            return score;
        }

        private static int GetMinerScore(PlayerStats playerStats) {
            int score = 0;
            
            score += playerStats.asteroidsDestroyed * 50; // 50 points for an asteroid
            score += playerStats.resourcesHarvested; // 1 point for each resource
            score += playerStats.piratesDestroyed * 100; // 100 points for a pirate
            score += playerStats.piratesDestroyedDefence * 20; // 120 points for an pirate near the station

            score += playerStats.finalCombatLevel * 5; // 5 points for each combat level
            score += playerStats.finalMiningLevel * 5; // 5 points for each mining level

            score -= playerStats.numberOfTimesRespawned * 300; // -300 points for each death
                
            if (GameStats.victory) score += 1000; // 1000 points for victory
            score += (int) GameStats.gameTime; // 1 point for each second survived

            return score;
        }

        public static EndRole GetEndRole(int id) {
            PlayerStats playerStats = GetPlayerStats(id);
            if (playerStats.role == Role.StationCommander) return EndRole.Commander;

            if (HasMostAttackKills(playerStats)) return EndRole.Hunter;
            if (HasMostDefenceKills(playerStats)) return EndRole.Defender;
            if (HasMostMined(playerStats)) return EndRole.Miner;

            return EndRole.Support;
        }

        private static bool HasMostDefenceKills(PlayerStats playerStats) {
            foreach (PlayerStats stats in PlayerStatsList) {
                if (!playerStats.photonID.Equals(stats.photonID)) {
                    if (stats.piratesDestroyedDefence > playerStats.piratesDestroyedDefence) return false;
                }
            }

            return true;
        }
        
        private static bool HasMostAttackKills(PlayerStats playerStats) {
            foreach (PlayerStats stats in PlayerStatsList) {
                if (!playerStats.photonID.Equals(stats.photonID)) {
                    if ((stats.piratesDestroyed - stats.piratesDestroyedDefence) > (playerStats.piratesDestroyed - playerStats.piratesDestroyedDefence)) return false;
                }
            }

            return true;
        }
        
        private static bool HasMostMined(PlayerStats playerStats) {
            foreach (PlayerStats stats in PlayerStatsList) {
                if (!playerStats.photonID.Equals(stats.photonID)) {
                    if (stats.resourcesHarvested > playerStats.resourcesHarvested) return false;
                }
            }

            return true;
        }

    }
}