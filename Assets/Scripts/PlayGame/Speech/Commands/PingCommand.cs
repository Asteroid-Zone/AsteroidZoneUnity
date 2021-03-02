using PlayGame.Pings;

namespace PlayGame.Speech.Commands {
    public class PingCommand : Command {

        private PingType _pingType;
        private GridCoord _gridCoord;

        public PingCommand(string pingType, string gridCoord) : base(CommandType.Ping) {
            _pingType = GetPingTypeFromString(pingType);
            _gridCoord = GetGridCoordFromString(gridCoord);
        }

        private PingType GetPingTypeFromString(string pingType) {
            throw new System.NotImplementedException();
        }

        private GridCoord GetGridCoordFromString(string gridCoord) {
            throw new System.NotImplementedException();
        }
    }
}