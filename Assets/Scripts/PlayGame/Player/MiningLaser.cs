using System.Linq;
using Photon.Pun;
using PlayGame.UI;
using Statics;
using UnityEngine;

namespace PlayGame.Player {
    public class MiningLaser : MonoBehaviour {

        private PlayerData _playerData;
        private AudioSource _miningLaserSfx;

        public AudioClip laserOn;
        public AudioClip laserOff;
        
        public LineRenderer laser;
        
        private int _lastFrameMined = 0;

        private void Start() {
            _playerData = GetComponent<PlayerData>();
            laser.positionCount = 2;
            laser.enabled = false;
            
            // Get the mining laser SFX that has the necessary tag and is a child of the current player's game object
            // Note: it should be a child of the current player, because in multiplayer it wouldn't work otherwise
            GameObject.FindGameObjectsWithTag(Tags.MiningLaserSFXTag).ToList().ForEach(miningSfx =>
                {
                    if (miningSfx.transform.parent.parent == gameObject.transform)
                    {
                        _miningLaserSfx = miningSfx.GetComponent<AudioSource>();
                    }
                });
            
            VolumeControl.AddSfxCSource(_miningLaserSfx);
            
            if (DebugSettings.InfiniteMiningRange) GameConstants.PlayerMiningRange = 10000;
        }
        
        private void Update() {
            if (laser.enabled) {
                RaycastHit hit;
                Physics.Raycast(transform.position, transform.forward, out hit, GameConstants.PlayerMiningRange); // Get the game object that the laser is hitting

                if (hit.collider) { // If the laser is hitting a game object
                    UpdateLaser((int) hit.distance);
                    if (hit.collider.gameObject.CompareTag(Tags.AsteroidTag)) {
                        if ((!DebugSettings.Debug && PhotonNetwork.IsMasterClient) || DebugSettings.Debug) MineAsteroid(hit.collider.gameObject);
                    }
                } else {
                    UpdateLaser(GameConstants.PlayerMiningRange);
                }
            }
        }

        private void MineAsteroid(GameObject asteroid) {
            if (Time.frameCount - _lastFrameMined > GameConstants.PlayerMiningDelay) { // Only mine the asteroid every x frames
                Asteroid asteroidScript = asteroid.GetComponent<Asteroid>();
                asteroidScript.MineAsteroid(GameConstants.PlayerMiningRate, _playerData);
                _lastFrameMined = Time.frameCount;

                int resources = asteroidScript.GetResources(GameConstants.PlayerMiningRate);
                if (!DebugSettings.Debug) gameObject.GetPhotonView().RPC(nameof(RPC_AddResources), RpcTarget.AllBuffered, resources);
                else _playerData.AddResources(resources);
            }
        }
        
        [PunRPC]
        public void RPC_AddResources(int resources) {  
            _playerData.AddResources(resources);
        } 
        
        private void UpdateLaser(int distance) {
            laser.SetPosition(1, new Vector3(0, 0, distance)); // Sets the end position of the laser
        }
        
        public void EnableMiningLaser() {
            if (laser.enabled) return; // If its already on dont do anything
            
            laser.enabled = true;
            
            _miningLaserSfx.clip = laserOn;
            _miningLaserSfx.Play();
        }

        public void DisableMiningLaser() {
            if (!laser.enabled) return; // If its already off dont do anything
            
            laser.enabled = false;
            
            _miningLaserSfx.clip = laserOff;
            _miningLaserSfx.Play();
        }
    }
}