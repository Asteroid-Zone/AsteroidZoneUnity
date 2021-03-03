using System;
using Statics;
using UnityEngine;

namespace PlayGame.Speech.Commands {
    public class MovementCommand : Command {

        public enum MovementType {
            Direction,
            Destination,
            Grid
        }

        public enum DestinationType {
            SpaceStation,
            Ping
        }
        
        public readonly MovementType movementType;
        public readonly Vector3 direction;

        public readonly DestinationType destinationType;
        
        public readonly GridCoord gridCoord;

        public MovementCommand(MovementType movementType, string data) : base(CommandType.Movement) {
            this.movementType = movementType;
            switch (movementType) {
                case MovementType.Direction:
                    direction = GetDirectionVectorFromString(data);
                    break;
                case MovementType.Destination:
                    destinationType = GetDestinationTypeFromString(data);
                    break;
                case MovementType.Grid:
                    gridCoord = GridCoord.GetCoordFromString(data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(movementType), movementType, null);
            }
        }

        public static Vector3 GetDirectionVectorFromString(string data) {
            switch (data) {
                case Strings.North:
                    return Vector3.forward;
                case Strings.East:
                    return Vector3.right;
                case Strings.South:
                    return Vector3.back;
                case Strings.West:
                    return Vector3.left;
                default:
                    throw new ArgumentException("Invalid Direction");
            }
        }
        
        public static DestinationType GetDestinationTypeFromString(string data) {
            switch (data) {
                case Strings.SpaceStation:
                    return DestinationType.SpaceStation;
                case Strings.Ping:
                    return DestinationType.Ping;
                default:
                    throw new ArgumentException("Invalid Destination");
            }
        }

    }
}