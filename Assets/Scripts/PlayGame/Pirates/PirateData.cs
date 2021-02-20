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

        private void Start()
        {
            _pirateAgent = GetComponent<NavMeshAgent>();
            _health = MaxHealth;
            _laserDamage = 10;

            _pirateAgent.speed = Speed;
            healthBar.maxValue = MaxHealth;
        }

        private void Update()
        {
            if (_health == 0)
            {
                // TODO Play some animation
                Destroy(gameObject);
                EventsManager.AddMessageToQueue("Pirate destroyed at " + GridCoord.GetCoordFromVector(gameObject.transform.position));
            }
            else
            {
                healthBar.value = _health;
            }
        }

        public void TakeDamage(float damage)
        {
            _health -= damage;
            if (_health <= 0)
            {
                _health = 0;
            }
        }

        public int GetLaserDamage()
        {
            // Make the amount of damage vary a bit
            return _laserDamage + Random.Range(-LaserDamageRange, LaserDamageRange + 1);
        }
    }
}