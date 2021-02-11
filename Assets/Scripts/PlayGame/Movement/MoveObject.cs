using Assets.Scripts.PlayGame;
using UnityEngine;

public class MoveObject : MonoBehaviour {

    // TODO change to Vector2?
    private Vector3 _direction;
    private float _speed;

    private Vector3 _destination = Vector3.positiveInfinity;

    private void Start() {
        _direction = transform.rotation.eulerAngles;
        UpdateRotation();
    }

    private void Update() {
        if (!HasReachedDestination()) {
            transform.Translate((Time.deltaTime * _speed) * _direction, Space.World);
        }
    }

    private bool HasReachedDestination()
    {
        return Vector3.Distance(transform.position, _destination) < 0.2;
    }

    private void UpdateRotation() {
        transform.localRotation = Quaternion.LookRotation(_direction);
    }
    
    public void SetDirection(Vector3 newDirection) {
        _direction = newDirection;
        _destination = Vector3.positiveInfinity;
        UpdateRotation();
    }

    public void SetDirection(GridCoord position) {
        _direction = Vector3.Normalize(position.getVector() - transform.position);
        _destination = position.getVector();
        UpdateRotation();
    }

    public void SetSpeed(float newSpeed) {
        _speed = newSpeed;
    }
}
