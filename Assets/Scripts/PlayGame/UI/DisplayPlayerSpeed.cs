using UnityEngine;
using UnityEngine.UI;

public class DisplayPlayerSpeed : MonoBehaviour {
    
    public GameObject player;

    private PlayerData _playerData;
    private Text _text;

    private void Start() {
        _text = GetComponent<Text>();
        _playerData = player.GetComponent<PlayerData>();
    }

    private void Update() {
        _text.text = "Speed: " + _playerData.GetSpeed() + "/" + _playerData.GetMaxSpeed();
    }
}
