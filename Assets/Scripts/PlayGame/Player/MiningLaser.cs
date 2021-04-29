using System.Linq;
using Photon.Pun;
using PlayGame.UI;
using Statics;
using UnityEngine;

namespace PlayGame.Player {
    
    /// <summary>
    /// This class controls the mining laser.
    /// </summary>
    public class MiningLaser : MonoBehaviour {

        private PlayerData _playerData;
        private AudioSource _miningLaserSfx;

        public AudioClip laserOn;
        public AudioClip laserOff;
        
        public LineRenderer laser;
        
        private float _timeSinceLastMined = 0;

        private int _miningRange;
        private int _miningRate;
        private int _miningDelay;

        private void Start() {
            _playerData = GetComponent<PlayerData>();
            laser.positionCount = 2;
            laser.enabled = false;
            
            // Get the mining laser SFX that has the necessary tag and is a child of the current player's game object
            // Note: it should be a child of the current player, because in multiplayer it wouldn't work otherwise
            GameObject.FindGameObjectsWithTag(Tags.MiningLaserSfxTag).ToList().ForEach(miningSfx => {
                if (miningSfx.transform.parent.parent == gameObject.transform) _miningLaserSfx = miningSfx.GetComponent<AudioSource>();
            });
            VolumeControl.AddSfxCSource(_miningLaserSfx);

            _miningDelay = GameConstants.PlayerMiningDelay;
            _miningRate = GameConstants.PlayerMiningRate;

            _miningRange = DebugSettings.InfiniteMiningRange ? 100000 : GameConstants.PlayerMiningRange;
        }
        
        /// <summary>
        /// Updates the time since last mined.
        /// <para>Sets the length of the mining laser and checks for collisions using raycast if the laser is on.</para>
        /// </summary>
        private void Update() {
            if (_timeSinceLastMined <= _miningDelay) _timeSinceLastMined += (Time.deltaTime * 1000);
            if (!laser.enabled) return;
            
            RaycastHit hit;
            Physics.Raycast(transform.position, transform.forward, out hit, _miningRange); // Get the game object that the laser is hitting

            if (hit.collider) { // If the laser is hitting a game object
                UpdateLaser((int) hit.distance);
                if (hit.collider.gameObject.CompareTag(Tags.AsteroidTag)) {
                    if (_timeSinceLastMined > _miningDelay) { // Only mine the asteroid every x ms
                        _playerData.IncreaseMiningXP(Random.Range(GameConstants.MinXPMiningHit, GameConstants.MaxXPMiningHit));
                        if ((!DebugSettings.Debug && PhotonNetwork.IsMasterClient) || DebugSettings.Debug) MineAsteroid(hit.collider.gameObject);
                    }
                }
            } else {
                UpdateLaser(_miningRange);
            }
        }

        /// <summary>
        /// Mines the asteroid and performs an RPC call to sync adding resources in all instances for the player. 
        /// </summary>
        /// <param name="asteroid">The asteroid to mine.</param>
        private void MineAsteroid(GameObject asteroid) {
            Asteroid asteroidScript = asteroid.GetComponent<Asteroid>();
            asteroidScript.MineAsteroid(_miningRate, _playerData);
            _timeSinceLastMined = 0;

            int resources = asteroidScript.GetResources(_miningRate);
            if (!DebugSettings.Debug) gameObject.GetPhotonView().RPC(nameof(RPC_AddResources), RpcTarget.AllBuffered, resources);
            else _playerData.AddResources(resources);
        }
        
        /// <summary>
        /// Adds resources for the player.
        /// </summary>
        /// <param name="resources">Amount of resources to add.</param>
        [PunRPC]
        public void RPC_AddResources(int resources) {  
            _playerData.AddResources(resources);
        }
        
        /// <summary>
        /// Sets the length of the laser beam.
        /// </summary>
        /// <param name="distance">Distance to the end of the laser beam.</param>
        private void UpdateLaser(int distance) {
            laser.SetPosition(1, new Vector3(0, 0, distance)); // Sets the end position of the laser
        }
        
        /// <summary>
        /// Turns the mining laser on and plays the sound effect.
        /// </summary>
        public void EnableMiningLaser() {
            if (laser.enabled) return; // If its already on dont do anything
            
            laser.enabled = true;
            
            _miningLaserSfx.clip = laserOn;
            _miningLaserSfx.Play();
        }

        /// <summary>
        /// Turns the mining laser off and plays the sound effect.
        /// </summary>
        public void DisableMiningLaser() {
            if (!laser.enabled) return; // If its already off dont do anything
            
            laser.enabled = false;
            
            _miningLaserSfx.clip = laserOff;
            _miningLaserSfx.Play();
        }

        /// <summary>
        /// Returns the mining lasers range.
        /// </summary>
        public int GetMiningRange() {
            return _miningRange;
        }
        
        /// <summary>
        /// Increases the range of the mining laser.
        /// </summary>
        /// <param name="amount">Amount to increase the range by.</param>
        public void IncreaseMiningRange(int amount) {
            _miningRange += amount;
        }

        /// <summary>
        /// Reduces the delay of the mining laser.
        /// </summary>
        /// <param name="amount">Number of ms to reduce the mining delay by.</param>
        public void ReduceMiningDelay(int amount) {
            _miningDelay -= amount;
        }
        
        /// <summary>
        /// Increases the rate of the mining laser.
        /// Mining rate is the amount of the asteroid to mine each time.
        /// </summary>
        /// <param name="amount">Amount to increase the rate by.</param>
        public void IncreaseMiningRate(int amount) {
            _miningRate += amount;
        }
    }
}