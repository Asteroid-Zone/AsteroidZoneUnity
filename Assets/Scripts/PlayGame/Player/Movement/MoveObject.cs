﻿using System.Collections.Generic;
using System.Collections;
using PlayGame.Pirates;
using UnityEngine;
using UnityEngine.AI;

namespace PlayGame.Player.Movement
{
    public class MoveObject : MonoBehaviour {

        private Vector3 _direction;
        private Vector3 _destination = Vector3.positiveInfinity;
        private Vector3 right;

        private PlayerData _playerData;
        private NavMeshAgent _playerAgent;

        // Colliders of the player's ship and the destination object
        private Collider _playerCollider;
        private Collider _targetCollider;

        // Whether the ship is headed to a specific object
        private bool _hasTargetObject;

        public bool rotating;
        private bool _turnRight; // false = turn left, true = turn right
        private bool _BarellRoll;
        //time float for evasive moves
        private float t;
        // Needed to reference enemies in order to rotate towards them
        private GameObject _enemySpawner;

        // Needed to reference asteroids in order to rotate towards them
        private GameObject _asteroidSpawner;

        private Transform _lockTarget;

        private void Start()
        {
            // fetch the objects of the spawners
            _enemySpawner = PirateSpawner.GetInstance().gameObject;
            _asteroidSpawner = AsteroidSpawner.GetInstance().gameObject;

            // Get the initial components
            _direction = transform.rotation.eulerAngles;
            _playerData = GetComponent<PlayerData>();
            _playerCollider = GetComponent<Collider>();
            _playerAgent = GetComponent<NavMeshAgent>();

            t = 0f;
            UpdateRotation();
        }

        private void Update() {
            // Get the speed of the player's ship
            var speed = _playerData.GetSpeed();

            // Check if destination has been reached and if not provide it to AI
            if (!HasReachedDestination())
            {
                _playerAgent.SetDestination(_destination);
            }
            else if (speed > 0)
            {
                // The speed is not 0, so the ship should be moving
                transform.Translate((Time.deltaTime * speed) * _direction, Space.World);
            }

            // Rotate slowly
            if (rotating) Rotate();

            if (_lockTarget)
            {
                FaceTarget(_lockTarget);
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
              if(rotating) rotating = false;
              right = Vector3.right;
              StartCoroutine(BarellRoll());
            }
        }

        private void Rotate() {
            float rotateSpeed = _playerData.GetRotateSpeed();
            Vector3 newDirection;
            if (_turnRight) newDirection = Vector3.Slerp(transform.forward, transform.right, rotateSpeed);
            else newDirection = Vector3.Slerp(transform.forward, -transform.right, rotateSpeed);
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
                if (Vector3.Distance(closestPointPlayer, closestPointTarget) < 0.1)
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

            if (closestEnemyIndex == -1) return null;

            return transforms[closestEnemyIndex].transform;
        }

        public void FaceTarget(Transform target)
        {
            if (target == null) return; // If the target is destroyed just return.

            rotating = false;
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
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
            rotating = false;
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

        public void SetLockTarget(Transform lockTarget)
        {
            _lockTarget = lockTarget;
        }


        public IEnumerator BarellRoll()
        {
          float rotation_smoothness = 100;
          float rotation_step = 360 / rotation_smoothness;
          float time_pause = 1 / rotation_smoothness;
          for( int rotate = 0; rotate < rotation_smoothness; rotate++)
          {
            transform.Translate(right * (time_pause * _playerData.GetMaxSpeed() * 2.0f), Space.World);
            transform.Rotate(Vector3.forward, rotation_step);
            yield return new WaitForSeconds(time_pause);
          }
        }
    }
}
