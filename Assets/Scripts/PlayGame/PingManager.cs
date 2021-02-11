﻿using UnityEngine;

namespace Assets.Scripts.PlayGame {
    
    public class PingManager : MonoBehaviour{

        private Ping _ping;

        private void Start() {
            SetPing(0, 0, PingType.None);
        }

        public Ping GetPing() {
            return _ping;
        }

        public void SetPing(int x, int z, PingType type) {
            _ping = new Ping(x, z, type);
            transform.position = _ping.GetPositionVector();

            // Hide ping if type is None
            transform.gameObject.SetActive(_ping.GetPingType() != PingType.None);
        }

    }
}