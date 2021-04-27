namespace Statics {
    public static class GameConstants {

        public static float TimeLimit = 300; // Time limit in seconds

        
        
        // Grid
        public static int GridCellSize = 10;
        public static int GridWidth = 11;
        public static int GridHeight = 11;
        
        

        // ---------- Pirates ----------
        
        public static int PirateLaserMiningRate = 4; // Amount of resources gathered every mining tick

        // Pirate Scout
        public static float PirateScoutMaxHealth = 50;
        public static float PirateScoutSpeed = 2;
        public static float PirateScoutLookRadius = 15;
        public static float PirateScoutLaserSpeed = 20;
        public static int PirateScoutLaserRange = 10;
        public static int PirateScoutLaserDamageRange = 5;
        public static int PirateScoutLaserDamage = 10;
        public static int PirateScoutShotDelay = 1000;
        
        // Pirate Elite
        public static float PirateEliteMaxHealth = 100;
        public static float PirateEliteSpeed = 1;
        public static float PirateEliteLookRadius = 10;
        public static float PirateEliteLaserSpeed = 20;
        public static int PirateEliteLaserRange = 15;
        public static int PirateEliteLaserDamageRange = 5;
        public static int PirateEliteLaserDamage = 15;
        public static int PirateEliteShotDelay = 1000;
        
        // Pirate Spawning
        // Every X seconds, there is a chance for an pirate to spawn on a random grid coordinate 
        public static float MaxPiratesMultiplier = 1;
        public static float PirateProbability = 0.5f;
        public static float PirateEveryXSeconds = 5;
        public static int PirateMinReinforcements = 2;
        public static int PirateMaxReinforcements = 4;
        
        
        
        // ---------- Players ----------

        public static int PlayerRespawnCost = 150; // Resources to respawn a player
        public static int PlayerRespawnCostIncrease = 25; // Amount the cost increases each time
        public static int PlayerMaxHealth = 200;
        public static float PlayerMaxSpeed = 2.5f;
        public static float PlayerRotateSpeed = 0.5f;

        public static float PlayerLookRadius = 20f;
        
        // Mining Laser
        public static int PlayerMiningRange = 20;
        public static int PlayerMiningRate = 8; // Amount of resources gathered every mining tick
        public static int PlayerMiningDelay = 20; // Number of frames to wait between mining
        
        // Combat Laser
        public static int PlayerShotDelay = 1000; // Number of ms to wait between shooting
        public static int PlayerLaserSpeed = 20;
        public static int PlayerLaserDamage = 20;
        public static int PlayerLaserDamageRange = 10; // Makes the amount of damage the laser does vary a bit
        public static int PlayerLaserMiningRate = 4; // Amount of resources gathered every mining tick
        public static int PlayerLaserRange = 20;
        
        // XP
        public static int InitialLevelUpThreshold = 1000; // Amount of XP needed to level up from 0 to 1
        public static int LevelUpScaleAmount = 100; // Level up amount increase each time

        public static int MinXPCombatHit = 50;
        public static int MaxXPCombatHit = 150;
        public static int MinXPCombatKill = 100;
        public static int MaxXPCombatKill = 250;
        
        public static int MinXPMiningHit = 20;
        public static int MaxXPMiningHit = 50;
        public static int MinXPMiningComplete = 100;
        public static int MaxXPMiningComplete = 250;
        
        
        // ---------- Space Station ----------
        
        // Modules
        public static int EnginesMaxHealth = 200;
        public static int HyperdriveMaxHealth = 2000;
        public static int StationHullMaxHealth = 1500;
        public static int SolarPanelsMaxHealth = 100;
        
        // Shields
        public static int ShieldGeneratorMaxHealth = 150;
        public static int StationMaxShields = 100;
        public static int StationShieldsRechargeRate = 1; // Amount the shields recharge per second

        
        
        // ---------- Asteroids ----------
        
        public static int AsteroidMinResources = 50;
        public static int AsteroidMaxResources = 150;
        
        // Spawning - Every X seconds, there is a chance for an asteroid to spawn on a random grid coordinate 
        public static float AsteroidProbability = 0.7f;
        public static float AsteroidEveryXSeconds = 3;
        public static float MaxAsteroidsMultiplier = 1;

    }
}