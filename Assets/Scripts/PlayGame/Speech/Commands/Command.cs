namespace PlayGame.Speech.Commands {
    public class Command {
        public enum CommandType {
            Movement,
            Ping,
            Transfer,
            Toggle,
            Speed
        }

        private readonly bool _isValid;
        private readonly CommandType _commandType;

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

        public CommandType GetCommandType() {
            return _commandType;
        }

    }
}