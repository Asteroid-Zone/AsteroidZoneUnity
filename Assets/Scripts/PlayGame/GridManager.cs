﻿using UnityEngine;
using UnityEngine.UI;

namespace PlayGame
{
    public class GridManager : MonoBehaviour
    {
        public GameObject gridSquarePrefab;
        public int width;
        public int height;

        private const int CellSize = 10;

        private void Start()
        {
            // X/Y of the first grid square (top left)
            var startX = CellSize / 2;
            var startY = CellSize / 2;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Instantiate a grid square prefab with `this` as parent
                    // Also sets the canvas text to the sector coords
                    // TODO: See if text setting can be done better
                    var position = new Vector3(startX + (x * CellSize), 0, startY + (y * CellSize));
                    var newGridSquare = Instantiate(gridSquarePrefab, position, Quaternion.identity);
                    newGridSquare.transform.parent = gameObject.transform;
                    newGridSquare.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = $"({x}, {y})";
                }
            }
        }

        public int GetCellSize()
        {
            return CellSize;
        }
        
        // Helper function that, given coordinates on the grid, returns the centre of it in normal 3D space
        // e.g: (0,0) is (5, 0, 5)
        public Vector3 GridToGlobalCoord(Vector2 gridCoord)
        {
            Vector3 globalCoord;
            globalCoord.y = 0;
            globalCoord.x = (CellSize / 2f) + (gridCoord.x * CellSize);
            globalCoord.z = (CellSize / 2f) + (gridCoord.y * CellSize);
            return globalCoord;
        }

        public Vector3 GetGridCentre()
        {
            float x = (width / 2f) * CellSize;
            float z = (height / 2f) * CellSize;
            return new Vector3(x, 0, z);
        }
    }
}
