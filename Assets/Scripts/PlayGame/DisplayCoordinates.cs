using UnityEngine;
using UnityEngine.UI;

public class DisplayCoordinates : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        // Get the coordinates of the target
        Vector3 coordinates = target.position;

        // Display the coordinates of the target rounded to 2 d.p.
        GetComponent<Text>().text = $"({coordinates.x:0.##}, {coordinates.y:0.##}, {coordinates.z:0.##})";
    }
}
