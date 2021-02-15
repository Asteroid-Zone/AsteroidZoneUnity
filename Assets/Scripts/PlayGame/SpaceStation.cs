using PlayGame;
using UnityEngine;

public class SpaceStation : MonoBehaviour
{

    public GridManager gridManager;
    
    void Start() {
        transform.position = gridManager.GetGridCentre();
    }

}
