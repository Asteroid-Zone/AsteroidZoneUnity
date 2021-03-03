using System;
using Statics;

namespace PlayGame.Speech.Commands {
    public class SpeedCommand : Command {

        public readonly float speed;

        public SpeedCommand(string speed) : base(CommandType.Speed) {
            this.speed = GetSpeedFromString(speed);
        }

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