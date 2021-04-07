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
                case "EnginesMaxHealthInput":
                    if (initial) _inputField.text = GameConstants.EnginesMaxHealth.ToString();
                    else GameConstants.EnginesMaxHealth = int.Parse(value);
                    break;
                case "HyperdriveMaxHealthInput":
                    if (initial) _inputField.text = GameConstants.HyperdriveMaxHealth.ToString();
                    else GameConstants.HyperdriveMaxHealth = int.Parse(value);
                    break;
                case "HullMaxHealthInput":
                    if (initial) _inputField.text = GameConstants.StationHullMaxHealth.ToString();
                    else GameConstants.StationHullMaxHealth = int.Parse(value);
                    break;
                case "SolarPanelsMaxHealthInput":
                    if (initial) _inputField.text = GameConstants.SolarPanelsMaxHealth.ToString();
                    else GameConstants.SolarPanelsMaxHealth = int.Parse(value);
                    break;
                case "ShieldGeneratorMaxHealthInput":
                    if (initial) _inputField.text = GameConstants.ShieldGeneratorMaxHealth.ToString();
                    else GameConstants.ShieldGeneratorMaxHealth = int.Parse(value);
                    break;
                case "MaxShieldsInput":
                    if (initial) _inputField.text = GameConstants.StationMaxShields.ToString();
                    else GameConstants.StationMaxShields = int.Parse(value);
                    break;
                case "ShieldRechargeRateInput":
                    if (initial) _inputField.text = GameConstants.StationShieldsRechargeRate.ToString();
                    else GameConstants.StationShieldsRechargeRate = int.Parse(value);
                    break;
                case "AsteroidMinResourcesInput":
                    if (initial) _inputField.text = GameConstants.AsteroidMinResources.ToString();
                    else GameConstants.AsteroidMinResources = int.Parse(value);
                    break;
                case "AsteroidMaxResourcesInput":
                    if (initial) _inputField.text = GameConstants.AsteroidMaxResources.ToString();
                    else GameConstants.AsteroidMaxResources = int.Parse(value);
                    break;
                case "AsteroidProbabilityInput":
                    if (initial) _inputField.text = GameConstants.AsteroidProbability.ToString();
                    else GameConstants.AsteroidProbability = float.Parse(value);
                    break;
                case "AsteroidEveryXSecondsInput":
                    if (initial) _inputField.text = GameConstants.AsteroidEveryXSeconds.ToString();
                    else GameConstants.AsteroidEveryXSeconds = float.Parse(value);
                    break;
                case "MaxAsteroidsMultiplierInput":
                    if (initial) _inputField.text = GameConstants.MaxAsteroidsMultiplier.ToString();
                    else GameConstants.MaxAsteroidsMultiplier = float.Parse(value);
                    break;
                case "GridCellSizeInput":
                    if (initial) _inputField.text = GameConstants.GridCellSize.ToString();
                    else GameConstants.GridCellSize = int.Parse(value);
                    break;
                case "GridWidthInput":
                    if (initial) _inputField.text = GameConstants.GridWidth.ToString();
                    else GameConstants.GridWidth = int.Parse(value);
                    break;
                case "GridHeightInput":
                    if (initial) _inputField.text = GameConstants.GridHeight.ToString();
                    else GameConstants.GridHeight = int.Parse(value);
                    break;
                case "PirateLaserMiningRateInput":
                    if (initial) _inputField.text = GameConstants.PirateLaserMiningRate.ToString();
                    else GameConstants.PirateLaserMiningRate = int.Parse(value);
                    break;
                case "MaxPiratesMultiplierInput":
                    if (initial) _inputField.text = GameConstants.MaxPiratesMultiplier.ToString();
                    else GameConstants.MaxPiratesMultiplier = float.Parse(value);
                    break;
                case "PirateProbabilityInput":
                    if (initial) _inputField.text = GameConstants.PirateProbability.ToString();
                    else GameConstants.PirateProbability = float.Parse(value);
                    break;
                case "PirateEveryXSecondsInput":
                    if (initial) _inputField.text = GameConstants.PirateEveryXSeconds.ToString();
                    else GameConstants.PirateEveryXSeconds = float.Parse(value);
                    break;
                case "PirateMinReinforcementsInput":
                    if (initial) _inputField.text = GameConstants.PirateMinReinforcements.ToString();
                    else GameConstants.PirateMinReinforcements = int.Parse(value);
                    break;
                case "PirateMaxReinforcementsInput":
                    if (initial) _inputField.text = GameConstants.PirateMaxReinforcements.ToString();
                    else GameConstants.PirateMaxReinforcements = int.Parse(value);
                    break;
            }
        }

    }
}