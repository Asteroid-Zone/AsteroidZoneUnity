using System;
using Photon.Pun;
using PlayGame.Player;
using PlayGame.Stats;
using PlayGame.UI;
using Statics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace PlayGame.Pirates {
    public class PirateData : MonoBehaviour {

        public enum PirateType {
            Scout,
            Elite
        }

        public PirateType pirateType;

        private float _maxHealth = 50;
        private float _speed = 2;
        private float _lookRadius = 15;
        private float _laserSpeed = 1000;
        private int _laserRange = 10;
        private int _laserDamage = 10;
        private int _laserDamageRange = 5; // Makes the amount of damage the laser does vary a bit

        private float _health;
        private NavMeshAgent _pirateAgent;

        public Slider healthBar;
        public Gradient healthBarGradient;
        public Image healthBarFill;

        public void Start() {
            SetStats();
            _pirateAgent = GetComponent<NavMeshAgent>();
            _pirateAgent.speed = _speed;
            
            _health = _maxHealth;
            
            // Set the health bar of the pirate
            healthBar.maxValue = _maxHealth;
            SetHealthBar();
        }

        private void SetStats() {
            switch (pirateType) {
                case PirateType.Scout:
                    _maxHealth = 50;
                    _speed = 2;
                    _lookRadius = 15;
                    _laserSpeed = 1000;
                    _laserRange = 10;
                    _laserDamageRange = 5;
                    _laserDamage = 10;
                    break;
                case PirateType.Elite:
                    _maxHealth = 100;
                    _speed = 1;
                    _lookRadius = 10;
                    _laserSpeed = 1000;
                    _laserRange = 15;
                    _laserDamageRange = 5;
                    _laserDamage = 15;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Leave() {
            // Todo play hyperdrive animation
            Despawn();
        }
        
        private void Die() {
            // TODO Play death animation
            EventsManager.AddMessage("Pirate destroyed at " + GridCoord.GetCoordFromVector(gameObject.transform.position));
            StatsManager.GameStats.piratesDestroyed++;

            // Unalert pirates if all of them were destroyed (this one is the last one)
            if (PirateSpawner.GetAllPirateControllers().Length == 1)
            {
                PirateController.UnalertPirates();
            }
            
            Despawn();
        }

        private void Despawn() {
            if (!DebugSettings.Debug && gameObject != null) GetComponent<PhotonView>().RPC("DestroyOnNetwork", RpcTarget.MasterClient, gameObject.GetComponent<PhotonView>().ViewID);
            else if (DebugSettings.Debug) Destroy(gameObject);
        }

        [PunRPC]
        public void DestroyOnNetwork(int pvID)
        {
            if ((PhotonView.Find(pvID) == null)) return;
            PhotonNetwork.Destroy(PhotonView.Find(pvID));
        }

        private void SetHealthBar() {
            healthBar.value = _health;
            healthBarFill.color = healthBarGradient.Evaluate(healthBar.normalizedValue);
        }

        public void TakeDamage(PlayerData playerData) {
            _health -= playerData.GetLaserDamage();
            if (_health <= 0) {
                playerData.PlayerStats.piratesDestroyed++;
                Die();
            }

            // Display the damage on the health bar
            SetHealthBar();
        }

        public int GetLaserDamage() {
            // Make the amount of damage vary a bit
            return _laserDamage + Random.Range(-_laserDamageRange, _laserDamageRange + 1);
        }

        public int GetLaserRange() {
            return _laserRange;
        }

        public float GetLookRadius() {
            return _lookRadius;
        }

        public float GetLaserSpeed() {
            return _laserSpeed;
        }
    }
}