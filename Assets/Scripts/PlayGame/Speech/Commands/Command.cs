namespace PlayGame.Speech.Commands {
    
    /// <summary>
    /// This class is the base class for all types of command.
    /// </summary>
    public class Command {
        
        /// <summary>
        /// Types of command.
        /// </summary>
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

        /// <summary>
        /// Constructor for an invalid command.
        /// </summary>
        public Command() {
            _isValid = false;
        }

        /// <summary>
        /// Base constructor for all valid commands.
        /// </summary>
        /// <param name="commandType">Type of command.</param>
        /// <param name="commanderOnly">True if only the commander can perform this command.</param>
        /// <param name="minerOnly">True if only miners can perform this command.</param>
        public Command(CommandType commandType, bool commanderOnly, bool minerOnly) {
            _isValid = true;
            _commandType = commandType;
            _commanderOnly = commanderOnly;
            _minerOnly = minerOnly;
        }

        /// <summary>
        /// Returns true if its a valid command.
        /// </summary>
        public bool IsValid() {
            return _isValid;
        }

        /// <summary>
        /// Returns the type of command.
        /// </summary>
        public CommandType GetCommandType() {
            return _commandType;
        }

        /// <summary>
        /// Returns true if only the commander can perform this command.
        /// </summary>
        public bool IsCommanderOnly() {
            return _commanderOnly;
        }

        /// <summary>
        /// Returns true if only miners can perform this command.
        /// </summary>
        public bool IsMinerOnly() {
            return _minerOnly;
        }

    }
}