using Statics;
using UnityEngine;

namespace PlayGame.SpaceStation {
    
    /// <summary>
    /// This class controls the stations shield generator module.
    /// </summary>
    public sealed class ShieldGenerator : StationModule {

        private int _shields = 0;
        private float _timeSinceLastCharge = 1; // Seconds since last charge

        /// <summary>
        /// Loads the shield generators textures.
        /// </summary>
        /// <param name="station"></param>
        public ShieldGenerator(SpaceStation station) : base("Shield Generator", GameConstants.ShieldGeneratorMaxHealth, station, "SpaceStation/station/shield_generator") {
            DamagedTexture = Resources.Load<Texture>(Textures.ShieldGeneratorDamaged);
            FunctionalTexture = Resources.Load<Texture>(Textures.ShieldGenerator);
            UpdateMesh();
        }
        
        /// <summary>
        /// Recharges the shields if the shield generator is functional and it has been long enough since they were last recharged.
        /// </summary>
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

        /// <summary>
        /// Increases the shields charge.
        /// </summary>
        private void RechargeShields() {
            _shields += GameConstants.StationShieldsRechargeRate; // Charge the shields
            if (_shields > GameConstants.StationMaxShields) _shields = GameConstants.StationMaxShields;
        }

        /// <summary>
        /// Reduces the shields charge by the damage amount.
        /// <para>Returns the remaining damage if the shields charge is less than the damage amount.</para>
        /// </summary>
        /// <param name="damage">The total amount of damage.</param>
        public int AbsorbDamage(int damage) {
            _shields -= damage;
            if (_shields >= 0) return 0;
            
            int damageRemaining = _shields * -1;
            _shields = 0;
            return damageRemaining;
        }

        /// <summary>
        /// Returns a string that contains the shields current charge and the max charge.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return base.ToString() + "\nShields: " + _shields + "/" + GameConstants.StationMaxShields;
        }
    }
}