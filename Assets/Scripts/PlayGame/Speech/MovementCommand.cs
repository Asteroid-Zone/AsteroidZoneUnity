using System;
using UnityEngine;

namespace PlayGame.Speech {
    public class MovementCommand : Command {

        public enum MovementType {
            Direction,
            Destination,
            Grid
        }
        
        private MovementType _movementType;
        private Vector3 _direction;
        private Vector3 _destination;
        private GridCoord _gridCoord;

        public MovementCommand(MovementType movementType, string data) : base(CommandType.Movement) {
            _movementType = movementType;
            switch (movementType) {
                case MovementType.Direction:
                    _direction = GetVectorFromString(data);
                    break;
                case MovementType.Destination:
                    _destination = GetVectorFromString(data);
                    break;
                case MovementType.Grid:
                    _gridCoord = GetCoordFromString(data);
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