namespace PlayGame.Speech.Commands {
    public class SpeedCommand : Command {

        public readonly float speed;

        public SpeedCommand(string speed) : base(CommandType.Speed) {
            this.speed = GetSpeedFromString(speed);
        }

        private float GetSpeedFromString(string speed) {
            throw new System.NotImplementedException();
        }
    }
}