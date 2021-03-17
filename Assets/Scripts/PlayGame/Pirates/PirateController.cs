using System;
using PlayGame.Player;
using Statics;
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
        private const float NewAlertDistance = 9f;

        private Vector3 _destination;
        private bool _searching = true;
        private static bool _alert = false;

        private static GameObject _minimapAlert = null;
        
        // todo pirates attack station

        private NavMeshAgent _agent;
        private PirateLaserGun _laserGun;

        private void Start() {
            _agent = GetComponent<NavMeshAgent>();
            _laserGun = GetComponent<PirateLaserGun>();
            _randomDestination = transform.position;
            _destination = transform.position;

            if (_minimapAlert == null) {
                GameObject prefab = Resources.Load<GameObject>(Prefabs.PirateMinimapAlert);
                _minimapAlert = Instantiate(prefab, transform.position, Quaternion.identity);
                _minimapAlert.SetActive(false);
            }
        }
        
        private void Update() {
            Tuple<GameObject, float> closestPlayerTuple = GetClosestPlayer();
            GameObject closestPlayer = closestPlayerTuple.Item1;
            float closestPlayerDist = closestPlayerTuple.Item2;

            if (closestPlayer != null && closestPlayerDist <= PirateData.LookRadius) { // If pirate is close to a player chase them
                ChasePlayer(closestPlayer.transform, closestPlayerDist);
            } else {
                if (_searching) { // Search for station
                    if (Vector3.Distance(transform.position, spaceStation.transform.position) < PirateData.LookRadius) { // If pirate can see the station
                        // Alert the pirates if pirates aren't already alert or if the station is a certain distance from the known location
                        if (!_alert || Vector3.Distance(spaceStation.transform.position, _destination) > NewAlertDistance) AlertPirates(spaceStation.transform.position);
                        _searching = false;
                    } else if (Vector3.Distance(transform.position, _destination) < PirateData.LookRadius) {
                        // If pirate has reached the search area check if station is there
                        SearchGridSquare();
                    }
                } else {
                    if (Vector3.Distance(transform.position, spaceStation.transform.position) < PirateData.LaserRange) ShootStation();
                }
            }
        }

        private void ShootStation() {
            FaceTarget(spaceStation.transform);
            _laserGun.StartShooting();
        }

        // Go to the stations known location
        private void Alert(Vector3 position) {
            _destination = position;
            _agent.stoppingDistance = StoppingDistSearchingForStation;
            _laserGun.StopShooting();
            _agent.SetDestination(_destination);
        }
        
        // Search randomly
        private void Unalert() {
            RandomSearch();
        }

        private PirateController[] GetOtherPirates() {
            GameObject parent = transform.parent.gameObject;
            PirateController[] pirateControllers = parent.GetComponentsInChildren<PirateController>();
            return pirateControllers;
        }

        private void AlertPirates(Vector3 position) {
            PirateController[] pirateControllers = GetOtherPirates();
            _alert = true;
            _minimapAlert.transform.localScale = new Vector3(PirateData.LookRadius, PirateData.LookRadius, PirateData.LookRadius);
            _minimapAlert.transform.position = position;
            _minimapAlert.SetActive(true);

            foreach (PirateController pirateController in pirateControllers) {
                pirateController.Alert(position);
            }
        }
        
        private void UnalertPirates() {
            PirateController[] pirateControllers = GetOtherPirates();
            _alert = false;
            _minimapAlert.SetActive(false);

            foreach (PirateController pirateController in pirateControllers) {
                pirateController.Unalert();
            }
        }

        // Search a random grid coordinate
        private void RandomSearch() {
            GridCoord newDestination = new GridCoord(Random.Range(0, GridManager.Width), Random.Range(0, GridManager.Height));
            _destination = newDestination.GetWorldVector();
            _agent.stoppingDistance = StoppingDistSearchingForStation;
            _laserGun.StopShooting();
            _agent.SetDestination(_destination);
        }

        private void SearchGridSquare() {
            if (GridCoord.GetCoordFromVector(spaceStation.transform.position).Equals(GridCoord.GetCoordFromVector(_destination))) { // If found station
                if (!_alert) AlertPirates(spaceStation.transform.position);
                _searching = false;
            } else {
                _searching = true;
                // If station not found tell the other pirates and search somewhere else
                UnalertPirates();
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
