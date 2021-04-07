using Statics;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI {
    public class InputFieldListener : MonoBehaviour {

        private InputField _inputField;
        
        private void Start() {
            _inputField = gameObject.GetComponent<InputField>();
            _inputField.onEndEdit.AddListener(EditValue);
            EditValue("initial");
        }
        
        // todo there must be a better way to do this
        private void EditValue(string value) {
            bool initial = (value == "initial");
            switch (_inputField.name) {
                case "PlayerMaxHealthInput":
                    if (initial) _inputField.text = GameConstants.PlayerMaxHealth.ToString();
                    else GameConstants.PlayerMaxHealth = int.Parse(value);
                    break;
                case "PlayerMaxSpeedInput":
                    if (initial) _inputField.text = GameConstants.PlayerMaxSpeed.ToString();
                    else GameConstants.PlayerMaxSpeed = float.Parse(value);
                    break;
                case "PlayerRotateSpeedInput":
                    if (initial) _inputField.text = GameConstants.PlayerRotateSpeed.ToString();
                    else GameConstants.PlayerRotateSpeed = float.Parse(value);
                    break;
            }
        }

    }
}