using UnityEngine;

namespace Assets.Scripts.PlayGame {
    public struct GridCoord {

        private const int gridSize = 10;

        private char x { get; }
        private int y { get; }

        public GridCoord(char x, int y) {
            this.x = x;
            this.y = y;
        }

        public int getWorldX() {
            return (x - 97) * gridSize;
        }

        public int getWorldY() {
            return y * gridSize;
        }

        // Returns the grid coordinate with a z value of 0
        // TODO change to Vector2?
        public Vector3 getVector() {
            return new Vector3(getWorldX(), getWorldY());
        }

    }
}
