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
    
    /// <summary>
    /// This class controls the pirates data.
    /// </summary>
    public class PirateData : MonoBehaviour {
        
        /// <summary>
        /// The type of pirate.
        /// </summary>
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

            // 50/50 chance of focusing on station/player
            int randomInt = Random.Range(0, 2);
            if (randomInt < 1) focusStation = false;
            
            _health = _maxHealth;

            // Set the health bar of the pirate
            healthBar.maxValue = _maxHealth;
            SetHealthBar();
        }

        /// <summary>
        /// Sets the pirates stats based on its PirateType.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if pirateType is invalid.</exception>
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

        /// <summary>
        /// <para>Method is called when a pirate reaches the edge of the grid to leave</para>
        /// Currently just despawns the pirate. In future it should play an animation.
        /// </summary>
        public void Leave() {
            // Todo play hyperdrive animation
            Despawn();
        }
        
        /// <summary>
        /// <para>Method is called when a pirate dies.</para>
        /// Unalerts the pirates if it was the last pirate then despawns.
        /// </summary>
        private void Die() {
            // TODO Play death animation
            EventsManager.AddMessage("Pirate destroyed at " + GridCoord.GetCoordFromVector(gameObject.transform.position));

            // Unalert pirates if all of them were destroyed (this one is the last one)
            if (PirateSpawner.GetAllPirateControllers().Length == 1) {
                PirateController.UnalertPirates();
            }
            
            Despawn();
        }

        /// <summary>
        /// Performs an RPC call to tell all instances to destroy the pirate.
        /// If the game is not online destroy the pirate.
        /// </summary>
        private void Despawn() {
            if (!DebugSettings.Debug && gameObject != null && PhotonNetwork.IsMasterClient) 
                GetComponent<PhotonView>().RPC(nameof(DestroyOnNetwork), RpcTarget.MasterClient, gameObject.GetComponent<PhotonView>().ViewID);
            else if (DebugSettings.Debug) Destroy(gameObject);
        }

        /// <summary>
        /// Destroys the pirate with the given photonID.
        /// </summary>
        /// <param name="pvID">PhotonID of the pirate to destroy.</param>
        [PunRPC]
        public void DestroyOnNetwork(int pvID) {
            if ((PhotonView.Find(pvID) == null)) return;
            PhotonNetwork.Destroy(PhotonView.Find(pvID));
        }

        /// <summary>
        /// Sets the value of the health bar that is displayed above the pirate.
        /// </summary>
        private void SetHealthBar() {
            healthBar.value = _health;
            healthBarFill.color = healthBarGradient.Evaluate(healthBar.normalizedValue);
        }

        /// <summary>
        /// Performs an RPC call to tell all instances to damage the pirate.
        /// If the game is not online damage the pirate and check if it dies.
        /// </summary>
        /// <param name="playerData"></param>
        public void TakeDamage(PlayerData playerData) {
            int damage = playerData.GetLaserDamage();
            if (!DebugSettings.Debug) {
                int photonID = playerData.photonView.ViewID;
                gameObject.GetPhotonView().RPC(nameof(RPC_TakeDamage), RpcTarget.AllBuffered, damage, photonID);
            } else {
                _health -= damage;
                if (_health <= 0) {
                    playerData.IncreaseCombatXP(Random.Range(GameConstants.MinXPCombatKill, GameConstants.MaxXPCombatKill));
                    
                    StatsManager.PlayerStatsList[0].piratesDestroyed++;
                    StatsManager.GameStats.piratesDestroyed++;
                    Die();
                }

                // Display the damage on the health bar
                SetHealthBar();
            }
        }
        
        /// <summary>
        /// Damage the pirate with the given photonID and check if the pirate dies.
        /// </summary>
        /// <param name="damage">The amount of damage to deal to the pirate.</param>
        /// <param name="photonID">The PhotonID of the pirate.</param>
        [PunRPC]
        public void RPC_TakeDamage(int damage, int photonID) {
            _health -= damage;
            if (_health <= 0) {
                PlayerData.GetPlayerWithID(photonID).IncreaseCombatXP(Random.Range(GameConstants.MinXPCombatKill, GameConstants.MaxXPCombatKill));
                
                StatsManager.GetPlayerStats(photonID).piratesDestroyed++;
                StatsManager.GameStats.piratesDestroyed++;
                Die();
            }

            // Display the damage on the health bar
            SetHealthBar();
        } 

        /// <summary>
        /// Returns the damage of the pirates laser.
        /// </summary>
        public int GetLaserDamage() {
            // Make the amount of damage vary a bit
            return _laserDamage + Random.Range(-_laserDamageRange, _laserDamageRange + 1);
        }

        /// <summary>
        /// Returns the range of the pirates lasers.
        /// </summary>
        public int GetLaserRange() {
            return _laserRange;
        }

        /// <summary>
        /// Returns the look radius of the pirate.
        /// </summary>
        public float GetLookRadius() {
            return _lookRadius;
        }

        /// <summary>
        /// Returns the speed of the pirates lasers.
        /// </summary>
        public float GetLaserSpeed() {
            return _laserSpeed;
        }

        /// <summary>
        /// Returns the shot delay of the pirate.
        /// </summary>
        public int GetShotDelay() {
            return _shotDelay;
        }
    }
}