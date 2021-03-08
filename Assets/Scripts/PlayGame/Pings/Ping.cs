using UnityEngine;

namespace PlayGame.Pings {

    public enum PingType {
        None,
        Asteroid,
        Pirate
    }
    
    public class Ping {

        private readonly GridCoord _gridCoord;

        private readonly PingType _type;
        
        public Ping(char x, int z, PingType type) {
            _gridCoord = new GridCoord(x, z);
            _type = type;
        }

        public Ping(GridCoord gridCoord, PingType type)
        {
            _gridCoord = gridCoord;
            _type = type;
        }

        public Vector3 GetPositionVector() {
            return _gridCoord.GetWorldVector();
        }

        public PingType GetPingType() {
            return _type;
        }

        public GridCoord GetGridCoord()
        {
            return _gridCoord;
        }

    }
}