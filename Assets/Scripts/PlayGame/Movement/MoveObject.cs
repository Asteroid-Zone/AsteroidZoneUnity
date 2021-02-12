using UnityEngine;

namespace PlayGame.Movement {
    public class MoveObject : MonoBehaviour {

        // TODO change to Vector2?
        private Vector3 _direction;
        private float _speed;

        private float _maxSpeed;

        private Vector3 _destination = Vector3.positiveInfinity;

        private void Start() {
            _direction = transform.rotation.eulerAngles;
            _maxSpeed = GetComponent<PlayerData>().GetSpeed();
            UpdateRotation();
        }

        private void Update() {
            if (!HasReachedDestination()) {
                transform.Translate((Time.deltaTime * _speed) * _direction, Space.World);
            }
        }

        private bool HasReachedDestination() {
            return Vector3.Distance(transform.position, _destination) < 0.2;
        }

        private void UpdateRotation() {
            transform.localRotation = Quaternion.LookRotation(_direction);
        }
    
        public void SetDirection(Vector3 newDirection) {
            _direction = newDirection;
            _destination = Vector3.positiveInfinity;
            UpdateRotation();
        }

        public void SetDestination(Vector3 destination) {
            _direction = Vector3.Normalize(destination - transform.position);
            _destination = destination;
            UpdateRotation();
        }

        // Sets the current speed to a percentage of the players maximum speed
        public void SetSpeed(float maxSpeedPercentage) {
            _speed = _maxSpeed * maxSpeedPercentage;
        }
    }
}
