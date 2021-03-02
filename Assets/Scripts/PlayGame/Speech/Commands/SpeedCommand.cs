namespace PlayGame.Speech.Commands {
    public class SpeedCommand : Command {

        private float _speed;

        public SpeedCommand(string speed) : base(CommandType.Speed) {
            _speed = GetSpeedFromString(speed);
        }

        private float GetSpeedFromString(string speed) {
            throw new System.NotImplementedException();
        }
    }
}