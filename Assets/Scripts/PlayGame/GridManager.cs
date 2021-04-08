﻿using System;
using Statics;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame
{
    public class GridManager : MonoBehaviour
    {
        public GameObject gridSquarePrefab;
        public int totalCells;

        private void Start() {
            // X/Y of the first grid square (top left)
            var startX = GameConstants.GridCellSize / 2;
            var startY = GameConstants.GridCellSize / 2;
            for (int y = 0; y < GameConstants.GridHeight; y++)
            {
                for (int x = 0; x < GameConstants.GridWidth; x++)
                {
                    // Instantiate a grid square prefab with `this` as parent
                    // Also sets the canvas text to the sector coords
                    // TODO: See if text setting can be done better
                    var position = new Vector3(startX + (x * GameConstants.GridCellSize), 0, startY + (y * GameConstants.GridCellSize));
                    var newGridSquare = Instantiate(gridSquarePrefab, position, Quaternion.identity);
                    newGridSquare.transform.parent = gameObject.transform;
                    newGridSquare.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = $"({NumberToLetterCoord(x)}, {y})";
                }
            }
        }

        private static char NumberToLetterCoord(int numCoord) {
            return (char) (numCoord + 65);
        }

        public int GetCellSize() {
            return GameConstants.GridCellSize;
        }

        // Returns the nearest point on the edge of the grid to the given position
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

        // Helper function that, given coordinates on the grid, returns the centre of it in normal 3D space
        // e.g: (0,0) is (5, 0, 5)
        public static Vector3 GridToGlobalCoord(Vector2 gridCoord) {
            Vector3 globalCoord;
            globalCoord.y = 0;
            globalCoord.x = (GameConstants.GridCellSize / 2f) + (gridCoord.x * GameConstants.GridCellSize);
            globalCoord.z = (GameConstants.GridCellSize / 2f) + (gridCoord.y * GameConstants.GridCellSize);
            return globalCoord;
        }

        public static Vector3 GetGridCentre() {
            float x = (GameConstants.GridWidth / 2f) * GameConstants.GridCellSize;
            float z = (GameConstants.GridHeight / 2f) * GameConstants.GridCellSize;
            return new Vector3(x, 0, z);
        }

        // Gets the total number of cells in the grid
        public int GetTotalCells() {
            return GameConstants.GridHeight * GameConstants.GridWidth;
        }
    }
}
