using System.Collections.Generic;
using PlayGame.Pirates;
using PlayGame.Speech.Commands;
using PlayGame.UI;
using Statics;
using UnityEngine;
using UnityEngine.AI;

namespace PlayGame.Player.Movement 
{
    public class MoveObject : MonoBehaviour {
        
        private Vector3 _direction;
        private Vector3 _destination = Vector3.positiveInfinity;

        private PlayerData _playerData;
        private NavMeshAgent _playerAgent;

        // Colliders of the player's ship and the destination object
        private Collider _playerCollider;
        private Collider _targetCollider;

        // Whether the ship is headed to a specific object
        private bool _hasTargetObject;

        public bool rotating;
        private bool _turnRight; // false = turn left, true = turn right

        // Needed to reference enemies in order to rotate towards them
        private GameObject _enemySpawner;

        // Needed to reference asteroids in order to rotate towards them
        private GameObject _asteroidSpawner;
        
        private Transform _lockTarget;
        public ToggleCommand.LockTargetType lockType = ToggleCommand.LockTargetType.None;

        private Transform _spaceStation;
        
        private void Start() {
            // fetch the objects of the spawners
            _enemySpawner = PirateSpawner.GetInstance().gameObject;
            _asteroidSpawner = AsteroidSpawner.GetInstance().gameObject;
            _spaceStation = GameObject.FindWithTag(Tags.StationTag).transform;
            
            // Get the initial components
            _direction = transform.rotation.eulerAngles;
            _playerData = GetComponent<PlayerData>();
            _playerCollider = GetComponent<Collider>();
            _playerAgent = GetComponent<NavMeshAgent>();
            UpdateRotation();
        }

        public bool InLockRange(ToggleCommand.LockTargetType lockTargetType) {
            float lockRange = 0;
            if (lockTargetType == ToggleCommand.LockTargetType.Asteroid) lockRange = GameConstants.PlayerMiningRange;
            if (lockTargetType == ToggleCommand.LockTargetType.Pirate) lockRange = _playerData.GetLaserRange();
            
            return Vector3.Distance(transform.position, _lockTarget.position) < lockRange;
        }

        private void Update() {
            if (GameManager.gameOver) return;
            // Get the speed of the player's ship
            float speed = _playerData.GetSpeed();

            // If locked on automatically move so player is in range
            if (lockType != ToggleCommand.LockTargetType.None && _lockTarget != null) {
                // If player not in range move forward
                if (!InLockRange(lockType)) {
                    SetDirection(transform.forward, false);
                    SetSpeed(1);
                } else SetSpeed(0);
            }

            // Check if destination has been reached and if not provide it to AI
            if (!HasReachedDestination()) {
                _playerAgent.SetDestination(_destination);
            } else if (speed > 0) {
                // The speed is not 0, so the ship should be moving
                transform.Translate((Time.deltaTime * speed) * _direction, Space.World);
            }
            
            // Rotate slowly
            if (rotating) Rotate();

            if (lockType != ToggleCommand.LockTargetType.None) {
                if (_lockTarget == null) _lockTarget = GetLockTarget(lockType);
                if (_lockTarget != null) FaceTarget(_lockTarget);
                else
                {
                    // Turn off lock on if nothing in range
                    lockType = ToggleCommand.LockTargetType.None;
                }
            } else {
                _lockTarget = null;
            }
        }

        private void Rotate() {
            float rotateSpeed = _playerData.GetRotateSpeed();
            Vector3 newDirection;
            
            if (_turnRight) newDirection = Vector3.Slerp(transform.forward, transform.right, rotateSpeed * Time.deltaTime);
            else newDirection = Vector3.Slerp(transform.forward, -transform.right, rotateSpeed * Time.deltaTime);
            
            transform.localRotation = Quaternion.LookRotation(newDirection);
            _direction = newDirection;
        }

        private bool HasReachedDestination() 
        {
            // If AI is disabled then the destination has either been reached or there is no destination at all
            if (!_playerAgent.enabled)
            {
                return true;
            }
            
            // The destination was reached, stop the player ship
            if (Vector3.Distance(transform.position, _destination) < 0.2)
            {
                SetSpeed(0f);
                return true;
            }

            // If moving to a target check distance based on closest points of collision
            if (_hasTargetObject) 
            {
                Vector3 closestPointTarget = _targetCollider.ClosestPoint(transform.position);
                Vector3 closestPointPlayer = _playerCollider.ClosestPoint(closestPointTarget);
                
                // The colliders are very close, stop the player ship
                if (Vector3.Distance(closestPointPlayer, closestPointTarget) < 2) 
                {
                    SetSpeed(0f);
                    return true;
                }
            }

            return false;
        }

        // Enemy target needed for lock-on
        public Transform GetNearestEnemyTransform() {
            return GetNearestTransform(GetChildren(_enemySpawner.transform));
        }

        public Transform GetNearestAsteroidTransform() {
            return GetNearestTransform(GetChildren(_asteroidSpawner.transform));
        }

        // Returns a list of all child transforms
        private static List<Transform> GetChildren(Transform parent) {
            List<Transform> children = new List<Transform>();
            
            foreach (Transform child in parent) {
                children.Add(child);
            }

            return children;
        }

        // Returns the nearest transform from a list
        private Transform GetNearestTransform(List<Transform> transforms) {
            float bestDistance = float.PositiveInfinity;
            int closestEnemyIndex = -1;
            
            for (int i = 0; i < transforms.Count; i++) {
                float calculatedDistance = Vector3.SqrMagnitude(transforms[i].transform.position - transform.position);
                if (calculatedDistance < bestDistance) {
                    bestDistance = calculatedDistance;
                    closestEnemyIndex = i;
                }
            }

            return closestEnemyIndex == -1 ? null : transforms[closestEnemyIndex].transform;
        }

        private void FaceTarget(Transform target) {
            if (target == null) return; // If the target is destroyed just return.

            bool forward = transform.forward == _direction;
            
            rotating = false;
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);

            if (forward) _direction = transform.forward;
            else _direction = -transform.forward;
        }

        // Turn to face the direction the player is moving
        private void UpdateRotation() {
            transform.localRotation = Quaternion.LookRotation(_direction);
        }

        public void StartRotating(Vector3 targetDirection) {
            rotating = true;
            _turnRight = targetDirection == transform.right;
        }

        public void StopRotating() {
            rotating = false;
        }
    
        public void SetDirection(Vector3 newDirection, bool rotate) {
            StopRotating();
            // Set the direction to be the new direction
            _direction = newDirection;
            
            // Set the flags for player not heading anywhere
            _hasTargetObject = false;
            _playerAgent.enabled = false;
            
            // Update the rotation of the player
            if (rotate) UpdateRotation();
        }

        public void SetDestination(Vector3 destination) {
            // Set the destination
            _destination = destination;
            
            // Set the direction to destination
            Vector3 direction = (destination - transform.position).normalized;
            SetDirection(direction, true);
            
            // Set the flags specifying that the player is not headed to a specific object and enable the AI
            _hasTargetObject = false;
            _playerAgent.enabled = true;
        }

        public void SetDestination(Vector3 destination, Collider targetCollider) 
        {
            SetDestination(destination);
            
            // Set the target object collider and specify that the player is heading to an object
            _targetCollider = targetCollider;
            _hasTargetObject = true;
        }

        // Sets the current speed to a percentage of the players maximum speed
        public void SetSpeed(float fraction) {
            _playerData.SetSpeed(fraction);
        }

        public void SetLockTargetType(ToggleCommand.LockTargetType type) {
            lockType = type;
            _lockTarget = null;
        }
        
        private Transform GetLockTarget(ToggleCommand.LockTargetType lockTargetType) {
            Transform lockTarget = null;
            
            if (lockTargetType == ToggleCommand.LockTargetType.Pirate) lockTarget = GetNearestEnemyTransform();
            if (lockTargetType == ToggleCommand.LockTargetType.Asteroid) lockTarget = GetNearestAsteroidTransform();
            if (lockTarget == null) return null;

            // Player cant lock onto targets they cant see
            if (DebugSettings.FogOfWar && Vector3.Distance(transform.position, lockTarget.position) > _playerData.GetLookRadius()) lockTarget = null;

            return lockTarget;
        }

        public Transform GetLockTarget()
        {
            return _lockTarget;
        }
        
        public ToggleCommand.LockTargetType GetLockType()
        {
            return lockType;
        }

        public float DistanceToStation() {
            return Vector3.Distance(transform.position, _spaceStation.position);
        }
        
        public bool NearStation() {
            return GridCoord.GetCoordFromVector(transform.position).Equals(GridCoord.GetCoordFromVector(_spaceStation.position));
        }

    }
}
