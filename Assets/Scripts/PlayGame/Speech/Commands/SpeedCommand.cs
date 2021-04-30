using System;
using Statics;

namespace PlayGame.Speech.Commands {
    
    /// <summary>
    /// This class represents a speed command.
    /// </summary>
    public class SpeedCommand : Command {

        public readonly float Speed;

        /// <summary>
        /// Constructor for a speed command.
        /// <remarks>Speed commands are miner only.</remarks>
        /// </summary>
        /// <param name="speed">A string describing the speed.</param>
        public SpeedCommand(string speed) : base(CommandType.Speed, false, true) {
            Speed = GetSpeedFromString(speed);
        }

        /// <summary>
        /// Converts a speed string to a fraction of the max speed.
        /// </summary>
        /// <param name="speed">A string describing the speed.</param>
        /// <exception cref="ArgumentException">Thrown if the speed string is not recognised.</exception>
        private static float GetSpeedFromString(string speed) {
            switch (speed) {
                case Strings.Stop:
                    return 0;
                case Strings.Go:
                    return 1;
                default:
                    throw new ArgumentException("Invalid Speed");
            }
        }
    }
}