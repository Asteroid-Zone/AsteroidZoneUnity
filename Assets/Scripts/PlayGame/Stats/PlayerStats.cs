using PlayGame.Player;

namespace PlayGame.Stats {
    
    /// <summary>
    /// This class stores player stats.
    /// </summary>
    public class PlayerStats {

        public int photonID;

        public string playerName;
        public Role role;

        public int asteroidsDestroyed;
        public int resourcesHarvested;

        public int piratesDestroyed;
        public int piratesDestroyedDefence;

        public int finalCombatLevel;
        public int finalMiningLevel;

        public int numberOfTimesRespawned;

        public int finalScore;

    }
}