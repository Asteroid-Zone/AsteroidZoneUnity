using System.Linq;
using PlayGame;
using UnityEngine;
using UnityEngine.AI;

public class PirateController : MonoBehaviour
{
    public float lookRadius = 10f;

    //public Transform target;
    private NavMeshAgent _agent;
    
    // Start is called before the first frame update
    void Start()
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
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
