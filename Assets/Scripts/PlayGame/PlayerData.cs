using UnityEngine;

public enum Role {
    StationCommander,
    MilitaryTactician,
    MiningCaptain,
    Researcher
}

public class PlayerData : MonoBehaviour {

    private Role _role;
    private int _health;
    private int _shields;
    private float _speed;

    private void Start() {
        _role = Role.StationCommander; // TODO assign roles in the menu
        _health = 100; // TODO different stats for different roles
        _shields = 100;
        _speed = 2.5f;
    }

    public float GetSpeed() {
        return _speed;
    }

}
