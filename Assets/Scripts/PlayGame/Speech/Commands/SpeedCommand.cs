using System;

namespace PlayGame.Speech.Commands {
    public class SpeedCommand : Command {

        public readonly float speed;

        public SpeedCommand(string speed) : base(CommandType.Speed) {
            this.speed = GetSpeedFromString(speed);
        }

        private static float GetSpeedFromString(string speed) {
            switch (speed) {
                case "stop":
                    return 0f;
                case "go":
                    return 1f;
                default:
                    throw new ArgumentException("Invalid Speed");
            }
        }
    }
}