using PlayGame.Player;
using UnityEngine;
using UnityEngine.AI;

namespace PlayGame.Pirates
{
    public class PirateController : MonoBehaviour
    {
        public float lookRadius = 10f;
        private int _lastFrameWentRandom;
        private const int GoRandomTimer = 600;
        private Vector3 _randomDestination;
        private const float StoppingDistGoingRand = 1f;
        private const float StoppingDistChasingPlayer = 4f;

        private NavMeshAgent _agent;
        private PirateLaserGun _laserGun;
    
        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _laserGun = GetComponent<PirateLaserGun>();
            _randomDestination = transform.position;
        }

        private void Update()
        {
            GameObject closestPlayer = null;
            float closestPlayerDist = float.MaxValue;
            PlayerData.Players.ForEach(p =>
            {
                var position = p.transform.position;
                var distance = Vector3.Distance(transform.position, position);
                if (distance < closestPlayerDist)
                {
                    closestPlayer = p;
                    closestPlayerDist = distance;
                }
            });

            if (closestPlayer != null && closestPlayerDist <= lookRadius)
            {
                _agent.stoppingDistance = StoppingDistChasingPlayer;
                _agent.SetDestination(closestPlayer.transform.position);

                if (closestPlayerDist <= _agent.stoppingDistance)
                {
                    FaceTarget(closestPlayer.transform);
                    _laserGun.StartShooting();
                }
            }
            else
            {
                _agent.stoppingDistance = StoppingDistGoingRand;
                _laserGun.StopShooting();
                _agent.SetDestination(_randomDestination);
                    
                if (Time.frameCount - _lastFrameWentRandom > GoRandomTimer)
                {
                    _randomDestination = transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                    _lastFrameWentRandom = Time.frameCount;
                }
            }
        }

        private void FaceTarget(Transform target)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, lookRadius);
        }
    }
}
