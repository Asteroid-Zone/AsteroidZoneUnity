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
            Ping,
            Pirate,
            Asteroid
        }
        
        public readonly MovementType movementType;
        public readonly Vector3 direction;

        public readonly DestinationType destinationType;
        
        public readonly GridCoord gridCoord;

        public readonly bool turn;

        public MovementCommand(MovementType movementType, string data, bool turn) : base(CommandType.Movement) {
            this.turn = turn;
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
                case Strings.Pirate:
                    return DestinationType.Pirate;
                case Strings.Asteroid:
                    return DestinationType.Asteroid;
                default:
                    throw new ArgumentException("Invalid Destination");
            }
        }

    }
}