using System;
using PlayGame.Pings;
using Statics;

namespace PlayGame.Speech.Commands {
    
    /// <summary>
    /// This class represents a ping command.
    /// </summary>
    public class PingCommand : Command {

        public readonly PingType pingType;
        public readonly GridCoord gridCoord;

        /// <summary>
        /// Constructor for a ping command.
        /// <remarks>Ping commands are commander only.</remarks>
        /// </summary>
        /// <param name="type">A string describing the type of ping.</param>
        /// <param name="coord">A string describing the grid coordinate of the ping.</param>
        public PingCommand(string type, string coord) : base(CommandType.Ping, true, false) {
            pingType = GetPingTypeFromString(type);
            gridCoord = GridCoord.GetCoordFromString(coord);
        }

        /// <summary>
        /// Converts a ping type string to a PingType.
        /// </summary>
        /// <param name="type">A string describing the type of ping.</param>
        /// <exception cref="ArgumentException">Thrown if the type string is not a recognised ping type.</exception>
        private static PingType GetPingTypeFromString(string type) {
            switch (type) {
                case Strings.None:
                    return PingType.None;
                case Strings.Asteroid:
                    return PingType.Asteroid;
                case Strings.Pirate:
                    return PingType.Pirate;
                case Strings.GenericPing:
                    return PingType.Generic;
                default:
                    throw new ArgumentException("Invalid Ping Type");
            }
        }
    }
}