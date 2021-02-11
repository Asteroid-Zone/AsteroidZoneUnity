﻿using UnityEngine;

namespace Assets.Scripts.PlayGame {
    public readonly struct GridCoord {

        private const int gridSize = 10;

        private int x { get; }
        private int y { get; }

        public GridCoord(char x, int y) {
            this.x = x - 97; // -97 to convert A to 0, B to 1, etc.
            this.y = y;
        }

        public int getWorldX() {
            return (x * gridSize) + (gridSize / 2);
        }

        public int getWorldY() {
            return (y * gridSize) + (gridSize / 2);
        }

        // Returns the grid coordinate with a y value of 0
        // TODO change to Vector2?
        public Vector3 GetWorldVector() {
            return new Vector3(getWorldX(), 0, getWorldY());
        }

    }
}
