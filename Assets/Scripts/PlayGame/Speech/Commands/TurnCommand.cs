using System;
using UnityEngine;

namespace PlayGame.Speech.Commands {
    public class TurnCommand : Command {

        public enum TurnType {
            Direction,
            Destination,
            Grid
        }
        
        private TurnType _turnType;
        private Vector3 _direction;
        private Vector3 _destination;
        private GridCoord _gridCoord;

        public TurnCommand(TurnType turnType, string data) : base(CommandType.Turn) {
            _turnType = turnType;
            switch (turnType) {
                case TurnType.Direction:
                    _direction = GetVectorFromString(data);
                    break;
                case TurnType.Destination:
                    _destination = GetVectorFromString(data);
                    break;
                case TurnType.Grid:
                    _gridCoord = GetCoordFromString(data);
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