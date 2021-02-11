using UnityEngine;

namespace Assets.Scripts.PlayGame {

    public enum PingType {
        None,
        Asteroid,
        Pirate
    }
    
    public struct Ping {
        
        private int x { get; }
        private int z { get; }

        private PingType type { get; }
        
        public Ping(int x, int z, PingType type) {
            this.x = x;
            this.z = z;
            this.type = type;
        }

        public Vector3 GetPositionVector() {
            return new Vector3(x, 0, z);
        }

    }
}