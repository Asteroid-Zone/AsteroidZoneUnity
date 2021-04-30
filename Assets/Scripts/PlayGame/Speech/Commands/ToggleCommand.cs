using Statics;

namespace PlayGame.Speech.Commands {
    
    /// <summary>
    /// This class represents a toggle command.
    /// </summary>
    public class ToggleCommand : Command {

        /// <summary>
        /// Type of object to toggle.
        /// </summary>
        public enum ObjectType {
            MiningLaser,
            Lock,
            LaserGun,
            Hyperdrive
        }

        /// <summary>
        /// Type of target to lock onto.
        /// </summary>
        public enum LockTargetType {
            None,
            Pirate,
            Asteroid
        }

        public readonly bool on; // true if turning on, false if turning off
        public readonly ObjectType objectType;
        public readonly LockTargetType lockTargetType;

        /// <summary>
        /// Constructor for a toggle command.
        /// </summary>
        /// <param name="on">True if the object is being toggled on.</param>
        /// <param name="objectType">The type of object that is being toggled.</param>
        public ToggleCommand(bool on, ObjectType objectType) : base(CommandType.Toggle, objectType == ObjectType.Hyperdrive, objectType != ObjectType.Hyperdrive) {
            this.on = on;
            this.objectType = objectType;
        }
        
        /// <summary>
        /// Constructor for a lock toggle command.
        /// <remarks>Lock commands are miner only.</remarks>
        /// </summary>
        /// <param name="on">True if the object is being toggled on.</param>
        /// <param name="objectType">The type of object that is being toggled.</param>
        /// <param name="lockTargetType">A string describing the type of object to lock onto.</param>
        public ToggleCommand(bool on, ObjectType objectType, string lockTargetType) : base(CommandType.Toggle, false, true) {
            this.on = on;
            this.objectType = objectType;
            if (on) this.lockTargetType = GetLockTargetTypeFromString(lockTargetType);
        }

        /// <summary>
        /// Converts a string to a LockTargetType.
        /// </summary>
        /// <param name="lockTargetType">A string describing the type of target to lock onto.</param>
        private static LockTargetType GetLockTargetTypeFromString(string lockTargetType) {
            switch (lockTargetType) {
                case Strings.Pirate:
                    return LockTargetType.Pirate;
                case Strings.Asteroid:
                    return LockTargetType.Asteroid;
                default:
                    return LockTargetType.None;
            }
        }
    }
}