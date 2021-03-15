namespace PlayGame.Speech.Commands {
    public class Command {
        public enum CommandType {
            Movement,
            Ping,
            Transfer,
            Toggle,
            Speed,
            Repair
        }

        private readonly bool _isValid;
        private readonly CommandType _commandType;
        private readonly bool _commanderOnly;

        // Creates an invalid command
        public Command() {
            _isValid = false;
        }

        public Command(CommandType commandType, bool commanderOnly) {
            _isValid = true;
            _commandType = commandType;
            _commanderOnly = commanderOnly;
        }

        public bool IsValid() {
            return _isValid;
        }

        public CommandType GetCommandType() {
            return _commandType;
        }

        public bool IsCommanderOnly() {
            return _commanderOnly;
        }

    }
}