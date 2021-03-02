using System;
using PlayGame.Pings;

namespace PlayGame.Speech.Commands {
    public class PingCommand : Command {

        public readonly PingType pingType;
        public readonly GridCoord gridCoord;

        public PingCommand(string type, string coord) : base(CommandType.Ping) {
            pingType = GetPingTypeFromString(type);
            gridCoord = GridCoord.GetCoordFromString(coord);
        }

        private static PingType GetPingTypeFromString(string type) {
            switch (type) {
                case "none":
                    return PingType.None;
                case "asteroid":
                    return PingType.Asteroid;
                case "pirate":
                    return PingType.Pirate;
                default:
                    throw new ArgumentException("Invalid Ping Type");
            }
        }
    }
}