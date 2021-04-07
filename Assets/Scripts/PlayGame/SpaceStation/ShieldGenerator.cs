using Statics;
using UnityEngine;

namespace PlayGame.SpaceStation {
    public class ShieldGenerator : StationModule {

        private int _shields = 0;
        private float _timeSinceLastCharge = 1; // Seconds since last charge

        public ShieldGenerator(SpaceStation station) : base("Shield Generator", GameConstants.ShieldGeneratorMaxHealth, station, "SpaceStation/station/shield_generator") {
            _damagedTexture = Resources.Load<Texture>(Textures.ShieldGeneratorDamaged);
            _functionalTexture = Resources.Load<Texture>(Textures.ShieldGenerator);
            UpdateMesh();
        }
        
        public void Update() {
            if (!IsFunctional()) return;
            
            // Only charge the shields every second
            if (_timeSinceLastCharge >= 1) {
                _timeSinceLastCharge = 0;
                RechargeShields();
            } else {
                _timeSinceLastCharge += Time.deltaTime;
            }
        }

        private void RechargeShields() {
            _shields += GameConstants.StationShieldsRechargeRate; // Charge the shields
            if (_shields > GameConstants.StationMaxShields) _shields = GameConstants.StationMaxShields;
        }

        // Shields take as much of the damage as they can, return the rest of the damage
        public int AbsorbDamage(int damage) {
            _shields -= damage;
            if (_shields < 0) {
                int damageRemaining = _shields * -1;
                _shields = 0;
                return damageRemaining;
            }
            return 0;
        }

        public override string ToString() {
            return base.ToString() + "\nShields: " + _shields + "/" + GameConstants.StationMaxShields;
        }
    }
}