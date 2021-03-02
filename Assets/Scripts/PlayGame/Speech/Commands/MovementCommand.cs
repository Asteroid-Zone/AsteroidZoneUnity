using System;
using UnityEngine;

namespace PlayGame.Speech.Commands {
    public class MovementCommand : Command {

        public enum MovementType {
            Direction,
            Destination,
            Grid
        }
        
        public readonly MovementType movementType;
        public readonly Vector3 direction;
        public readonly Vector3 destination;
        public readonly GridCoord gridCoord;

        public MovementCommand(MovementType movementType, string data) : base(CommandType.Movement) {
            this.movementType = movementType;
            switch (movementType) {
                case MovementType.Direction:
                    direction = GetVectorFromString(data);
                    break;
                case MovementType.Destination:
                    destination = GetVectorFromString(data);
                    break;
                case MovementType.Grid:
                    gridCoord = GetCoordFromString(data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(movementType), movementType, null);
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