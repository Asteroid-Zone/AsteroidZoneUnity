using System;
using System.Collections.Generic;
using PlayGame.Pirates;
using PlayGame.Speech.Commands;
using PlayGame.UI;
using Statics;
using UnityEngine;
using UnityEngine.AI;

namespace PlayGame.Player.Movement {
    
    /// <summary>
    /// This class controls the players movement and rotation.
    /// </summary>
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

        private LaserGun _laserGun;
        private MiningLaser _miningLaser;

        private bool _autoMove = true;
        
        private void Start() {
            // Fetch the objects of the spawners
            _enemySpawner = PirateSpawner.GetInstance().gameObject;
            _asteroidSpawner = AsteroidSpawner.GetInstance().gameObject;
            _spaceStation = GameObject.FindWithTag(Tags.StationTag).transform;
            
            _miningLaser = gameObject.GetComponent<MiningLaser>();
            _laserGun = gameObject.GetComponent<LaserGun>();
            
            // Get the initial components
            _direction = transform.rotation.eulerAngles;
            _playerData = GetComponent<PlayerData>();
            _playerCollider = GetComponent<Collider>();
            _playerAgent = GetComponent<NavMeshAgent>();
            UpdateRotation();
        }

        /// <summary>
        /// Returns true if the current lock target is in range of the players mining/combat laser.
        /// </summary>
        /// <param name="lockTargetType">The current type of lock target.</param>
        public bool InLockRange(ToggleCommand.LockTargetType lockTargetType) {
            float lockRange = 0;
            if (lockTargetType == ToggleCommand.LockTargetType.Asteroid) lockRange = _miningLaser.GetMiningRange();
            if (lockTargetType == ToggleCommand.LockTargetType.Pirate) lockRange = _playerData.GetLaserRange();
            
            return Vector3.Distance(transform.position, _lockTarget.position) < lockRange;
        }

        /// <summary>
        /// Returns true if there are no objects in between the players laser gun and the target.
        /// </summary>
        /// <param name="target"></param>
        private bool HasLineOfSight(Transform target) {
            RaycastHit hit;
            var laserPosition = _laserGun.gameObject.transform.position;
            Vector3 direction = (target.transform.position - laserPosition).normalized; // The direction should be from the player to the target
            Physics.SphereCast(laserPosition, 1, direction, out hit, Vector3.Distance(laserPosition, target.position));
            if (!hit.collider) return true; // If there is no collision return true
            if (hit.collider.gameObject.transform.Equals(target)) return true; // If it collides with the target return true
            return false; // If it collides with anything else return false
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// <para>Moves the player either by directly using the transform or using the players NavMeshAgent.</para>
        /// Updates the lock target.
        /// </summary>
        private void Update() {
            if (GameManager.gameOver) return;
            // Get the speed of the player's ship
            float speed = _playerData.GetSpeed();

            // Check if destination has been reached and if not provide it to AI
            if (!HasReachedDestination()) {
                _playerAgent.SetDestination(_destination);
            } else if (speed > 0) {
                // The speed is not 0, so the ship should be moving
                transform.Translate((Time.deltaTime * speed) * _direction, Space.World);
            }
            
            // Rotate slowly
            if (rotating) Rotate();
            
            // If locked on automatically move so player is in range
            if (lockType != ToggleCommand.LockTargetType.None && _lockTarget != null) {
                if (_autoMove) {
                    AutoMove();
                }
            }
            
            // Get lock target and face it
            UpdateLockTarget();
        }

        /// <summary>
        /// <para>Updates <c>_lockTarget</c>.</para>
        /// If a lock target is found, face it. Otherwise disable the lock system.
        /// </summary>
        private void UpdateLockTarget() {
            if (lockType != ToggleCommand.LockTargetType.None) {
                if (_lockTarget == null) {
                    _lockTarget = GetLockTarget(lockType);
                    if (_lockTarget != null) {
                        _playerAgent.enabled = true;
                    }
                }

                if (_lockTarget != null) FaceTarget(_lockTarget);
                else {
                    // Disable the lock on system
                    string eventMessage = "No targets found.";
                    if (lockType == ToggleCommand.LockTargetType.Asteroid) eventMessage = "No asteroids found. Mining laser disabled.";
                    if (lockType == ToggleCommand.LockTargetType.Pirate) eventMessage = "No pirates found. Laser gun disabled.";
                    EventsManager.AddMessage(eventMessage);
                    
                    SetLockTargetType(ToggleCommand.LockTargetType.None);
                }
            } else {
                _lockTarget = null;
            }
        }

        /// <summary>
        /// Automatically moves the player using the NavMeshAgent so that they have line of sight and are in range of the lock target.
        /// </summary>
        private void AutoMove() {
            if (!(InLockRange(lockType) && HasLineOfSight(_lockTarget))) {
                if (_playerAgent.enabled) {
                    SetSpeed(1);
                    _playerAgent.SetDestination(_lockTarget.position);
                }
            } else { // If the player is in range and has line of sight, stop moving
                SetSpeed(0f);
                SetDirection(transform.forward, false);
            }
        }

        /// <summary>
        /// Calculates the players new rotation using spherical interpolation.
        /// </summary>
        private void Rotate() {
            float rotateSpeed = _playerData.GetRotateSpeed();
            Vector3 newDirection;
            
            if (_turnRight) newDirection = Vector3.Slerp(transform.forward, transform.right, rotateSpeed * Time.deltaTime);
            else newDirection = Vector3.Slerp(transform.forward, -transform.right, rotateSpeed * Time.deltaTime);
            
            transform.localRotation = Quaternion.LookRotation(newDirection);
            _direction = newDirection;
        }

        /// <summary>
        /// Returns true and stops the player if they have reached their destination.
        /// Returns true if the players NavMeshAgent is disabled.
        /// </summary>
        private bool HasReachedDestination() {
            // If AI is disabled then the destination has either been reached or there is no destination at all
            if (!_playerAgent.enabled) return true;

            // The destination was reached, stop the player ship
            if (Vector3.Distance(transform.position, _destination) < 0.2) {
                SetSpeed(0f);
                return true;
            }

            // If moving to a target check distance based on closest points of collision
            if (_hasTargetObject) {
                Vector3 closestPointTarget = _targetCollider.ClosestPoint(transform.position);
                Vector3 closestPointPlayer = _playerCollider.ClosestPoint(closestPointTarget);
                
                // The colliders are very close, stop the player ship
                if (Vector3.Distance(closestPointPlayer, closestPointTarget) < 1) {
                    SetSpeed(0f);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the nearest pirate transform to the player.
        /// </summary>
        public Transform GetNearestEnemyTransform() {
            return GetNearestTransform(GetChildren(_enemySpawner.transform));
        }

        /// <summary>
        /// Returns the nearest asteroid transform to the player.
        /// </summary>
        /// <returns></returns>
        public Transform GetNearestAsteroidTransform() {
            return GetNearestTransform(GetChildren(_asteroidSpawner.transform));
        }

        /// <summary>
        /// Returns a list of all child transforms.
        /// </summary>
        /// <param name="parent"></param>
        private static List<Transform> GetChildren(Transform parent) {
            List<Transform> children = new List<Transform>();
            
            foreach (Transform child in parent) {
                children.Add(child);
            }

            return children;
        }

        /// <summary>
        /// Returns the closest transform to the player.
        /// Returns null if no transform is closer than <c>float.PositiveInfinity</c>.
        /// </summary>
        /// <param name="transforms">The list of transforms to search.</param>
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

        /// <summary>
        /// Rotates the player using spherical interpolation to look at a target.
        /// </summary>
        /// <param name="target">Target transform to look at.</param>
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

        /// <summary>
        /// Sets the players rotation equal to the direction of travel.
        /// </summary>
        private void UpdateRotation() {
            transform.localRotation = Quaternion.LookRotation(_direction);
        }

        /// <summary>
        /// Sets the booleans that control the players rotation.
        /// </summary>
        /// <param name="targetDirection">The direction to turn. Should be equal to <c>transform.right</c> or <c>-transform.right</c>.</param>
        public void StartRotating(Vector3 targetDirection) {
            rotating = true;
            _turnRight = targetDirection == transform.right;
        }

        /// <summary>
        /// Disables the players rotation.
        /// </summary>
        public void StopRotating() {
            rotating = false;
        }
    
        /// <summary>
        /// Updates the direction variable and disables the players NavMeshAgent.
        /// <para>Updates the players rotation if rotate is true.</para>
        /// Method is used to move in a given direction.
        /// </summary>
        /// <param name="newDirection"></param>
        /// <param name="rotate"></param>
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

        /// <summary>
        /// Updates the destination variable, calculates the new direction variable and enables the players NavMeshAgent.
        /// <para>Method is used to move to a destination.</para>
        /// </summary>
        /// <param name="destination"></param>
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

        /// <summary>
        /// Sets the destination and the target object.
        /// <para>Method is used to move to a target object.</para>
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="targetCollider"></param>
        public void SetDestination(Vector3 destination, Collider targetCollider) {
            SetDestination(destination);
            
            // Set the target object collider and specify that the player is heading to an object
            _targetCollider = targetCollider;
            _hasTargetObject = true;
        }

        /// <summary>
        /// Sets the current speed to a percentage of the players maximum speed
        /// </summary>
        /// <param name="fraction">Value between 0 and 1.</param>
        public void SetSpeed(float fraction) {
            _playerData.SetSpeed(fraction);
        }

        /// <summary>
        /// Sets the type of lock target and resets the current lock target.
        /// <para>Disables/Enables laser guns and automatic movement depending on lock target type.</para>
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if type is an invalid LockTargetType.</exception>
        public void SetLockTargetType(ToggleCommand.LockTargetType type) {
            lockType = type;
            _lockTarget = null;

            switch (type) {
                case ToggleCommand.LockTargetType.Asteroid:
                    _laserGun.StopShooting();
                    _miningLaser.EnableMiningLaser();
                    _autoMove = true;
                    break;
                case ToggleCommand.LockTargetType.Pirate:
                    _laserGun.StartShooting();
                    _miningLaser.DisableMiningLaser();
                    _autoMove = true;
                    break;
                case ToggleCommand.LockTargetType.None:
                    _laserGun.StopShooting();
                    _miningLaser.DisableMiningLaser();
                    _autoMove = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        /// <summary>
        /// Returns the transform of the nearest lock target of a given type.
        /// If fog of war is enabled it only returns transforms that are visible to the player.
        /// </summary>
        /// <param name="lockTargetType"></param>
        private Transform GetLockTarget(ToggleCommand.LockTargetType lockTargetType) {
            Transform lockTarget = null;
            
            if (lockTargetType == ToggleCommand.LockTargetType.Pirate) lockTarget = GetNearestEnemyTransform();
            if (lockTargetType == ToggleCommand.LockTargetType.Asteroid) lockTarget = GetNearestAsteroidTransform();
            if (lockTarget == null) return null;

            // Player cant lock onto targets they cant see
            if (DebugSettings.FogOfWar && Vector3.Distance(transform.position, lockTarget.position) > _playerData.GetLookRadius()) lockTarget = null;

            return lockTarget;
        }

        /// <summary>
        /// Returns the transform of the current lock target.
        /// </summary>
        public Transform GetLockTarget() {
            return _lockTarget;
        }
        
        /// <summary>
        /// Returns the current type of lock target.
        /// </summary>
        public ToggleCommand.LockTargetType GetLockType() {
            return lockType;
        }

        /// <summary>
        /// Returns the distance from the player to the space station.
        /// </summary>
        public float DistanceToStation() {
            return Vector3.Distance(transform.position, _spaceStation.position);
        }
        
        /// <summary>
        /// Returns true if the player is in the same grid square as the space station.
        /// </summary>
        public bool NearStation() {
            return GridCoord.GetCoordFromVector(transform.position).Equals(GridCoord.GetCoordFromVector(_spaceStation.position));
        }

    }
}
