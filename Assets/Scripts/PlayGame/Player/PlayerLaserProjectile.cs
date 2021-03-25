﻿using Photon.Pun;
using PlayGame.Pirates;
using Statics;
using UnityEngine;

namespace PlayGame.Player {
  
    public class PlayerLaserProjectile : MonoBehaviour {
        
        private const int MiningRate = 4; // Amount of resources gathered every mining tick
        private const int MaxRange = 20;
        
        private Vector3 _startPosition;
        private PlayerData _shootingPlayerData;

        private void Start() {
            _startPosition = transform.position;
        }

        private void Update() {
            if (Vector3.Distance(transform.position, _startPosition) > MaxRange) Destroy(gameObject); // Limit the lasers range
        }

        private void OnCollisionEnter(Collision collision) {
            if (collision.gameObject == _shootingPlayerData.gameObject) return;
            if (collision.gameObject.CompareTag(Tags.CombatLaserTag)) return;
            
            // todo play animation (explosion)

            if (collision.gameObject.CompareTag(Tags.PirateTag)) {
                if (PhotonNetwork.IsMasterClient) {
                    PirateData pirateData = collision.gameObject.GetComponent<PirateData>();
                    pirateData.TakeDamage(_shootingPlayerData);
                }
            }
            
            if (collision.gameObject.CompareTag(Tags.AsteroidTag)) {
                Asteroid asteroid = collision.gameObject.GetComponent<Asteroid>();
                asteroid.MineAsteroid(MiningRate, _shootingPlayerData);
            }

            Destroy(gameObject);
        }

        public void SetShootingPlayerData(PlayerData shootingPlayerData) {
            _shootingPlayerData = shootingPlayerData;
        }
    }
}
