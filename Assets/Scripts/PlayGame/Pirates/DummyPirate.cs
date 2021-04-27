using UnityEngine;

namespace PlayGame.Pirates
{
    public class DummyPirate : MonoBehaviour
    {
        private void Start()
        {
            // The Die() method in PirateData should also be commented out in order for the dummy pirate to not die
            transform.parent = PirateSpawner.GetInstance().transform;
        }
    }
}