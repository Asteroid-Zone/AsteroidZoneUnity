﻿using Statics;
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
                case "PlayerMiningRateInput":
                    if (initial) _inputField.text = GameConstants.PlayerMiningRate.ToString();
                    else GameConstants.PlayerMiningRate = int.Parse(value);
                    break;
                case "PlayerMiningRangeInput":
                    if (initial) _inputField.text = GameConstants.PlayerMiningRange.ToString();
                    else GameConstants.PlayerMiningRange = int.Parse(value);
                    break;
                case "PlayerMiningDelayInput":
                    if (initial) _inputField.text = GameConstants.PlayerMiningDelay.ToString();
                    else GameConstants.PlayerMiningDelay = int.Parse(value);
                    break;
                case "PlayerShotDelayInput":
                    if (initial) _inputField.text = GameConstants.PlayerShotDelay.ToString();
                    else GameConstants.PlayerShotDelay = int.Parse(value);
                    break;
                case "PlayerLaserSpeedInput":
                    if (initial) _inputField.text = GameConstants.PlayerLaserSpeed.ToString();
                    else GameConstants.PlayerLaserSpeed = int.Parse(value);
                    break;
                case "PlayerLaserDamageInput":
                    if (initial) _inputField.text = GameConstants.PlayerLaserDamage.ToString();
                    else GameConstants.PlayerLaserDamage = int.Parse(value);
                    break;
                case "PlayerLaserDamageRangeInput":
                    if (initial) _inputField.text = GameConstants.PlayerLaserDamageRange.ToString();
                    else GameConstants.PlayerLaserDamageRange = int.Parse(value);
                    break;
                case "PlayerLaserMiningRateInput":
                    if (initial) _inputField.text = GameConstants.PlayerLaserMiningRate.ToString();
                    else GameConstants.PlayerLaserMiningRate = int.Parse(value);
                    break;
                case "PlayerLaserRangeInput":
                    if (initial) _inputField.text = GameConstants.PlayerLaserRange.ToString();
                    else GameConstants.PlayerLaserRange = int.Parse(value);
                    break;
            }
        }

    }
}