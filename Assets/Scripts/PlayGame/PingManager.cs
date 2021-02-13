using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PlayGame {
    

    public class PingManager : MonoBehaviour{
        private const int PingTimeInSeconds = 10;

        private Dictionary<Ping, GameObject> _pings;

        private void Start() {
            //SetPing('A', 0, PingType.None);
            _pings = new Dictionary<Ping, GameObject>();
        }

        public void AddPing(Ping ping)
        {
            // Remove all old pings with the same location (should be at most one old ping, but just to make sure it is implemented for many)
            var sameLocPings = _pings.ToList().Where(kvp => kvp.Key.GetGridCoord().Equals(ping.GetGridCoord())).ToList();
            foreach (var sameLocPing in sameLocPings)
            {
                RemovePing(sameLocPing.Key);
            }

            GameObject pingObject = CreateObjectForPing(ping);
            _pings[ping] = pingObject;
            
            pingObject.transform.position = ping.GetPositionVector();
            
            // Remove the new ping after a specific amount of time
            Task.Factory.StartNew(() => Thread.Sleep(PingTimeInSeconds * 1000))
                .ContinueWith(t => RemovePing(ping));
        }

        private void RemovePing(Ping ping)
        {
            if (_pings.ContainsKey(ping))
            {
                DestroyImmediate(_pings[ping]);
            }
            
            _pings.Remove(ping);
        }

        private static GameObject CreateObjectForPing(Ping ping)
        {
            GameObject pingObject;
            switch (ping.GetPingType())
            {
                case PingType.Asteroid:
                    pingObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    break;
                case PingType.Pirate:
                    pingObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    break;
                default:
                    pingObject = null;
                    break;
            }

            if (pingObject)
            {
                pingObject.transform.localScale = new Vector3(4,4,4);
                pingObject.layer = LayerMask.NameToLayer("Minimap");
            }
            
            return pingObject;
        }

    }
}