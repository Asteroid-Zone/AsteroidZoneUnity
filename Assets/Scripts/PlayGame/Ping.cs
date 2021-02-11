using UnityEngine;

namespace Assets.Scripts.PlayGame {

    public enum PingType {
        None,
        Asteroid,
        Pirate
    }
    
    public struct Ping {

        private GridCoord _gridCoord;

        private PingType _type;
        
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

    }
}