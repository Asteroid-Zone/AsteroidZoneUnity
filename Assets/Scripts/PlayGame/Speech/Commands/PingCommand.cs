using System;
using PlayGame.Pings;
using Statics;

namespace PlayGame.Speech.Commands {
    public class PingCommand : Command {

        public readonly PingType pingType;
        public readonly GridCoord gridCoord;

        public PingCommand(string type, string coord) : base(CommandType.Ping, true, false) {
            pingType = GetPingTypeFromString(type);
            gridCoord = GridCoord.GetCoordFromString(coord);
        }

        private static PingType GetPingTypeFromString(string type) {
            switch (type) {
                case Strings.None:
                    return PingType.None;
                case Strings.Asteroid:
                    return PingType.Asteroid;
                case Strings.Pirate:
                    return PingType.Pirate;
                default:
                    throw new ArgumentException("Invalid Ping Type");
            }
        }
    }
}