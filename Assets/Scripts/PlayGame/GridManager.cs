using System;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame
{
    public class GridManager : MonoBehaviour
    {
        public GameObject gridSquarePrefab;
        public const int Width = 11;
        public const int Height = 11;
        public int totalCells;
        private GameObject[,] _grid;

        private const int CellSize = 10;

        private void Start() {
            // X/Y of the first grid square (top left)
            var startX = CellSize / 2;
            var startY = CellSize / 2;
            _grid = new GameObject[Height, Width];
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    // Instantiate a grid square prefab with `this` as parent
                    // Also sets the canvas text to the sector coords
                    // TODO: See if text setting can be done better
                    var position = new Vector3(startX + (x * CellSize), 0, startY + (y * CellSize));
                    var newGridSquare = Instantiate(gridSquarePrefab, position, Quaternion.identity);
                    newGridSquare.transform.parent = gameObject.transform;
                    newGridSquare.transform.GetChild(0).GetChild(0).GetComponent<Text>().text =
                        $"({NumberToLetterCoord(x)}, {y})";
                    _grid[y, x] = newGridSquare;
                }
            }
        }

        private static char NumberToLetterCoord(int numCoord) {
            return (char) (numCoord + 65);
        }

        public int GetCellSize() {
            return CellSize;
        }

        // Returns the nearest point on the edge of the grid to the given position
        public static Vector3 GetNearestEdgePoint(Vector3 position) {
            GridCoord coord = GridCoord.GetCoordFromVector(position);

            int topDistance = Math.Abs(coord.GetZ() - Height);
            int bottomDistance = Math.Abs(coord.GetZ() - 0);
            int rightDistance = Math.Abs(coord.GetX() - Width);
            int leftDistance = Math.Abs(coord.GetX() - 0);

            int smallestDistance = Math.Min(topDistance, Math.Min(bottomDistance, Math.Min(leftDistance, rightDistance)));
            if (smallestDistance == topDistance) return GridToGlobalCoord(new Vector2(coord.GetX(), Height));
            if (smallestDistance == bottomDistance) return GridToGlobalCoord(new Vector2(coord.GetX(), 0));
            if (smallestDistance == rightDistance) return GridToGlobalCoord(new Vector2(Width, coord.GetZ()));
            if (smallestDistance == leftDistance) return GridToGlobalCoord(new Vector2(0, coord.GetZ()));

            return Vector3.positiveInfinity;
        }

        // Helper function that, given coordinates on the grid, returns the centre of it in normal 3D space
        // e.g: (0,0) is (5, 0, 5)
        public static Vector3 GridToGlobalCoord(Vector2 gridCoord) {
            Vector3 globalCoord;
            globalCoord.y = 0;
            globalCoord.x = (CellSize / 2f) + (gridCoord.x * CellSize);
            globalCoord.z = (CellSize / 2f) + (gridCoord.y * CellSize);
            return globalCoord;
        }

        public Vector2 GlobalToGridCoord(Vector3 globalCoord)
        {
            Vector2 gridCoord;
            gridCoord.x = (int) globalCoord.x / CellSize % CellSize;
            gridCoord.y = (int) globalCoord.z / CellSize % CellSize;
            return gridCoord;
        }

        public Vector3 GetGridCentre() {
            float x = (Width / 2f) * CellSize;
            float z = (Height / 2f) * CellSize;
            return new Vector3(x, 0, z);
        }

        // Gets the total number of cells in the grid
        public int GetTotalCells() {
            return Height * Width;
        }

        public GameObject [,] GetGrid()
        {
            return _grid;
        }
    }
}
