using UnityEngine;

namespace PlayGame
{
    public class SpaceStation : MonoBehaviour {

        public GridManager gridManager;

        private int _stationHealth = 100;
        private int _stationShields = 100;
    
        void Start() {
            transform.position = gridManager.GetGridCentre();
        }

    }
}
