using System.Collections.Generic;
using System.Linq;
using PlayGame.Player;
using UnityEngine;

namespace PlayGame.Stats {
    
    /// <summary>
    /// This class manages the games stats.
    /// </summary>
    public static class StatsManager {

        /// <summary>
        /// Player roles that are displayed at the end of the game.
        /// <para>Based on the players performance.</para>
        /// </summary>
        public enum EndRole {
            Commander,
            Miner,
            Hunter,
            Defender,
            Support
        }

        public static readonly List<PlayerStats> PlayerStatsList = new List<PlayerStats>();
        public static readonly GameStats GameStats = new GameStats();

        /// <summary>
        /// Returns the PlayerStats of the player with the given photonID.
        /// <para>Returns null if no player is found with the given photonID.</para>
        /// </summary>
        /// <param name="id">PhotonID of the player.</param>
        public static PlayerStats GetPlayerStats(int id) {
            foreach (PlayerStats playerStats in PlayerStatsList) {
                if (playerStats.photonID.Equals(id)) return playerStats;
            }

            Debug.Log("Player stats not found");
            return null;
        }

        /// <summary>
        /// Sorts the list of PlayerStats into descending order by final score.
        /// </summary>
        public static void SortPlayersByScore() {
            List<PlayerStats> statsList = PlayerStatsList.OrderByDescending(x => x.finalScore).ThenByDescending(x => x.piratesDestroyed).ToList();
            
            PlayerStatsList.Clear();
            PlayerStatsList.AddRange(statsList);
        }

        /// <summary>
        /// Calculates each players final score and the total game score.
        /// </summary>
        public static void CalculateScores() {
            int score = 0;
            
            foreach (PlayerStats stats in PlayerStatsList) {
                stats.finalScore = GetPlayerScore(stats);
                score += stats.finalScore;
            }

            GameStats.finalScore = score;
        }

        /// <summary>
        /// Returns the players final score.
        /// </summary>
        /// <param name="playerStats"></param>
        private static int GetPlayerScore(PlayerStats playerStats) {
            return playerStats.role == Role.Miner ? GetMinerScore(playerStats) : GetCommanderScore();
        }

        /// <summary>
        /// Calculates and returns the station commanders final score.
        /// </summary>
        private static int GetCommanderScore() {
            int score = 0;
            
            if (GameStats.victory) score += 1500; // 1500 points for victory
            score += (int) (GameStats.gameTime * 1.5); // 1.5 points for each second survived

            return score;
        }

        /// <summary>
        /// Calculates and returns a miners final score.
        /// </summary>
        /// <param name="playerStats">The PlayerStats of the miner.</param>
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

        /// <summary>
        /// Selects and returns an EndRole for a player.
        /// </summary>
        /// <param name="id">The photonID of the player.</param>
        public static EndRole GetEndRole(int id) {
            PlayerStats playerStats = GetPlayerStats(id);
            if (playerStats.role == Role.StationCommander) return EndRole.Commander;

            if (HasMostAttackKills(playerStats)) return EndRole.Hunter;
            if (HasMostDefenceKills(playerStats)) return EndRole.Defender;
            if (HasMostMined(playerStats)) return EndRole.Miner;

            return EndRole.Support;
        }

        /// <summary>
        /// Returns true if the player has the most kills near the station.
        /// </summary>
        /// <param name="playerStats"></param>
        private static bool HasMostDefenceKills(PlayerStats playerStats) {
            foreach (PlayerStats stats in PlayerStatsList) {
                if (!playerStats.photonID.Equals(stats.photonID)) {
                    if (stats.piratesDestroyedDefence > playerStats.piratesDestroyedDefence) return false;
                }
            }

            return true;
        }
        
        /// <summary>
        /// Returns true if the player has the most kills not near the station.
        /// </summary>
        /// <param name="playerStats"></param>
        private static bool HasMostAttackKills(PlayerStats playerStats) {
            foreach (PlayerStats stats in PlayerStatsList) {
                if (!playerStats.photonID.Equals(stats.photonID)) {
                    if ((stats.piratesDestroyed - stats.piratesDestroyedDefence) > (playerStats.piratesDestroyed - playerStats.piratesDestroyedDefence)) return false;
                }
            }

            return true;
        }
        
        /// <summary>
        /// Returns true if the player has the most resources harvested.
        /// </summary>
        /// <param name="playerStats"></param>
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