using UnityEngine;

namespace Assets.Scripts.PlayGame {
    public readonly struct GridCoord {

        private const int gridSize = 10;

        private int x { get; }
        private int z { get; }

        public GridCoord(char x, int z) {
            this.x = x - 97; // -97 to convert A to 0, B to 1, etc.
            this.z = z;
        }

        public int getWorldX() {
            return (x * gridSize) + (gridSize / 2);
        }

        public int getWorldZ() {
            return (z * gridSize) + (gridSize / 2);
        }

        // Returns the grid coordinate with a y value of 0
        // TODO change to Vector2?
        public Vector3 GetWorldVector() {
            return new Vector3(getWorldX(), 0, getWorldZ());
        }

    }
}
