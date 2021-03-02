namespace PlayGame.Speech.Commands {
    public class Command {
        public enum CommandType {
            Movement,
            Turn,
            Ping,
            Transfer,
            Toggle,
            Speed,
            Shoot
        }

        private readonly bool _isValid;
        private CommandType _commandType;

        // Creates an invalid command
        public Command() {
            _isValid = false;
        }

        public Command(CommandType commandType) {
            _isValid = true;
            _commandType = commandType;
        }

        public bool IsValid() {
            return _isValid;
        }

    }
}