namespace PlayGame.Speech {
    public class Command {
        public enum CommandType {
            Movement,
            Turn
        }

        private readonly bool _isValid;
        private CommandType _commandType;

        // Creates an invalid command
        public Command() {
            _isValid = false;
        }

        protected Command(CommandType commandType) {
            _isValid = true;
            _commandType = commandType;
        }

        public bool IsValid() {
            return _isValid;
        }

    }
}