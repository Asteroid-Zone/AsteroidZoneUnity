using System;

namespace PlayGame.Speech.Commands {
    public class ToggleCommand : Command {

        public enum ObjectType {
            MiningLaser,
            Lock
        }

        public enum LockTargetType {
            Pirate,
            Asteroid
        }

        public readonly bool on; // true if turning on, false if turning off
        public readonly ObjectType objectType;
        public readonly LockTargetType lockTargetType;

        public ToggleCommand(bool on, ObjectType objectType) : base(CommandType.Toggle) {
            this.on = on;
            this.objectType = objectType;
        }
        
        public ToggleCommand(bool on, ObjectType objectType, string lockTargetType) : base(CommandType.Toggle) {
            this.on = on;
            this.objectType = objectType;
            if (on) this.lockTargetType = GetLockTargetTypeFromString(lockTargetType);
        }

        private static LockTargetType GetLockTargetTypeFromString(string lockTargetType) {
            switch (lockTargetType) {
                case "pirate":
                    return LockTargetType.Pirate;
                case "enemy":
                    return LockTargetType.Pirate;
                case "asteroid":
                    return LockTargetType.Asteroid;
                default:
                    throw new ArgumentException("Invalid Lock Target Type: " + lockTargetType);
            }
        }
    }
}