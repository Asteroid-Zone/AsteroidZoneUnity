using UnityEngine;

namespace PlayGame {
    using System;

    public readonly struct GridCoord : IEquatable<GridCoord> {

        private const int GridSize = 10;

        private readonly int _x;
        private readonly int _z;

        public GridCoord(char x, int z) {
            _x = x - 97; // -97 to convert A to 0, B to 1, etc.
            _z = z;
        }

        private int getWorldX() {
            return (_x * GridSize) + (GridSize / 2);
        }

        private int getWorldZ() {
            return (_z * GridSize) + (GridSize / 2);
        }

        // Returns the grid coordinate with a y value of 0
        // TODO change to Vector2?
        public Vector3 GetWorldVector() {
            return new Vector3(getWorldX(), 0, getWorldZ());
        }

        public bool Equals(GridCoord other)
        {
            return _x == other._x && _z == other._z;
        }
    }
}
