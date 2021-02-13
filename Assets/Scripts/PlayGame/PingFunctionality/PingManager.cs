using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlayGame.PingFunctionality {

    public class PingManager : MonoBehaviour{
        private const int PingTimeInSeconds = 10;

        private Dictionary<Ping, GameObject> _pings;

        private void Start() {
            // Initialise dictionary of pings
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

            // Create a new object for the new ping at the specific position on the map
            GameObject pingObject = CreateObjectForPing(ping);
            _pings[ping] = pingObject;

            // Remove the new ping after a specific amount of time
            StartCoroutine(RemovePingAfterTime(ping));
        }

        public Dictionary<Ping, GameObject> GetPings()
        {
            return _pings;
        }

        private IEnumerator RemovePingAfterTime(Ping ping)
        {
            // Wait for a specific amount of time and remove the ping (including the game object)
            yield return new WaitForSeconds(PingTimeInSeconds);
            RemovePing(ping);
        }

        private void RemovePing(Ping ping)
        {
            if (_pings.ContainsKey(ping))
            {
                // Remove ping game object
                DestroyImmediate(_pings[ping]);
            }
            
            _pings.Remove(ping);
        }

        private GameObject CreateObjectForPing(Ping ping)
        {
            GameObject pingObject;
            
            // Create a game object according to the ping type
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

            if (null != pingObject)
            {
                // Set the properties of the ping game object
                pingObject.transform.localScale = new Vector3(4,4,4);
                pingObject.layer = LayerMask.NameToLayer("Minimap");
                pingObject.AddComponent<Blink>();
                pingObject.GetComponent<Collider>().enabled = false;
                pingObject.transform.position = ping.GetPositionVector();
            }
            
            return pingObject;
        }
    }
}