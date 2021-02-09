using UnityEngine;
using UnityEngine.UI;

public class DisplayCoordinates : MonoBehaviour
{
    public Transform target;
    private Text _text;

    private void Start()
    {
        _text = GetComponent<Text>();
    }

    private void Update()
    {
        // Get the coordinates of the target
        var coordinates = target.position;

        // Display the coordinates of the target rounded to 2 d.p.
        _text.text = $"({coordinates.x:0.##}, {coordinates.y:0.##}, {coordinates.z:0.##})";
    }
}
