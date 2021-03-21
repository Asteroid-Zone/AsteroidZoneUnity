using System;
using PlayGame.Player;
using Statics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace PlayGame.Pirates {
    public class PirateController : MonoBehaviour {
        public SpaceStation.SpaceStation spaceStation;
        public PirateSpawner pirateSpawner;
        
        private int _lastFrameWentRandom;
        private const int GoRandomTimer = 600;
        private Vector3 _randomDestination;
        
        private const float StoppingDistGoingRand = 1f;
        private const float StoppingDistChasingPlayer = 4f;
        private const float StoppingDistSearchingForStation = 1f;
        private const float NewAlertDistance = 9f;
        private const float DespawnDistance = 2f;

        private Vector3 _destination;
        private static bool _alert;
        private static Vector3 _knownStationLocation;

        private static GameObject _minimapAlert;

        private NavMeshAgent _agent;
        private PirateLaserGun _laserGun;
        private PirateData _pirateData;

        private void Start() {
            _agent = GetComponent<NavMeshAgent>();
            _laserGun = GetComponent<PirateLaserGun>();
            _pirateData = GetComponent<PirateData>();
            _destination = _randomDestination = transform.position;

            if (_minimapAlert == null) {
                GameObject prefab = Resources.Load<GameObject>(Prefabs.PirateAlertMinimap);
                _minimapAlert = Instantiate(prefab, transform.position, Quaternion.identity);
                _minimapAlert.SetActive(false);
            }

            if (_alert) Alert();
        }
        
        private void Update() {
            (GameObject closestPlayer, float closestPlayerDist) = GetClosestPlayer();
            float lookRadius = _pirateData.GetLookRadius();

            // If pirate is close to a player chase them
            if (closestPlayer != null && closestPlayerDist <= lookRadius) {
                ChasePlayer(closestPlayer.transform, closestPlayerDist);
                return;
            }
            
            // If pirate can see the station
            if (Vector3.Distance(transform.position, spaceStation.transform.position) < lookRadius) {
                // Alert the pirates if pirates aren't already alert or if the station is a certain distance from the known location
                if (!_alert || Vector3.Distance(spaceStation.transform.position, _destination) > NewAlertDistance) AlertPirates(spaceStation.transform.position);
            }
            
            // If pirate is at the search area check if station is there
            if (Vector3.Distance(transform.position, _destination) < lookRadius) {
                SearchGridSquare();
            }
            
            if (_alert && Vector3.Distance(transform.position, spaceStation.transform.position) < _pirateData.GetLaserRange()) ShootStation();
            if (!_alert && _pirateData.pirateType != PirateData.PirateType.Scout && Vector3.Distance(transform.position, _destination) < DespawnDistance) _pirateData.Leave();
        }

        private void ShootStation() {
            FaceTarget(spaceStation.transform);
            _laserGun.StartShooting();
        }

        private void LeaveMap() {
            _destination = GridManager.GetNearestEdgePoint(transform.position);
            _agent.stoppingDistance = 0;
            _laserGun.StopShooting();
            _agent.SetDestination(_destination);
        }

        // Go to the stations known location
        private void Alert() {
            _destination = _knownStationLocation;
            _agent.stoppingDistance = StoppingDistSearchingForStation;
            _laserGun.StopShooting();
            _agent.SetDestination(_destination);
        }
        
        // Scouts start searching again, other pirates leave
        private void Unalert() {
            if (_pirateData.pirateType == PirateData.PirateType.Scout) RandomSearch();
            else LeaveMap();
        }

        private void AlertPirates(Vector3 position) {
            PirateController[] pirateControllers = PirateSpawner.GetAllPirateControllers();
            _alert = true;
            _knownStationLocation = position;
            
            // Display minimap area
            float size = _pirateData.GetLookRadius() * 2;
            _minimapAlert.transform.localScale = new Vector3(size, size, size);
            _minimapAlert.transform.position = position;
            _minimapAlert.SetActive(true);

            foreach (PirateController pirateController in pirateControllers) {
                pirateController.Alert();
            }

            pirateSpawner.SpawnReinforcements();
        }
        
        public static void UnalertPirates() {
            // Check if the pirates were alerted at all initially
            if (!_alert)
            {
                return;
            }
            
            PirateController[] pirateControllers = PirateSpawner.GetAllPirateControllers();
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
            } else {
                // If station not found tell the other pirates and search somewhere else
                if (_alert) UnalertPirates();
                else RandomSearch();
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
            Gizmos.DrawWireSphere(transform.position, _pirateData.GetLookRadius());
        }
    }
}