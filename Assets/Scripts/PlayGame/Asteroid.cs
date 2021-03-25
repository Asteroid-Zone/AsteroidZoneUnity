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
    public class Asteroid : MonoBehaviour {

        private readonly struct AsteroidModel {
            public readonly Vector3 scale;
            public readonly Mesh mesh;

            public AsteroidModel(Vector3 scale, Mesh mesh) {
                this.scale = scale;
                this.mesh = mesh;
            }
        }
        
        private static readonly List<AsteroidModel> AsteroidMeshes = new List<AsteroidModel>();
        private Vector3 _modelScale;

        private float _initialScale;
        private int _resourcesRemaining;
        private int _totalResources;

        private bool _asteroidDestroyed;
        
        private const float MaxScale = 6f;
        private const float MinScale = 2f;

        private const int MinResources = 25;
        private const int MaxResources = 100;
        
        private const float FadeSpeed = 2f;
        
        private void Start() {
            if (AsteroidMeshes.Count == 0) {
                AsteroidMeshes.Add(GetAsteroidModel("Models/asteroid_1"));
                AsteroidMeshes.Add(GetAsteroidModel("Models/asteroid_2"));
            }

            int asteroidMeshIndex = Random.Range(0, AsteroidMeshes.Count);
            GetComponent<MeshFilter>().mesh = AsteroidMeshes[asteroidMeshIndex].mesh;
            GetComponent<MeshCollider>().sharedMesh = AsteroidMeshes[asteroidMeshIndex].mesh;
            _modelScale = AsteroidMeshes[asteroidMeshIndex].scale;
            transform.rotation = Random.rotation;
            CreateNavMeshObstacle();

            _totalResources = Random.Range(MinResources, MaxResources);
            _resourcesRemaining = _totalResources;
            _initialScale = MaxScale * ((float) _totalResources / MaxResources); // Initial size of the asteroid
            transform.localScale = _initialScale * _modelScale;
        }

        private void CreateNavMeshObstacle()
        {
            NavMeshObstacle navMeshObstacle = gameObject.AddComponent<NavMeshObstacle>();
            navMeshObstacle.radius = 0.6f;
            navMeshObstacle.shape = NavMeshObstacleShape.Capsule;
            navMeshObstacle.carving = true;
        }
        
        private static AsteroidModel GetAsteroidModel(string path) {
            Vector3 scale = Resources.Load<GameObject>(path).transform.localScale;
            Mesh mesh = Resources.Load<Mesh>(path);
            
            return new AsteroidModel(scale, mesh);
        }

        private IEnumerator FadeOutAsteroid() {
            Material material = GetComponent<Renderer>().material;
            
            while (material.color.a > 0) {
                Color c = material.color;
                float fadeAmount = c.a - (FadeSpeed * Time.deltaTime);

                c = new Color(c.r, c.g, c.b, fadeAmount);
                material.color = c;
                yield return null;
            }

            if (!DebugSettings.Debug && gameObject != null && PhotonNetwork.IsMasterClient) GetComponent<PhotonView>().RPC("DestroyOnNetwork", RpcTarget.MasterClient, gameObject.GetComponent<PhotonView>().ViewID);
            else if (DebugSettings.Debug) Destroy(gameObject);
        }

        [PunRPC]
        public void DestroyOnNetwork(int pvID)
        {
            if ((PhotonView.Find(pvID) == null)) return;
            PhotonNetwork.Destroy(PhotonView.Find(pvID));
        }

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
            gameObject.GetPhotonView().RPC("RPC_MineAsteroid", RpcTarget.AllBuffered, _resourcesRemaining, transform.localScale, destroyed, photonID);
        }
        
        [PunRPC]
        public void RPC_MineAsteroid(int resourcesRemaining, Vector3 scale, bool destroyed, int photonID) {
            _resourcesRemaining = resourcesRemaining;
            transform.localScale = scale;

            if (destroyed && !_asteroidDestroyed) {
                _asteroidDestroyed = destroyed;
                if (photonID != -1) {
                    StatsManager.GetPlayerStats(photonID).asteroidsDestroyed++;
                    StatsManager.GameStats.asteroidsDestroyed++;
                    EventsManager.AddMessage("Asteroid destroyed at " + GridCoord.GetCoordFromVector(transform.position));
                }
                StartCoroutine(FadeOutAsteroid());
            }
        } 

        public int GetResources(int miningRate) {
            if (_resourcesRemaining > miningRate) return miningRate;
            return _resourcesRemaining;
        }

    }
}