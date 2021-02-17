using UnityEngine;
using UnityEngine.AI;

namespace PlayGame.Movement 
{
    public class MoveObject : MonoBehaviour 
    {

        // TODO change to Vector2?
        private Vector3 _direction;
        private Vector3 _destination = Vector3.positiveInfinity;

        private PlayerData _playerData;
        private NavMeshAgent _playerAgent;

        private Collider _playerCollider;
        private Collider _targetCollider;

        private bool _hasTarget = false;

        private void Start() 
        {
            _direction = transform.rotation.eulerAngles;
            _playerData = GetComponent<PlayerData>();
            _playerCollider = GetComponent<Collider>();
            _playerAgent = GetComponent<NavMeshAgent>();
            UpdateRotation();
        }

        private void Update()
        {
            var speed = _playerData.GetSpeed();
            if (!HasReachedDestination()) 
            {
                //transform.Translate((Time.deltaTime * _playerData.GetSpeed()) * _direction, Space.World);
                _playerAgent.SetDestination(_destination);
            }
            else if (speed > 0)
            {
                transform.Translate((Time.deltaTime * speed) * _direction, Space.World);
            }
        }

        private bool HasReachedDestination() 
        {
            if (!_playerAgent.enabled)
            {
                return true;
            }
            
            if (Vector3.Distance(transform.position, _destination) < 0.2)
            {
                SetSpeed(0f);
                return true;
            }

            // If moving to a target check distance based on closest points of collision
            if (_hasTarget) 
            {
                Vector3 closestPointTarget = _targetCollider.ClosestPoint(transform.position);
                Vector3 closestPointPlayer = _playerCollider.ClosestPoint(closestPointTarget);
                if (Vector3.Distance(closestPointPlayer, closestPointTarget) < 0.1) 
                {
                    SetSpeed(0f);
                    return true;
                }
            }

            return false;
        }

        // Turn to face the direction the player is moving
        private void UpdateRotation() 
        {
            transform.localRotation = Quaternion.LookRotation(_direction);
        }
    
        public void SetDirection(Vector3 newDirection) 
        {
            _direction = newDirection;
            _destination = Vector3.positiveInfinity; // Player keeps moving infinitely
            _hasTarget = false;
            _playerAgent.enabled = false;
            UpdateRotation();
        }

        public void SetDestination(Vector3 destination) 
        {
            _direction = Vector3.Normalize(destination - transform.position); // Get the direction vector to the destination
            _destination = destination;
            _hasTarget = false;
            _playerAgent.enabled = true;
            //UpdateRotation();
        }

        public void SetDestination(GameObject target, Collider targetCollider) 
        {
            Vector3 position = target.transform.position;
            _direction = Vector3.Normalize(position - transform.position); // Get the direction vector to the destination
            _destination = position;
            _targetCollider = targetCollider;
            _hasTarget = true;
            _playerAgent.enabled = true;
            //UpdateRotation();
        }

        // Sets the current speed to a percentage of the players maximum speed
        public void SetSpeed(float fraction) 
        {
            _playerData.SetSpeed(fraction);
            if (fraction == 0)
            {
                _playerAgent.enabled = false;
            }
        }
    }
}
