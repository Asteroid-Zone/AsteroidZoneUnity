using Assets.Scripts.PlayGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour {

    private Vector3 direction;
    private float speed = 0;

    private void Start() {
        this.direction = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update() {
        transform.Translate(direction * (Time.deltaTime) * speed, Space.World);
        transform.localRotation = Quaternion.LookRotation(direction);
    }

    public void setDirection(Vector3 direction) {
        this.direction = direction;
    }

    public void setSpeed(float speed) {
        this.speed = speed;
    }

    public void setPosition(GridCoord coord) {
        transform.position = coord.getVector();

    }
}
