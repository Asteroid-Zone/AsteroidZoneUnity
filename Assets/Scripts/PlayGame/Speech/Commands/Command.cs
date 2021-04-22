namespace PlayGame.Speech.Commands {
    public class Command {
        public enum CommandType {
            Movement,
            Ping,
            Transfer,
            Toggle,
            Speed,
            Repair,
            Respawn
        }

        private readonly bool _isValid;
        private readonly CommandType _commandType;
        private readonly bool _commanderOnly;
        private readonly bool _minerOnly;

        // Creates an invalid command
        public Command() {
            _isValid = false;
        }

        public Command(CommandType commandType, bool commanderOnly, bool minerOnly) {
            _isValid = true;
            _commandType = commandType;
            _commanderOnly = commanderOnly;
            _minerOnly = minerOnly;
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

        public bool IsMinerOnly() {
            return _minerOnly;
        }

    }
}