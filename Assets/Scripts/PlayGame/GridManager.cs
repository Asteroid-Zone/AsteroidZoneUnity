using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject gridSquarePrefab;
    public int width;
    public int height;

    private const int CellSize = 10;

    private void Start()
    {
        var xBound = (width / 2) * CellSize + CellSize / 2;
        var yBound = (height / 2) * CellSize + CellSize / 2;
        for (int y = -yBound + CellSize; y < yBound; y += CellSize)
        {
            for (int x = -xBound + CellSize; x < xBound; x += CellSize)
            {
                var newGridSquare = Instantiate(gridSquarePrefab, new Vector3(x, 0, y), Quaternion.identity);
                newGridSquare.transform.parent = gameObject.transform;
            }
        }
    }
}
