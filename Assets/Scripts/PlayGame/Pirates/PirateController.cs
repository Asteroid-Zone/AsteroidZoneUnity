using System;
using PlayGame.Player;
using Statics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace PlayGame.Pirates {
    
    /// <summary>
    /// This class controls the pirates behaviour.
    /// </summary>
    public class PirateController : MonoBehaviour {
        
        private Transform _spaceStation;
        public PirateSpawner pirateSpawner;
        
        private int _lastFrameWentRandom;
        private const int GoRandomTimer = 600;
        private Vector3 _randomDestination;
        
        private const float StoppingDistGoingRand = 1f;
        private const float StoppingDistChasingPlayer = 6f;
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

        private bool _chasingPlayer = false;

        /// <summary>
        /// Resets all of the static variables.
        /// </summary>
        public static void ResetStaticVariables() {
            _alert = false;
            _knownStationLocation = Vector3.zero;
            _minimapAlert = null;
        }
        
        private void Start() {
            _spaceStation = GameObject.FindGameObjectWithTag(Tags.StationTag).transform;
            _agent = GetComponent<NavMeshAgent>();
            _laserGun = GetComponent<PirateLaserGun>();
            _pirateData = GetComponent<PirateData>();
            _destination = _randomDestination = transform.position;

            // Create the minimap alert sphere if it has not already been created
            if (_minimapAlert == null) {
                GameObject prefab = Resources.Load<GameObject>(Prefabs.PirateAlertMinimap);
                _minimapAlert = Instantiate(prefab, transform.position, Quaternion.identity);
                _minimapAlert.SetActive(false);
            }

            if (_alert) Alert(); // If the pirates know the location of the space station alert this pirate
        }
        
        /// <summary>
        /// <para>Decides which actions the pirate will perform this frame.</para>
        /// <para></para>
        /// If a pirate was chasing a player that escaped them they start searching for the station again.
        /// <para>If a pirate finds the station it alerts all other pirates.</para>
        /// If a pirate is at its search location it checks if the station is there.
        /// <para>If a pirate is close enough to shoot a player/station it does so, priority is based on the pirates 'personality'.</para>
        /// If the pirate is an elite pirate and not alert it leaves the map.
        /// </summary>
        private void Update() {
            (GameObject closestPlayer, float closestPlayerDist) = GetClosestPlayer();
            float lookRadius = _pirateData.GetLookRadius();

            // If the player has escaped start searching for the station again
            if (_chasingPlayer && closestPlayer == null) {
                _chasingPlayer = false;
                RandomSearch();
            }
            
            // If pirate can see the station
            if (Vector3.Distance(transform.position, _spaceStation.position) < lookRadius) {
                // Alert the pirates if pirates aren't already alert or if the station is a certain distance from the known location
                if (!_alert || Vector3.Distance(_spaceStation.position, _destination) > NewAlertDistance) AlertPirates(_spaceStation.position);
            }
            
            // If pirate is at the search area check if station is there
            if (Vector3.Distance(transform.position, _destination) < lookRadius) {
                SearchGridSquare();
            }

            // Some pirates focus on the station and some focus on the player
            if (_pirateData.focusStation) {
                if (_alert && Vector3.Distance(transform.position, _spaceStation.position) < _pirateData.GetLaserRange()) {
                    _chasingPlayer = false;
                    ShootTarget(_spaceStation);
                } else if (closestPlayer != null && closestPlayerDist <= lookRadius) ChasePlayer(closestPlayer.transform, closestPlayerDist); // If pirate is close to a player chase them
                else if (!_alert && _pirateData.pirateType != PirateData.PirateType.Scout && Vector3.Distance(transform.position, _destination) < DespawnDistance) _pirateData.Leave();
            } else {
                if (closestPlayer != null && closestPlayerDist <= lookRadius) ChasePlayer(closestPlayer.transform, closestPlayerDist); // If pirate is close to a player chase them
                else if (_alert && Vector3.Distance(transform.position, _spaceStation.position) < _pirateData.GetLaserRange()) {
                    _chasingPlayer = false;
                    ShootTarget(_spaceStation);
                }
                else if (!_alert && _pirateData.pirateType != PirateData.PirateType.Scout && Vector3.Distance(transform.position, _destination) < DespawnDistance) _pirateData.Leave();
            }
        }

        /// <summary>
        /// Face the target and start shooting.
        /// </summary>
        /// <param name="target"></param>
        private void ShootTarget(Transform target) {
            FaceTarget(target);
            _laserGun.StartShooting();
        }

        /// <summary>
        /// Sets the destination to the nearest point on the edge of the grid.
        /// </summary>
        private void LeaveMap() {
            _chasingPlayer = false;
            _destination = GridManager.GetNearestEdgePoint(transform.position);
            _agent.stoppingDistance = 0;
            _laserGun.StopShooting();
            _agent.SetDestination(_destination);
        }

        /// <summary>
        /// Sets the destination to the stations known location
        /// </summary>
        private void Alert() {
            _destination = _knownStationLocation;
            _agent.stoppingDistance = StoppingDistSearchingForStation;
            _laserGun.StopShooting();
            _agent.SetDestination(_destination);
        }
        
        /// <summary>
        /// <para>Method is called when the station moves or all pirates are killed.</para>
        /// Scout pirates start searching for the station again.
        /// Elite pirates leave the grid.
        /// </summary>
        private void Unalert() {
            if (_pirateData.pirateType == PirateData.PirateType.Scout) RandomSearch();
            else LeaveMap();
        }

        /// <summary>
        /// Calls the Alert method for all pirates, displays the minimap alert object and spawns reinforcements.
        /// </summary>
        /// <param name="position"></param>
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
        
        /// <summary>
        /// Calls the Unalert method for all pirates and disables the minimap alert object.
        /// </summary>
        public static void UnalertPirates() {
            // Check if the pirates were alerted at all initially
            if (!_alert) {
                return;
            }
            
            PirateController[] pirateControllers = PirateSpawner.GetAllPirateControllers();
            _alert = false;
            _minimapAlert.SetActive(false);

            foreach (PirateController pirateController in pirateControllers) {
                pirateController.Unalert();
            }
        }

        /// <summary>
        /// Sets the pirates destination to a random grid coordinate that they will search.
        /// </summary>
        private void RandomSearch() {
            GridCoord newDestination = new GridCoord(Random.Range(0, GameConstants.GridWidth), Random.Range(0, GameConstants.GridHeight));
            _destination = newDestination.GetWorldVector();
            _agent.stoppingDistance = StoppingDistSearchingForStation;
            _laserGun.StopShooting();
            _agent.SetDestination(_destination);
        }

        /// <summary>
        /// Method is called when a pirate reaches its grid coordinate to search.
        /// If the station is found alert the other pirates.
        /// Otherwise unalert them and start a new random search.
        /// </summary>
        private void SearchGridSquare() {
            if (GridCoord.GetCoordFromVector(_spaceStation.position).Equals(GridCoord.GetCoordFromVector(_destination))) { // If found station
                if (!_alert) AlertPirates(_spaceStation.position);
            } else {
                // If station not found tell the other pirates and search somewhere else
                if (_alert) UnalertPirates();
                else RandomSearch();
            }
        }
        
        /// <summary>
        /// Move randomly.
        /// If enough time has passed set a new random destination.
        /// </summary>
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
        
        /// <summary>
        /// Follows the player.
        /// If the pirate is within range shoot the player.
        /// </summary>
        /// <param name="closestPlayer"></param>
        /// <param name="closestPlayerDist"></param>
        private void ChasePlayer(Transform closestPlayer, float closestPlayerDist) {
            _chasingPlayer = true;
            _agent.stoppingDistance = StoppingDistChasingPlayer;
            _agent.SetDestination(closestPlayer.position);

            if (closestPlayerDist <= _pirateData.GetLaserRange()) {
                ShootTarget(closestPlayer);
            }
        }
        
        /// <summary>
        /// Returns the closest player to this pirate.
        /// </summary>
        private Tuple<GameObject, float> GetClosestPlayer() {
            GameObject closestPlayer = null;
            float closestPlayerDist = float.MaxValue;
            PlayerData.Players.ForEach(p => {
                if (p != null) {
                    PlayerData playerData = p.GetComponent<PlayerData>();
                    if (playerData.GetRole() != Role.StationCommander && playerData.dead == false) {
                        Vector3 position = p.transform.position;
                        float distance = Vector3.Distance(transform.position, position);
                        if (distance < closestPlayerDist) {
                            closestPlayer = p;
                            closestPlayerDist = distance;
                        }
                    }
                }
            });

            return new Tuple<GameObject, float>(closestPlayer, closestPlayerDist);
        }

        /// <summary>
        /// Spherically interpolates the rotation of the pirate to face the target.
        /// </summary>
        /// <param name="target"></param>
        private void FaceTarget(Transform target) {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
        }

        /// <summary>
        /// Draws Gizmos for the pirates look radius.
        /// </summary>
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            float gizmosRadius = 15f;
            if (_pirateData != null) {
                gizmosRadius = _pirateData.GetLookRadius();
            }
            
            Gizmos.DrawWireSphere(transform.position, gizmosRadius);
        }
    }
}