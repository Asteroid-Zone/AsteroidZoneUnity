using UnityEngine;
using UnityEngine.AI;

namespace PlayGame.Pirates
{
    public class PirateData : MonoBehaviour
    {
        private const float MaxHealth = 50;
        private const float Speed = 2;
        public const float LookRadius = 15;
        public const float LaserSpeed = 1000;
        private const int DamageRange = 5;

        private float _health;
        private int _laserDamage;
        private NavMeshAgent _pirateAgent;

        private void Start()
        {
            _pirateAgent = GetComponent<NavMeshAgent>();
            _health = MaxHealth;
            _laserDamage = 10;

            _pirateAgent.speed = Speed;
        }

        private void Update()
        {
            if (_health == 0)
            {
                // TODO Play some animation
                Destroy(gameObject);
                EventsManager.AddMessageToQueue("Pirate destroyed at " + GridCoord.GetCoordFromVector(gameObject.transform.position));
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
            return _laserDamage + Random.Range(-DamageRange, DamageRange + 1);
        }
    }
}