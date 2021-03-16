using System;
using System.Collections.Generic;
using PlayGame.Player;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace PlayGame.Pirates {
    public class PirateController : MonoBehaviour {
        public SpaceStation.SpaceStation spaceStation;
        
        private int _lastFrameWentRandom;
        private const int GoRandomTimer = 600;
        private Vector3 _randomDestination;
        
        private const float StoppingDistGoingRand = 1f;
        private const float StoppingDistChasingPlayer = 4f;
        private const float StoppingDistSearchingForStation = 1f;

        private Vector3 _destination;
        private bool _alert = false; // todo unalert when not found
        
        // todo pirates attack station

        private NavMeshAgent _agent;
        private PirateLaserGun _laserGun;

        private void Start() {
            _agent = GetComponent<NavMeshAgent>();
            _laserGun = GetComponent<PirateLaserGun>();
            _randomDestination = transform.position;
            _destination = transform.position;
        }
        
        private void Update() {
            Tuple<GameObject, float> closestPlayerTuple = GetClosestPlayer();
            GameObject closestPlayer = closestPlayerTuple.Item1;
            float closestPlayerDist = closestPlayerTuple.Item2;

            GridCoord currentGridCoord = GridCoord.GetCoordFromVector(transform.position);
            
            if (closestPlayer != null && closestPlayerDist <= PirateData.LookRadius) {
                ChasePlayer(closestPlayer.transform, closestPlayerDist);
            } else if (Vector3.Distance(transform.position, spaceStation.transform.position) < PirateData.LookRadius) {
                // If pirate can see the station, alert other pirates
                if (!_alert) AlertPirates(spaceStation.transform.position);
            } else if (currentGridCoord.Equals(GridCoord.GetCoordFromVector(_destination))) { // If pirate has reached the search grid coord check if station is there
                SearchGridSquare();
            }
        }

        private void Alert(Vector3 position) {
            _destination = position;
            _agent.stoppingDistance = StoppingDistSearchingForStation;
            _laserGun.StopShooting();
            _agent.SetDestination(_destination);
        }

        private PirateController[] GetOtherPirates() {
            GameObject parent = transform.parent.gameObject;
            PirateController[] pirateControllers = parent.GetComponentsInChildren<PirateController>();
            return pirateControllers;
        }

        private void AlertPirates(Vector3 position) {
            PirateController[] pirateControllers = GetOtherPirates();

            foreach (PirateController pirateController in pirateControllers) {
                if (!pirateController.Equals(this)) pirateController.Alert(position);
            }
        }

        private void SearchGridSquare() {
            if (GridCoord.GetCoordFromVector(spaceStation.transform.position).Equals(GridCoord.GetCoordFromVector(_destination))) { // If found station
                if (!_alert) AlertPirates(spaceStation.transform.position);
            } else {
                // If station not found look somewhere else
                GridCoord newDestination = new GridCoord(Random.Range(0, GridManager.Width), Random.Range(0, GridManager.Height));
                _destination = newDestination.GetWorldVector();
                _agent.stoppingDistance = StoppingDistSearchingForStation;
                _laserGun.StopShooting();
                _agent.SetDestination(_destination);
            }
        }
        
        private void RandomMovement() {
            _agent.stoppingDistance = StoppingDistGoingRand;
            _laserGun.StopShooting();
            _agent.SetDestination(_randomDestination);
            
            // If enough time has passed set a new random destination
            if (Time.frameCount - _lastFrameWentRandom > GoRandomTimer) {
                _randomDestination = transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                _lastFrameWentRandom = Time.frameCount;
            }
        }
        
        private void ChasePlayer(Transform closestPlayer, float closestPlayerDist) {
            _agent.stoppingDistance = StoppingDistChasingPlayer;
            _agent.SetDestination(closestPlayer.position);

            if (closestPlayerDist <= _agent.stoppingDistance) {
                FaceTarget(closestPlayer);
                _laserGun.StartShooting();
            }
        }
        
        private Tuple<GameObject, float> GetClosestPlayer() {
            GameObject closestPlayer = null;
            float closestPlayerDist = float.MaxValue;
            PlayerData.Players.ForEach(p => {
                if (p != null) {
                    Vector3 position = p.transform.position;
                    float distance = Vector3.Distance(transform.position, position);
                    if (distance < closestPlayerDist) {
                        closestPlayer = p;
                        closestPlayerDist = distance;
                    }
                }
            });

            return new Tuple<GameObject, float>(closestPlayer, closestPlayerDist);
        }

        private void FaceTarget(Transform target) {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, PirateData.LookRadius);
        }
    }
}
