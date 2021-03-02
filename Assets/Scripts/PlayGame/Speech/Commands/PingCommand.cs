using PlayGame.Pings;

namespace PlayGame.Speech.Commands {
    public class PingCommand : Command {

        public readonly PingType pingType;
        public readonly GridCoord gridCoord;

        public PingCommand(string type, string coord) : base(CommandType.Ping) {
            pingType = GetPingTypeFromString(type);
            gridCoord = GetGridCoordFromString(coord);
        }

        private PingType GetPingTypeFromString(string type) {
            throw new System.NotImplementedException();
        }

        private GridCoord GetGridCoordFromString(string coord) {
            throw new System.NotImplementedException();
        }
    }
}