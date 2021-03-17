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

        private PirateType _pirateType;

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
            _pirateAgent = GetComponent<NavMeshAgent>();
            _pirateAgent.speed = _speed;
        }

        public void SetPirateType(PirateType type) {
            _pirateType = type;
            SetStats();
            
            if (_pirateAgent != null) _pirateAgent.speed = _speed;
            
            _health = _maxHealth;
            
            // Set the health bar of the pirate
            healthBar.maxValue = _maxHealth;
            SetHealthBar();
        }

        private void SetStats() {
            switch (_pirateType) {
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

        private void Die() {
            // TODO Play some animation
            if (!DebugSettings.Debug && PhotonNetwork.IsMasterClient) PhotonNetwork.Destroy(gameObject);
            else if (DebugSettings.Debug) Destroy(gameObject);
            EventsManager.AddMessage("Pirate destroyed at " + GridCoord.GetCoordFromVector(gameObject.transform.position));
        }

        private void SetHealthBar() {
            healthBar.value = _health;
            healthBarFill.color = healthBarGradient.Evaluate(healthBar.normalizedValue);
        }

        public void TakeDamage(PlayerData playerData) {
            _health -= playerData.GetLaserDamage();
            if (_health <= 0) {
                playerData.playerStats.piratesDestroyed++;
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

        public PirateType GetPirateType() {
            return _pirateType;
        }
    }
}