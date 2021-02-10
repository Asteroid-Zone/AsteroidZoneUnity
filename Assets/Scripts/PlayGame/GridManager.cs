using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public GameObject gridSquarePrefab;
    public int width;
    public int height;

    private const int CellSize = 10;

    private void Start()
    {
        // X/Y of the first grid square (top left)
        var startX = -(width / 2) * CellSize + CellSize / 2;
        var startY = (height / 2) * CellSize - CellSize / 2;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Instantiate a grid square prefab with `this` as parent
                // Also sets the canvas text to the sector coords
                // TODO: See if text setting can be done better
                var position = new Vector3(startX + (x * CellSize), 0, startY - (y * CellSize));
                var newGridSquare = Instantiate(gridSquarePrefab, position, Quaternion.identity);
                newGridSquare.transform.parent = gameObject.transform;
                newGridSquare.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = $"({x}, {y})";
            }
        }
    }
}
