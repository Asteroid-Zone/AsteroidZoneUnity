using System;
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
                case "PlayerLookRadiusInput":
                    if (initial) _inputField.text = GameConstants.PlayerLookRadius.ToString();
                    else GameConstants.PlayerLookRadius = float.Parse(value);
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
                case "PirateScoutMaxHealthInput":
                    if (initial) _inputField.text = GameConstants.PirateScoutMaxHealth.ToString();
                    else GameConstants.PirateScoutMaxHealth = float.Parse(value);
                    break;
                case "PirateScoutSpeedInput":
                    if (initial) _inputField.text = GameConstants.PirateScoutSpeed.ToString();
                    else GameConstants.PirateScoutSpeed = float.Parse(value);
                    break;
                case "PirateScoutLookRadiusInput":
                    if (initial) _inputField.text = GameConstants.PirateScoutLookRadius.ToString();
                    else GameConstants.PirateScoutLookRadius = float.Parse(value);
                    break;
                case "PirateScoutLaserSpeedInput":
                    if (initial) _inputField.text = GameConstants.PirateScoutLaserSpeed.ToString();
                    else GameConstants.PirateScoutLaserSpeed = float.Parse(value);
                    break;
                case "PirateScoutLaserRangeInput":
                    if (initial) _inputField.text = GameConstants.PirateScoutLaserRange.ToString();
                    else GameConstants.PirateScoutLaserRange = int.Parse(value);
                    break;
                case "PirateScoutLaserDamageRangeInput":
                    if (initial) _inputField.text = GameConstants.PirateScoutLaserDamageRange.ToString();
                    else GameConstants.PirateScoutLaserDamageRange = int.Parse(value);
                    break;
                case "PirateScoutLaserDamageInput":
                    if (initial) _inputField.text = GameConstants.PirateScoutLaserDamage.ToString();
                    else GameConstants.PirateScoutLaserDamage = int.Parse(value);
                    break;
                case "PirateScoutShotDelayInput":
                    if (initial) _inputField.text = GameConstants.PirateScoutShotDelay.ToString();
                    else GameConstants.PirateScoutShotDelay = int.Parse(value);
                    break;
                case "PirateEliteMaxHealthInput":
                    if (initial) _inputField.text = GameConstants.PirateEliteMaxHealth.ToString();
                    else GameConstants.PirateEliteMaxHealth = float.Parse(value);
                    break;
                case "PirateEliteSpeedInput":
                    if (initial) _inputField.text = GameConstants.PirateEliteSpeed.ToString();
                    else GameConstants.PirateEliteSpeed = float.Parse(value);
                    break;
                case "PirateEliteLookRadiusInput":
                    if (initial) _inputField.text = GameConstants.PirateEliteLookRadius.ToString();
                    else GameConstants.PirateEliteLookRadius = float.Parse(value);
                    break;
                case "PirateEliteLaserSpeedInput":
                    if (initial) _inputField.text = GameConstants.PirateEliteLaserSpeed.ToString();
                    else GameConstants.PirateEliteLaserSpeed = float.Parse(value);
                    break;
                case "PirateEliteLaserRangeInput":
                    if (initial) _inputField.text = GameConstants.PirateEliteLaserRange.ToString();
                    else GameConstants.PirateEliteLaserRange = int.Parse(value);
                    break;
                case "PirateEliteLaserDamageRangeInput":
                    if (initial) _inputField.text = GameConstants.PirateEliteLaserDamageRange.ToString();
                    else GameConstants.PirateEliteLaserDamageRange = int.Parse(value);
                    break;
                case "PirateEliteLaserDamageInput":
                    if (initial) _inputField.text = GameConstants.PirateEliteLaserDamage.ToString();
                    else GameConstants.PirateEliteLaserDamage = int.Parse(value);
                    break;
                case "PirateEliteShotDelayInput":
                    if (initial) _inputField.text = GameConstants.PirateEliteShotDelay.ToString();
                    else GameConstants.PirateEliteShotDelay = int.Parse(value);
                    break;
                default:
                    throw new ArgumentException("Invalid _inputField.name : " + _inputField.name);
            }
        }

    }
}