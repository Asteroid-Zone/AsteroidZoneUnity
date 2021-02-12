using UnityEngine;

namespace PlayGame.Movement {
    public class MoveObject : MonoBehaviour {

        // TODO change to Vector2?
        private Vector3 _direction;

        private PlayerData _playerData;

        private Vector3 _destination = Vector3.positiveInfinity;

        private void Start() {
            _direction = transform.rotation.eulerAngles;
            _playerData = GetComponent<PlayerData>();
            UpdateRotation();
        }

        private void Update() {
            if (!HasReachedDestination()) {
                transform.Translate((Time.deltaTime * _playerData.GetSpeed()) * _direction, Space.World);
            }
        }

        private bool HasReachedDestination() {
            return Vector3.Distance(transform.position, _destination) < 0.2;
        }

        // Turn to face the direction the player is moving
        private void UpdateRotation() {
            transform.localRotation = Quaternion.LookRotation(_direction);
        }
    
        public void SetDirection(Vector3 newDirection) {
            _direction = newDirection;
            _destination = Vector3.positiveInfinity; // Player keeps moving infinitely
            UpdateRotation();
        }

        public void SetDestination(Vector3 destination) {
            _direction = Vector3.Normalize(destination - transform.position); // Get the direction vector to the destination
            _destination = destination;
            UpdateRotation();
        }

        // Sets the current speed to a percentage of the players maximum speed
        public void SetSpeed(float fraction) {
            _playerData.SetSpeed(fraction);
        }
    }
}
