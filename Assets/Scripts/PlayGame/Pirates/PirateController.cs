using UnityEngine;
using UnityEngine.AI;

namespace PlayGame.Pirates
{
    public class PirateController : MonoBehaviour
    {
        public float lookRadius = 10f;

        //public Transform target;
        private NavMeshAgent _agent;
    
        // Start is called before the first frame update
        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        // Update is called once per frame
        void Update()
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
                _agent.SetDestination(closestPlayer.transform.position);

                if (closestPlayerDist <= _agent.stoppingDistance)
                {
                    FaceTarget(closestPlayer.transform);
                    // TODO: Attack the target
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
