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
    private float _speed;

    private void Start() {
        _role = Role.StationCommander; // TODO assign roles in the menu
        _health = 100; // TODO different stats for different roles
        _speed = 2.5f;
    }

    public float GetSpeed() {
        return _speed;
    }

    public Role GetRole() {
        return _role;
    }

    public int GetHealth() {
        return _health;
    }
}
