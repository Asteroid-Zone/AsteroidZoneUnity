using UnityEngine;

namespace PlayGame.Speech.Commands {
    public class ToggleCommand : Command {

        public enum ObjectType {
            MiningLaser,
            Lock
        }

        public readonly bool on; // true if turning on, false if turning off
        public readonly ObjectType objectType;
        public readonly GameObject lockTarget;

        public ToggleCommand(bool on, ObjectType objectType) : base(CommandType.Toggle) {
            this.on = on;
            this.objectType = objectType;
        }
        
        public ToggleCommand(bool on, ObjectType objectType, string lockTarget) : base(CommandType.Toggle) {
            this.on = on;
            this.objectType = objectType;
            this.lockTarget = GetLockTargetFromString(lockTarget);
        }

        private GameObject GetLockTargetFromString(string lockTarget) {
            throw new System.NotImplementedException();
        }
    }
}