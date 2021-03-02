using UnityEngine;

namespace PlayGame.Speech.Commands {
    public class TurnOnCommand : Command {

        public enum ObjectType {
            MiningLaser,
            Lock
        }

        private bool _on; // true if turning on, false if turning off
        private ObjectType _objectType;
        private GameObject _lockTarget;

        public TurnOnCommand(bool on, ObjectType objectType) : base(CommandType.Toggle) {
            _on = on;
            _objectType = objectType;
        }
        
        public TurnOnCommand(bool on, ObjectType objectType, string lockTarget) : base(CommandType.Toggle) {
            _on = on;
            _objectType = objectType;
            _lockTarget = GetLockTargetFromString(lockTarget);
        }

        private GameObject GetLockTargetFromString(string lockTarget) {
            throw new System.NotImplementedException();
        }
    }
}