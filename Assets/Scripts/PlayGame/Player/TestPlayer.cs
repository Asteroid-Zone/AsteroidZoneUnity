using Statics;
using UnityEngine;

namespace PlayGame.Player {
    
    /// <summary>
    /// This class creates a player that can be used for testing.
    /// </summary>
    public class TestPlayer : MonoBehaviour {
        #region Singleton
        private static TestPlayer _instance;

        public static void ResetStaticVariables()
        {
            _instance = null;
        }
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }

            // Enable the test player ship if Debug mode.
            if (DebugSettings.Debug)
            {
                Vector3 testPlayerSpawnPosition = GameObject.FindGameObjectWithTag(Tags.TestPlayerShipSpawnLoc).transform.position;
                _playerShipInstance = Instantiate(playerShipPrefab, testPlayerSpawnPosition, Quaternion.identity);
            }
        }

        #endregion
        
        public GameObject playerShipPrefab;
        private GameObject _playerShipInstance;

        /// <summary>
        /// Returns the players GameObject.
        /// <para>Returns null if the instance is null.</para>
        /// </summary>
        public static GameObject GetPlayerShip() {
            return _instance != null ? _instance._playerShipInstance : null;
        }
    }
}
