﻿using Photon.GameControllers;
using PlayGame.Player;
using Statics;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI {
    public class DisplayGridCoordinates : MonoBehaviour {
        
        public Transform target;
        private Text _text;
        
        private void Start() {
            target = !DebugSettings.Debug ? PhotonPlayer.Instance.myAvatar.transform : TestPlayer.GetPlayerShip().transform;
            _text = GetComponent<Text>();
        }

        private void Update() {
            if (target == null) return;
            
            // Get the coordinates of the target
            Vector3 coordinates = target.position;
            Vector2 gridCoords = GridManager.GlobalToGridCoord(coordinates);

            // Display the coordinates of the target rounded to 2 d.p.
            _text.text = $"({gridCoords.x}, {gridCoords.y})";
        }
    }
}