using UnityEngine;

namespace PlayGame.Speech.Commands {
    public class TurnOnCommand : Command {

        public enum ObjectType {
            MiningLaser,
            Lock
        }

        private ObjectType _objectType;
        private GameObject _lockTarget;

        public TurnOnCommand(ObjectType objectType) : base(CommandType.TurnOn) {
            _objectType = objectType;
        }
        
        public TurnOnCommand(ObjectType objectType, string lockTarget) : base(CommandType.TurnOn) {
            _objectType = objectType;
            _lockTarget = GetLockTargetFromString(lockTarget);
        }

        private GameObject GetLockTargetFromString(string lockTarget) {
            throw new System.NotImplementedException();
        }
    }
}