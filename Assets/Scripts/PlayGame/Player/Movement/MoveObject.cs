using System;
using System.Collections.Generic;
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

        // Needed to reference enemies in order to rotate towards them
        public GameObject EnemySpawner;

        private void Start() 
        {
            // Get the initial components
            _direction = transform.rotation.eulerAngles;
            _playerData = GetComponent<PlayerData>();
            _playerCollider = GetComponent<Collider>();
            _playerAgent = GetComponent<NavMeshAgent>();
            UpdateRotation();
        }

        private void Update()
        {
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
        public Transform GetNearestEnemyTransform()
        {
            // Probably a better way to do this
            List<Transform> enemyTransforms = new List<Transform>();
            foreach (Transform child in EnemySpawner.transform)
            {
                enemyTransforms.Add(child);
            }

            float bestDistance = Single.PositiveInfinity;
            int closestEnemyIndex = -1;
            
            for (int i = 0; i < enemyTransforms.Count; i++)
            {
                float calculatedDistance =
                    Vector3.SqrMagnitude(enemyTransforms[i].transform.position - transform.position);
                if (calculatedDistance < bestDistance)
                {
                    bestDistance = calculatedDistance;
                    closestEnemyIndex = i;
                }
            }

            if (closestEnemyIndex == -1) return null;
            
            Transform closestEnemyTransform = enemyTransforms[closestEnemyIndex].transform;
            return closestEnemyTransform;
        }
        
        public void FaceTarget(Transform target)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
        }

        // Turn to face the direction the player is moving
        private void UpdateRotation() 
        {
            transform.localRotation = Quaternion.LookRotation(_direction);
        }
    
        public void SetDirection(Vector3 newDirection) 
        {
            // Set the direction to be the new direction
            _direction = newDirection;
            
            // Set the flags for player not heading anywhere
            _hasTargetObject = false;
            _playerAgent.enabled = false;
            
            // Update the rotation of the player
            UpdateRotation();
        }

        public void SetDestination(Vector3 destination) 
        {
            // Set the destination
            _destination = destination;
            
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
        public void SetSpeed(float fraction) 
        {
            _playerData.SetSpeed(fraction);
        }
    }
}
