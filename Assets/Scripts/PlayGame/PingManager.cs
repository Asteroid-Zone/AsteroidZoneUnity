using UnityEngine;

namespace PlayGame {
    
    public class PingManager : MonoBehaviour{

        private Ping _ping;

        private void Start() {
            SetPing('A', 0, PingType.None);
        }

        public Ping GetPing() {
            return _ping;
        }
        
        private void UpdatePing() {
            transform.position = _ping.GetPositionVector();
            transform.gameObject.SetActive(_ping.GetPingType() != PingType.None); // Hide ping if type is None
        }

        public void SetPing(char x, int z, PingType type) {
            _ping = new Ping(x, z, type);
            UpdatePing();
        }

        public void SetPing(Ping ping) {
            _ping = ping;
            UpdatePing();
        }

    }
}