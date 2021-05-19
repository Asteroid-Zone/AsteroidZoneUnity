using System;
using Statics;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame {
    
    /// <summary>
    /// This class controls the game grid.
    /// </summary>
    public class GridManager : MonoBehaviour {
        
        public GameObject gridSquarePrefab;
        private GameObject[,] _grid;

        /// <summary>
        /// This method creates the grid at the start of the game.
        /// </summary>
        private void Start() {
            // X/Y of the first grid square (top left)
            var startX = GameConstants.GridCellSize / 2;
            var startY = GameConstants.GridCellSize / 2;
            
            _grid = new GameObject[GameConstants.GridHeight, GameConstants.GridWidth];
            
            for (int y = 0; y < GameConstants.GridHeight; y++) {
                for (int x = 0; x < GameConstants.GridWidth; x++) {
                    // Instantiate a grid square prefab with `this` as parent
                    // Also sets the canvas text to the sector coords
                    var position = new Vector3(startX + (x * GameConstants.GridCellSize), 0, startY + (y * GameConstants.GridCellSize));
                    var newGridSquare = Instantiate(gridSquarePrefab, position, Quaternion.identity);
                    newGridSquare.transform.parent = gameObject.transform;
                    newGridSquare.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = $"({NumberToLetterCoord(x)}, {y})";
                    _grid[y, x] = newGridSquare;
                }
            }
        }

        /// <summary>
        /// Converts a number to a letter. (a=0, ...).
        /// </summary>
        /// <param name="numCoord"></param>
        private static char NumberToLetterCoord(int numCoord) {
            return (char) (numCoord + 65);
        }

        /// <summary>
        /// Returns the size of a grid cell.
        /// </summary>
        public static int GetCellSize() {
            return GameConstants.GridCellSize;
        }

        /// <summary>
        /// Returns the nearest point on the edge of the grid to the given position.
        /// </summary>
        /// <param name="position"></param>
        public static Vector3 GetNearestEdgePoint(Vector3 position) {
            GridCoord coord = GridCoord.GetCoordFromVector(position);

            int topDistance = Math.Abs(coord.GetZ() - GameConstants.GridHeight);
            int bottomDistance = Math.Abs(coord.GetZ() - 0);
            int rightDistance = Math.Abs(coord.GetX() - GameConstants.GridWidth);
            int leftDistance = Math.Abs(coord.GetX() - 0);

            int smallestDistance = Math.Min(topDistance, Math.Min(bottomDistance, Math.Min(leftDistance, rightDistance)));
            if (smallestDistance == topDistance) return GridToGlobalCoord(new Vector2(coord.GetX(), GameConstants.GridHeight));
            if (smallestDistance == bottomDistance) return GridToGlobalCoord(new Vector2(coord.GetX(), 0));
            if (smallestDistance == rightDistance) return GridToGlobalCoord(new Vector2(GameConstants.GridWidth, coord.GetZ()));
            if (smallestDistance == leftDistance) return GridToGlobalCoord(new Vector2(0, coord.GetZ()));

            return Vector3.positiveInfinity;
        }

        /// <summary>
        /// Returns the world space Vector3 of the centre of the given grid coordinate.
        /// </summary>
        /// <param name="gridCoord"></param>
        // todo convert to GridCoord
        public static Vector3 GridToGlobalCoord(Vector2 gridCoord) {
            Vector3 globalCoord;
            globalCoord.y = 0;
            globalCoord.x = (GameConstants.GridCellSize / 2f) + (gridCoord.x * GameConstants.GridCellSize);
            globalCoord.z = (GameConstants.GridCellSize / 2f) + (gridCoord.y * GameConstants.GridCellSize);
            return globalCoord;
        }

        /// <summary>
        /// Converts a world space Vector3 to a grid coordinate Vector2.
        /// </summary>
        /// <param name="globalCoord"></param>
        // todo convert to GridCoord
        public static Vector2 GlobalToGridCoord(Vector3 globalCoord) {
            Vector2 gridCoord;
            gridCoord.x = (int) (globalCoord.x / GameConstants.GridCellSize);
            gridCoord.y = (int) (globalCoord.z / GameConstants.GridCellSize);
            return gridCoord;
        }

        /// <summary>
        /// Returns a world space Vector3 representing the centre of the grid.
        /// </summary>
        public static Vector3 GetGridCentre() {
            float x = (GameConstants.GridWidth / 2f) * GameConstants.GridCellSize;
            float z = (GameConstants.GridHeight / 2f) * GameConstants.GridCellSize;
            return new Vector3(x, 0, z);
        }

        /// <summary>
        /// Returns the total number of cells in the grid.
        /// </summary>
        public static int GetTotalCells() {
            return GameConstants.GridHeight * GameConstants.GridWidth;
        }

        /// <summary>
        /// Returns the grid array of GameObjects.
        /// </summary>
        /// <returns></returns>
        public GameObject [,] GetGrid() {
            return _grid;
        }

        /// <summary>
        /// Returns the width of the grid.
        /// </summary>
        public static int GetWidth() {
            return GameConstants.GridWidth;
        }

        /// <summary>
        /// Returns the height of the grid.
        /// </summary>
        public static int GetHeight() {
            return GameConstants.GridHeight;
        }
    }
}
