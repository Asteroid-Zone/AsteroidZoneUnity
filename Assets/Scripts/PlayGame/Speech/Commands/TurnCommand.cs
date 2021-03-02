using System;
using UnityEngine;

namespace PlayGame.Speech.Commands {
    public class TurnCommand : Command {

        public enum TurnType {
            Direction,
            Destination,
            Grid
        }
        
        public readonly TurnType turnType;
        public readonly Vector3 direction;
        public readonly Vector3 destination;
        public readonly GridCoord gridCoord;

        public TurnCommand(TurnType turnType, string data) : base(CommandType.Turn) {
            this.turnType = turnType;
            switch (turnType) {
                case TurnType.Direction:
                    direction = GetVectorFromString(data);
                    break;
                case TurnType.Destination:
                    destination = GetVectorFromString(data);
                    break;
                case TurnType.Grid:
                    gridCoord = GetCoordFromString(data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(turnType), turnType, null);
            }
        }
        
        private static Vector3 GetVectorFromString(string data) {
            throw new NotImplementedException();
        }
        
        private static GridCoord GetCoordFromString(string data) {
            throw new NotImplementedException();
        }
    }
}