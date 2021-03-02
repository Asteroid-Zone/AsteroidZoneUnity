using Photon.Pun;
using PlayGame.UI;
using Statics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace PlayGame.Pirates
{
    public class PirateData : MonoBehaviour
    {
        private const float MaxHealth = 50;
        private const float Speed = 2;
        public const float LookRadius = 15;
        public const float LaserSpeed = 1000;
        private const int LaserDamageRange = 5; // Makes the amount of damage the laser does vary a bit

        private float _health;
        private int _laserDamage;
        private NavMeshAgent _pirateAgent;

        public Slider healthBar;
        public Gradient healthBarGradient;
        public Image healthBarFill;

        private void Start()
        {
            _pirateAgent = GetComponent<NavMeshAgent>();
            _health = MaxHealth;
            _laserDamage = 10;

            _pirateAgent.speed = Speed;
            healthBar.maxValue = MaxHealth;

            // Set the health bar of the pirate
            healthBar.maxValue = MaxHealth;
            SetHealthBar();
        }

        private void Die()
        {
            // TODO Play some animation
            if(!Variables.Debug) PhotonNetwork.Destroy(gameObject);
            EventsManager.AddMessageToQueue("Pirate destroyed at " + GridCoord.GetCoordFromVector(gameObject.transform.position));
        }

        private void SetHealthBar()
        {
            healthBar.value = _health;
            healthBarFill.color = healthBarGradient.Evaluate(healthBar.normalizedValue);
        }

        public void TakeDamage(float damage)
        {
            _health -= damage;
            if (_health <= 0)
            {
                Die();
            }

            // Display the damage on the health bar
            SetHealthBar();
        }

        public int GetLaserDamage()
        {
            // Make the amount of damage vary a bit
            return _laserDamage + Random.Range(-LaserDamageRange, LaserDamageRange + 1);
        }
    }
}