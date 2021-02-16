using UnityEngine;
using UnityEngine.Serialization;

namespace PlayGame {
    public class MiningLaser : MonoBehaviour {

        public LineRenderer laser;

        public void EnableMiningLaser()
        {
            laser.enabled = true;
        }

        public void DisableMiningLaser()
        {
            laser.enabled = false;
        }
    }
}