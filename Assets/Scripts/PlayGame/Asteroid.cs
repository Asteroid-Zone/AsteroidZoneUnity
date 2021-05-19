using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using PlayGame.Player;
using PlayGame.Stats;
using PlayGame.UI;
using Statics;
using UnityEngine;
using UnityEngine.AI;

namespace PlayGame {
    
    /// <summary>
    /// This class controls the behaviour of an asteroid.
    /// </summary>
    public class Asteroid : MonoBehaviour {

        /// <summary>
        /// Struct that stores an asteroid models scale and mesh.
        /// </summary>
        private readonly struct AsteroidModel {
            public readonly Vector3 Scale;
            public readonly Mesh Mesh;

            public AsteroidModel(Vector3 scale, Mesh mesh) {
                Scale = scale;
                Mesh = mesh;
            }
        }
        
        private static readonly List<AsteroidModel> AsteroidMeshes = new List<AsteroidModel>();
        private Vector3 _modelScale;

        private float _initialScale;
        private int _resourcesRemaining;
        private int _totalResources;

        private bool _asteroidDestroyed;
        
        private const float MaxScale = 4f;
        private const float MinScale = 2f;
        
        private const float FadeSpeed = 2f;
        
        private void Start() {
            // Load the asteroid meshes if they haven't been loaded
            if (AsteroidMeshes.Count == 0) {
                AsteroidMeshes.Add(GetAsteroidModel("Models/asteroid_1"));
                AsteroidMeshes.Add(GetAsteroidModel("Models/asteroid_2"));
            }

            // Select a random mesh and a random amount of resources
            int asteroidMeshIndex = Random.Range(0, AsteroidMeshes.Count);
            Quaternion rotation = Random.rotation;
            int totalResources = Random.Range(GameConstants.AsteroidMinResources, GameConstants.AsteroidMaxResources);

            if (!DebugSettings.Debug) {
                if (!PhotonNetwork.IsMasterClient) return;
                gameObject.GetPhotonView().RPC(nameof(RPC_SyncAsteroid), RpcTarget.AllBuffered, asteroidMeshIndex, rotation, totalResources);
            } else {
                SetAsteroidProperties(asteroidMeshIndex, rotation, totalResources);
                CreateNavMeshObstacle(asteroidMeshIndex);
            }
        }
        
        /// <summary>
        /// Syncs the asteroids properties and creates the NavMeshObstacle.
        /// <remarks>Method is called via RPC.</remarks>
        /// </summary>
        /// <param name="asteroidMeshIndex">The index of the asteroid mesh.</param>
        /// <param name="rotation">The Quaternion storing the rotation of the asteroid.</param>
        /// <param name="resources">The amount of resources in the asteroid.</param>
        [PunRPC]
        public void RPC_SyncAsteroid(int asteroidMeshIndex, Quaternion rotation, int resources) {
            SetAsteroidProperties(asteroidMeshIndex, rotation, resources);
            CreateNavMeshObstacle(asteroidMeshIndex);
        }

        /// <summary>
        /// This method sets the asteroids mesh, scale and rotation.
        /// It also sets the amount of resources.
        /// </summary>
        /// <param name="asteroidMeshIndex">The index of the asteroid mesh.</param>
        /// <param name="rotation">The Quaternion storing the rotation of the asteroid.</param>
        /// <param name="resources">The amount of resources in the asteroid.</param>
        private void SetAsteroidProperties(int asteroidMeshIndex, Quaternion rotation, int resources) {
            GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().mesh = AsteroidMeshes[asteroidMeshIndex].Mesh;
            _modelScale = AsteroidMeshes[asteroidMeshIndex].Scale;
            transform.rotation = rotation;

            _totalResources = resources;
            _resourcesRemaining = _totalResources;
            _initialScale = MaxScale * ((float) _totalResources / GameConstants.AsteroidMaxResources); // Initial size of the asteroid
            transform.localScale = _initialScale * _modelScale;
        }

        /// <summary>
        /// This method creates a NavMeshObstacle with the correct size.
        /// </summary>
        /// <param name="asteroidMeshIndex">The index of the asteroid mesh.</param>
        private void CreateNavMeshObstacle(int asteroidMeshIndex) {
            NavMeshObstacle navMeshObstacle = gameObject.AddComponent<NavMeshObstacle>();
            navMeshObstacle.shape = NavMeshObstacleShape.Capsule;
            navMeshObstacle.carving = true;
            navMeshObstacle.radius = (asteroidMeshIndex == 0) ? 1.2f : 1f;
        }
        
        /// <summary>
        /// Loads the asteroids mesh and scale.
        /// </summary>
        /// <param name="path">Path to the asteroids model.</param>
        private static AsteroidModel GetAsteroidModel(string path) {
            Vector3 scale = Resources.Load<GameObject>(path).transform.localScale;
            Mesh mesh = Resources.Load<Mesh>(path);
            
            return new AsteroidModel(scale, mesh);
        }

        /// <summary>
        /// This method slowly adjusts the asteroids alpha value to make it fade out.
        /// It then destroys the GameObject.
        /// <remarks>This method runs in a coroutine.</remarks>
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeOutAsteroid() {
            Material material = GetComponent<Renderer>().material;
            
            while (material.color.a > 0) {
                Color c = material.color;
                float fadeAmount = c.a - (FadeSpeed * Time.deltaTime);

                c = new Color(c.r, c.g, c.b, fadeAmount);
                material.color = c;
                yield return null;
            }

            if (!DebugSettings.Debug && gameObject != null && PhotonNetwork.IsMasterClient) 
                GetComponent<PhotonView>().RPC(nameof(DestroyOnNetwork), RpcTarget.MasterClient, gameObject.GetComponent<PhotonView>().ViewID);
            else if (DebugSettings.Debug) 
                Destroy(gameObject);
        }

        /// <summary>
        /// This method destroys the networked GameObject with the given photonID.
        /// </summary>
        /// <param name="pvID">The photonID of the GameObject to destroy.</param>
        [PunRPC]
        public void DestroyOnNetwork(int pvID) {
            if ((PhotonView.Find(pvID) == null)) return;
            PhotonNetwork.Destroy(PhotonView.Find(pvID));
        }

        /// <summary>
        /// Reduces the amount of resources and adjusts the scale.
        /// Also calls RPC_MineAsteroid via an RPC call.
        /// </summary>
        /// <param name="miningRate">Amount of resources to remove from the asteroid.</param>
        /// <param name="playerData">PlayerData of the player who mined the asteroid.</param>
        public void MineAsteroid(int miningRate, PlayerData playerData) {
            _resourcesRemaining -= miningRate;
            if (_resourcesRemaining < 0) _resourcesRemaining = 0;
            
            float scale = _initialScale * ((float) _resourcesRemaining / _totalResources); // Calculate the asteroids size based on the amount the asteroid has been mined
            if (scale < MinScale) scale = MinScale; // Asteroid is always bigger than minimum scale
            transform.localScale = _modelScale * scale;

            bool destroyed = _resourcesRemaining <= 0;

            int photonID;
            if (playerData == null) photonID = -1;
            else photonID = playerData.photonView.ViewID;
            if (!DebugSettings.Debug) gameObject.GetPhotonView().RPC(nameof(RPC_MineAsteroid), RpcTarget.AllBuffered, _resourcesRemaining, transform.localScale, destroyed, photonID);
            else {
                if (destroyed && !_asteroidDestroyed) {
                    _asteroidDestroyed = true;
                    playerData.IncreaseMiningXP(Random.Range(GameConstants.MinXPMiningComplete, GameConstants.MaxXPMiningComplete));
                    StatsManager.PlayerStatsList[0].asteroidsDestroyed++;
                    StatsManager.GameStats.asteroidsDestroyed++;
                    EventsManager.AddMessage("Asteroid destroyed at " + GridCoord.GetCoordFromVector(transform.position));
                    StartCoroutine(FadeOutAsteroid());
                }
            }
        }
        
        /// <summary>
        /// Syncs the asteroids resources and scale.
        /// If the asteroid is destroyed, increase the players xp and stats, and start the fade out coroutine.
        /// <remarks>This method is called via RPC.</remarks>
        /// </summary>
        /// <param name="resourcesRemaining">Amount of resources remaining.</param>
        /// <param name="scale">New scale of the asteroid.</param>
        /// <param name="destroyed">Whether or not the asteroid is destroyed.</param>
        /// <param name="photonID">PhotonID of the player who mined the asteroid.</param>
        [PunRPC]
        public void RPC_MineAsteroid(int resourcesRemaining, Vector3 scale, bool destroyed, int photonID) {
            _resourcesRemaining = resourcesRemaining;
            transform.localScale = scale;

            if (destroyed && !_asteroidDestroyed) {
                _asteroidDestroyed = true;
                if (photonID != -1) {
                    PlayerData.GetPlayerWithID(photonID).IncreaseMiningXP(Random.Range(GameConstants.MinXPMiningComplete, GameConstants.MaxXPMiningComplete));
                    StatsManager.GetPlayerStats(photonID).asteroidsDestroyed++;
                    StatsManager.GameStats.asteroidsDestroyed++;
                    EventsManager.AddMessage("Asteroid destroyed at " + GridCoord.GetCoordFromVector(transform.position));
                }
                StartCoroutine(FadeOutAsteroid());
            }
        }

        /// <summary>
        /// Returns the number of resources that a player would receive from mining the asteroid.
        /// </summary>
        /// <param name="miningRate">The players mining rate.</param>
        public int GetResources(int miningRate) {
            if (_resourcesRemaining > miningRate) return miningRate;
            return _resourcesRemaining;
        }

    }
}