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
        
        public readonly MovementCommand.DestinationType destinationType;
        
        public readonly GridCoord gridCoord;

        public TurnCommand(TurnType turnType, string data) : base(CommandType.Turn) {
            this.turnType = turnType;
            switch (turnType) {
                case TurnType.Direction:
                    direction = MovementCommand.GetDirectionVectorFromString(data);
                    break;
                case TurnType.Destination:
                    destinationType = MovementCommand.GetDestinationTypeFromString(data);
                    break;
                case TurnType.Grid:
                    gridCoord = GridCoord.GetCoordFromString(data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(turnType), turnType, null);
            }
        }
    }
}