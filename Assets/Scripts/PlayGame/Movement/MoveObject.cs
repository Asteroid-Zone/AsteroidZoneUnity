using Assets.Scripts.PlayGame;
using UnityEngine;

public class MoveObject : MonoBehaviour {

    // TODO change to Vector2?
    private Vector3 direction;
    private float speed = 0;

    private Vector3 destination = Vector3.positiveInfinity;

    private void Start() {
        direction = transform.rotation.eulerAngles;
        updateRotation();
    }

    private void Update() {
        if (!hasReachedDestination()) {
            transform.Translate(direction * (Time.deltaTime) * speed, Space.World);
        }
    }

    private bool hasReachedDestination() {
        if (Vector3.Distance(transform.position, destination) < 0.2) {
            return true;
        }

        return false;
    }

    private void updateRotation() {
        transform.localRotation = Quaternion.LookRotation(direction);
    }
    
    public void setDirection(Vector3 direction) {
        this.direction = direction;
        destination = Vector3.positiveInfinity;
        updateRotation();
    }

    public void setDirection(GridCoord position) {
        this.direction = Vector3.Normalize(position.getVector() - transform.position);
        destination = position.getVector();
        updateRotation();
    }

    public void setSpeed(float speed) {
        this.speed = speed;
    }

}
