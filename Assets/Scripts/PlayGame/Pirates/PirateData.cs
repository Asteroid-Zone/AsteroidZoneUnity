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
        private int _shotDelay = 100; // Number of frames to wait between shots

        public bool focusStation = true;
        
        private float _health;
        private NavMeshAgent _pirateAgent;

        public Slider healthBar;
        public Gradient healthBarGradient;
        public Image healthBarFill;

        public void Start() {
            SetStats();
            _pirateAgent = GetComponent<NavMeshAgent>();
            _pirateAgent.speed = _speed;

            int randomInt = Random.Range(0, 2);
            if (randomInt < 1) focusStation = false;
            
            _health = _maxHealth;

            // Set the health bar of the pirate
            healthBar.maxValue = _maxHealth;
            SetHealthBar();
        }

        private void SetStats() {
            switch (pirateType) {
                case PirateType.Scout:
                    _maxHealth = GameConstants.PirateScoutMaxHealth;
                    _speed = GameConstants.PirateScoutSpeed;
                    _lookRadius = GameConstants.PirateScoutLookRadius;
                    _laserSpeed = GameConstants.PirateScoutLaserSpeed;
                    _laserRange = GameConstants.PirateScoutLaserRange;
                    _laserDamageRange = GameConstants.PirateScoutLaserDamageRange;
                    _laserDamage = GameConstants.PirateScoutLaserDamage;
                    _shotDelay = GameConstants.PirateScoutShotDelay;
                    break;
                case PirateType.Elite:
                    _maxHealth = GameConstants.PirateEliteMaxHealth;
                    _speed = GameConstants.PirateEliteSpeed;
                    _lookRadius = GameConstants.PirateEliteLookRadius;
                    _laserSpeed = GameConstants.PirateEliteLaserSpeed;
                    _laserRange = GameConstants.PirateEliteLaserRange;
                    _laserDamageRange = GameConstants.PirateEliteLaserDamageRange;
                    _laserDamage = GameConstants.PirateEliteLaserDamage;
                    _shotDelay = GameConstants.PirateEliteShotDelay;
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

            // Unalert pirates if all of them were destroyed (this one is the last one)
            if (PirateSpawner.GetAllPirateControllers().Length == 1)
            {
                PirateController.UnalertPirates();
            }
            
            Despawn();
        }

        private void Despawn() {
            if (!DebugSettings.Debug && gameObject != null && PhotonNetwork.IsMasterClient) 
                GetComponent<PhotonView>().RPC(nameof(DestroyOnNetwork), RpcTarget.MasterClient, gameObject.GetComponent<PhotonView>().ViewID);
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
            int damage = playerData.GetLaserDamage();
            if (!DebugSettings.Debug) {
                int photonID = playerData.photonView.ViewID;
                gameObject.GetPhotonView().RPC(nameof(RPC_TakeDamage), RpcTarget.AllBuffered, damage, photonID);
            } else {
                _health -= damage;
                if (_health <= 0) {
                    StatsManager.PlayerStatsList[0].piratesDestroyed++;
                    StatsManager.GameStats.piratesDestroyed++;
                    Die();
                }

                // Display the damage on the health bar
                SetHealthBar();
            }
        }
        
        [PunRPC]
        public void RPC_TakeDamage(int damage, int photonID) {
            _health -= damage;
            if (_health <= 0) {
                StatsManager.GetPlayerStats(photonID).piratesDestroyed++;
                StatsManager.GameStats.piratesDestroyed++;
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

        public int GetShotDelay() {
            return _shotDelay;
        }
    }
}