using UnityEngine;

namespace PlayGame.Pings {

    /// <summary>
    /// Type of object that has been pinged.
    /// </summary>
    public enum PingType {
        None,
        Asteroid,
        Pirate,
        Generic
    }
    
    /// <summary>
    /// This class stores the information about a ping.
    /// Ping has a <c>GridCoord</c> and a <c>PingType</c>.
    /// </summary>
    public class Ping {

        private readonly GridCoord _gridCoord;

        private readonly PingType _type;
        
        /// <summary>
        /// Create a ping from x and z coordinates and a PingType.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="type"></param>
        public Ping(char x, int z, PingType type) {
            _gridCoord = new GridCoord(x, z);
            _type = type;
        }

        /// <summary>
        /// Create a ping from a GridCoord and a PingType.
        /// </summary>
        /// <param name="gridCoord"></param>
        /// <param name="type"></param>
        public Ping(GridCoord gridCoord, PingType type) {
            _gridCoord = gridCoord;
            _type = type;
        }

        /// <summary>
        /// Returns the Vector3 world position of the ping.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPositionVector() {
            return _gridCoord.GetWorldVector();
        }

        /// <summary>
        /// Returns the PingType of the ping.
        /// </summary>
        /// <returns></returns>
        public PingType GetPingType() {
            return _type;
        }

        /// <summary>
        /// Returns the GridCoord position of the ping.
        /// </summary>
        /// <returns></returns>
        public GridCoord GetGridCoord() {
            return _gridCoord;
        }

    }
}