﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayGame.UI;
using UnityEngine;

namespace PlayGame.Pings {

    /// <summary>
    /// This class manages all the active pings.
    /// </summary>
    public class PingManager : MonoBehaviour {
        
        private const int PingTimeInSeconds = 10;

        private Dictionary<Ping, GameObject> _pings;

        private void Start() {
            _pings = new Dictionary<Ping, GameObject>(); // Initialise a dictionary of pings
        }

        /// <summary>
        /// Creates a new ping with a game object.
        /// </summary>
        /// <param name="ping"></param>
        public void AddPing(Ping ping) {
            // Remove all old pings with the same location (should be at most one old ping, but just to make sure it is implemented for many)
            var sameLocPings = _pings.ToList().Where(kvp => kvp.Key.GetGridCoord().Equals(ping.GetGridCoord())).ToList();
            foreach (var sameLocPing in sameLocPings) {
                RemovePing(sameLocPing.Key);
            }

            // Create a new object for the new ping at the specific position on the map
            GameObject pingObject = CreateObjectForPing(ping);
            _pings[ping] = pingObject;
            
            EventsManager.AddMessage($"Added ping at {ping.GetGridCoord().ToString()} of type {ping.GetPingType().ToString()}");

            // Remove the new ping after a specific amount of time
            StartCoroutine(RemovePingAfterTime(ping));
        }

        /// <summary>
        /// Returns a dictionary of all active pings.
        /// </summary>
        /// <returns></returns>
        public Dictionary<Ping, GameObject> GetPings() {
            return _pings;
        }

        /// <summary>
        /// Removes a ping after a certain amount of time.
        /// </summary>
        /// <param name="ping"></param>
        /// <returns></returns>
        private IEnumerator RemovePingAfterTime(Ping ping) {
            yield return new WaitForSeconds(PingTimeInSeconds);
            RemovePing(ping);
        }

        /// <summary>
        /// Destroys the pings GameObject and removes it from the dictionary.
        /// </summary>
        /// <param name="ping"></param>
        private void RemovePing(Ping ping) {
            if (_pings.ContainsKey(ping)) {
                // Remove ping game object
                DestroyImmediate(_pings[ping]);
            }
            
            _pings.Remove(ping);
        }

        /// <summary>
        /// Creates a GameObject for a ping.
        /// </summary>
        /// <param name="ping"></param>
        /// <returns></returns>
        private GameObject CreateObjectForPing(Ping ping) {
            GameObject pingObject;
            
            // Create a game object according to the ping type
            switch (ping.GetPingType()) {
                case PingType.Asteroid:
                    pingObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    break;
                case PingType.Pirate:
                    pingObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    break;
                case PingType.Generic:
                    pingObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    break;
                default:
                    pingObject = null;
                    break;
            }

            if (pingObject != null) {
                // Set the properties of the ping game object
                pingObject.transform.localScale = new Vector3(4,4,4);
                pingObject.layer = LayerMask.NameToLayer("Minimap");
                pingObject.AddComponent<Blink>();
                pingObject.GetComponent<Collider>().enabled = false;
                pingObject.transform.position = ping.GetPositionVector() + new Vector3(0, 5, 0);
            }
            
            return pingObject;
        }
    }
}