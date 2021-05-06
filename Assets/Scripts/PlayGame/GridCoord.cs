using System.Text.RegularExpressions;
using Statics;
using UnityEngine;

namespace PlayGame {
    using System;

    public readonly struct GridCoord : IEquatable<GridCoord> {

        public static GridCoord NullCoord = new GridCoord(-1, -1);
        
        private readonly int _x;
        private readonly int _z;

        public GridCoord(char x, int z) {
            _x = x - 97; // -97 to convert A to 0, B to 1, etc.
            _z = z;
        }

        public GridCoord(int x, int z) {
            _x = x;
            _z = z;
        }

        // Get the grid coordinate of a given world position
        public static GridCoord GetCoordFromVector(Vector3 position) {
            int x = (int) Math.Floor(position.x / GameConstants.GridCellSize);
            int z = (int) Math.Floor(position.z / GameConstants.GridCellSize);

            return new GridCoord(x, z);
        }

        // Use regex to get a grid coordinate from a string
        public static GridCoord GetCoordFromString(string coord) {
            Match number = Regex.Match(coord, @"(\d+)"); // One or more numbers
            char x = coord[0];
            int z = int.Parse(number.Value);

            return new GridCoord(x, z);
        }

        public int GetX() {
            return _x;
        }

        public int GetZ() {
            return _z;
        }

        private int getWorldX() {
            return (_x * GameConstants.GridCellSize) + (GameConstants.GridCellSize / 2);
        }

        private int getWorldZ() {
            return (_z * GameConstants.GridCellSize) + (GameConstants.GridCellSize / 2);
        }

        // Returns the grid coordinate with a y value of 0
        public Vector3 GetWorldVector() {
            return new Vector3(getWorldX(), 0, getWorldZ());
        }

        public bool Equals(GridCoord other)
        {
            return _x == other._x && _z == other._z;
        }

        private string GetXInChar()
        {
            return ((char) (_x + 97)).ToString().ToUpper();
        }

        public override string ToString()
        {
            return $"({GetXInChar()},{_z})";
        }
    }
}
