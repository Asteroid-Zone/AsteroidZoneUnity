using UnityEngine;

namespace Assets.Scripts.PlayGame {
    public readonly struct GridCoord {

        private const int gridSize = 10;

        private int x { get; }
        private int y { get; }

        public GridCoord(char x, int y) {
            this.x = x - 97;
            this.y = y;
        }

        public int getWorldX() {
            return x * gridSize;
        }

        public int getWorldY() {
            return y * gridSize;
        }

        // Returns the grid coordinate with a y value of 0
        // TODO change to Vector2?
        public Vector3 getVector() {
            return new Vector3(getWorldX(), 0, getWorldY());
        }

    }
}
