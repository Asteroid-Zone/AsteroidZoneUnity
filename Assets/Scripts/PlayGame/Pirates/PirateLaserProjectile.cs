﻿using PlayGame.Player;
using Statics;
using UnityEngine;

namespace PlayGame.Pirates {

    public class PirateLaserProjectile : MonoBehaviour
    {
        private const int MaxRange = 10;
        
        private Vector3 _startPosition;

        private PirateData _shootingPirateData;

        private void Start() {
            _startPosition = transform.position;
        }

        private void Update() {
            if (Vector3.Distance(transform.position, _startPosition) > MaxRange) Destroy(gameObject); // Limit the lasers range
        }

        private void OnCollisionEnter(Collision collision) {
            if (collision.gameObject.CompareTag(Tags.PirateTag)) return;

            if (collision.gameObject.CompareTag(Tags.PlayerTag))
            {
                PlayerData playerData = collision.gameObject.GetComponent<PlayerData>();
                playerData.TakeDamage(_shootingPirateData.GetLaserDamage());
            }
            
            if (collision.gameObject.CompareTag(Tags.AsteroidTag)) {
                Asteroid asteroid = collision.gameObject.GetComponent<Asteroid>();
                asteroid.MineAsteroid();
            }

            Destroy(gameObject);
        }

        public void SetShootingPirateData(PirateData shootingPirateData)
        {
            _shootingPirateData = shootingPirateData;
        }
    }
}
