namespace Statics
{
    // ReSharper disable always ConvertToConstant.Global
    public static class DebugSettings {
        // Readonly variables are used instead of constants, because when using constants, the code grays out the if statements in which the variables are used
        // as they are always either true or false
        
        public static readonly bool Debug = false; // true disables photon, allows running in unity editor
        public static readonly bool SinglePlayer = false; // true makes game playable for 1 player, station commander but spawn with ship
        public static bool DebugKeys = false; // disables/enables keyboard controls
        public static bool ArrowKeys = false; // disables/enables arrow key controls

        public static readonly bool SpawnPirates = true; // enables/disables pirate spawning
        public static readonly bool InfiniteMiningRange = false;

        public static readonly bool TileBasedFogOfWar = true; // Switch between Tile based fog of war and radius based
    }
}