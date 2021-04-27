﻿using System.Linq;
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
            GameObject.FindGameObjectsWithTag(Tags.MiningLaserSfxTag).ToList().ForEach(miningSfx =>
                {
                    if (miningSfx.transform.parent.parent == gameObject.transform)
                    {
                        _miningLaserSfx = miningSfx.GetComponent<AudioSource>();
                    }
                });
            
            VolumeControl.AddSfxCSource(_miningLaserSfx);

            _miningDelay = GameConstants.PlayerMiningDelay;
            _miningRate = GameConstants.PlayerMiningRate;

            if (DebugSettings.InfiniteMiningRange) _miningRange = 100000;
            else _miningRange = GameConstants.PlayerMiningRange;
        }
        
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

        private void MineAsteroid(GameObject asteroid) {
            Asteroid asteroidScript = asteroid.GetComponent<Asteroid>();
            asteroidScript.MineAsteroid(_miningRate, _playerData);
            _timeSinceLastMined = 0;

            int resources = asteroidScript.GetResources(_miningRate);
            if (!DebugSettings.Debug) gameObject.GetPhotonView().RPC(nameof(RPC_AddResources), RpcTarget.AllBuffered, resources);
            else _playerData.AddResources(resources);
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

        public void IncreaseMiningRange(int amount) {
            _miningRange += amount;
        }

        public void ReduceMiningDelay(int amount) {
            _miningDelay -= amount;
        }

        public void IncreaseMiningRate(int amount) {
            _miningRate += amount;
        }
    }
}