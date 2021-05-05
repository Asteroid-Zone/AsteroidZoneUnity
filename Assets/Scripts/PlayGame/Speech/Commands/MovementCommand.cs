using System;
using Statics;
using UnityEngine;

namespace PlayGame.Speech.Commands {
    
    /// <summary>
    /// This class represents a movement command.
    /// </summary>
    public class MovementCommand : Command {

        /// <summary>
        /// The type of movement.
        /// </summary>
        public enum MovementType {
            Direction,
            Destination,
            Grid
        }

        /// <summary>
        /// The type of destination.
        /// </summary>
        public enum DestinationType {
            SpaceStation,
            Ping,
            Pirate,
            Asteroid
        }

        /// <summary>
        /// The type of turning.
        /// </summary>
        public enum TurnType {
            None,
            Instant,
            Smooth
        }

        public readonly MovementType movementType;
        public readonly Vector3 direction;
        public readonly string directionString;

        public readonly DestinationType destinationType;
        
        public readonly GridCoord gridCoord;

        public readonly bool turnOnly;
        public readonly TurnType turn;

        /// <summary>
        /// Constructor for a movement command.
        /// <remarks>Movement commands are miner only.</remarks>
        /// </summary>
        /// <param name="movementType">The type of movement.</param>
        /// <param name="data">A string containing the direction, destination or grid coordinate.</param>
        /// <param name="turnOnly">True if the player only wants to turn.</param>
        /// <param name="turn">The type of turn.</param>
        /// <param name="player">The transform of the player to move.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if movementType is not a valid MovementType.</exception>
        public MovementCommand(MovementType movementType, string data, bool turnOnly, TurnType turn, Transform player) : base(CommandType.Movement, false, true) {
            this.turnOnly = turnOnly;
            this.turn = turn;
            this.movementType = movementType;
            switch (movementType) {
                case MovementType.Direction:
                    directionString = data;
                    direction = GetDirectionVectorFromString(data, player);
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

        /// <summary>
        /// Converts a direction string to a Vector3.
        /// </summary>
        /// <param name="data">A string describing the direction.</param>
        /// <param name="player">The transform of the player to move.</param>
        /// <exception cref="ArgumentException">Thrown if the data string is not a recognised direction.</exception>
        private static Vector3 GetDirectionVectorFromString(string data, Transform player) {
            switch (data) {
                case Strings.North:
                    return Vector3.forward;
                case Strings.East:
                    return Vector3.right;
                case Strings.South:
                    return Vector3.back;
                case Strings.West:
                    return Vector3.left;
                case Strings.Forward:
                    return player.forward;
                case Strings.Back:
                    return -player.forward;
                case Strings.Right:
                    return player.right;
                case Strings.Left:
                    return -player.right;
                default:
                    throw new ArgumentException("Invalid Direction");
            }
        }

        /// <summary>
        /// Converts a destination string to a DestinationType.
        /// </summary>
        /// <param name="data">A string describing the destination type.</param>
        /// <exception cref="ArgumentException">Thrown if the data string is not a recognised destination.</exception>
        private static DestinationType GetDestinationTypeFromString(string data) {
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