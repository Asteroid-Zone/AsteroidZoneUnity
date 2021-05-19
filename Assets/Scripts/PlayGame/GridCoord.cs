using System.Text.RegularExpressions;
using Statics;
using UnityEngine;

namespace PlayGame {
    using System;

    /// <summary>
    /// Struct that represents a square on the grid.
    /// </summary>
    public readonly struct GridCoord : IEquatable<GridCoord> {

        public static GridCoord NullCoord = new GridCoord(-1, -1);
        
        private readonly int _x;
        private readonly int _z;

        /// <summary>
        /// Constructor that takes a char and an int.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public GridCoord(char x, int z) {
            _x = x - 97; // -97 to convert A to 0, B to 1, etc.
            _z = z;
        }

        /// <summary>
        /// Constructor that takes 2 ints.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public GridCoord(int x, int z) {
            _x = x;
            _z = z;
        }
        
        /// <summary>
        /// Returns the GridCoord of a given world position.
        /// </summary>
        /// <param name="position">Vector3 position in world space.</param>
        public static GridCoord GetCoordFromVector(Vector3 position) {
            int x = (int) Math.Floor(position.x / GameConstants.GridCellSize);
            int z = (int) Math.Floor(position.z / GameConstants.GridCellSize);

            return new GridCoord(x, z);
        }

        /// <summary>
        /// Uses regex to extract a GridCoord from a string.
        /// </summary>
        /// <param name="coord">Must be a valid coordinate in the format 'a1'.</param>
        public static GridCoord GetCoordFromString(string coord) {
            Match number = Regex.Match(coord, @"(\d+)"); // One or more numbers
            char x = coord[0];
            int z = int.Parse(number.Value);

            return new GridCoord(x, z);
        }

        /// <summary>
        /// Returns the x coordinate.
        /// </summary>
        public int GetX() {
            return _x;
        }

        /// <summary>
        /// Returns the z coordinate.
        /// </summary>
        public int GetZ() {
            return _z;
        }

        /// <summary>
        /// Returns the x coordinate in world space.
        /// </summary>
        private int getWorldX() {
            return (_x * GameConstants.GridCellSize) + (GameConstants.GridCellSize / 2);
        }

        /// <summary>
        /// Returns the z coordinate in world space.
        /// </summary>
        private int getWorldZ() {
            return (_z * GameConstants.GridCellSize) + (GameConstants.GridCellSize / 2);
        }

        /// <summary>
        /// Returns the world space Vector3 with a y value of 0.
        /// </summary>
        public Vector3 GetWorldVector() {
            return new Vector3(getWorldX(), 0, getWorldZ());
        }

        /// <summary>
        /// Returns true if this is equal to the GridCoord provided.
        /// </summary>
        /// <param name="other"></param>
        public bool Equals(GridCoord other) {
            return _x == other._x && _z == other._z;
        }

        /// <summary>
        /// Returns the x coordinate as an uppercase letter.
        /// </summary>
        private string GetXInChar() {
            return ((char) (_x + 97)).ToString().ToUpper();
        }

        /// <summary>
        /// Returns a string in the format (x, z).
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return $"({GetXInChar()},{_z})";
        }
    }
}
