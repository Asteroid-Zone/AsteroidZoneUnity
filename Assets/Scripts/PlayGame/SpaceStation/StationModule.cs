using System;
using Random = UnityEngine.Random;

namespace PlayGame.SpaceStation {
    public class StationModule {
        
        protected readonly SpaceStation spaceStation;
        
        public readonly string name;
        public readonly int maxHealth;
        public int moduleHealth;

        protected StationModule(string name, int maxHealth, SpaceStation station) {
            this.name = name;
            this.maxHealth = maxHealth;
            spaceStation = station;
            moduleHealth = Random.Range(0, maxHealth / 2); // Set initial health to be less than half
        }

        public void Repair(int resources) {
            int repairAmount = GetRepairAmount(resources);
            
            moduleHealth += repairAmount;
            spaceStation.resources -= repairAmount;

            //if (moduleHealth > maxHealth) moduleHealth = maxHealth;
        }

        // Returns the repair amount, minimum of station resources, remaining health to repair and the amount chosen by the player
        private int GetRepairAmount(int resources) {
            return Math.Min(Math.Min(maxHealth - moduleHealth, spaceStation.resources), resources);
        }
        
        public bool IsFunctional() {
            return moduleHealth >= maxHealth;
        }

        public virtual void TakeDamage(int damage) {
            moduleHealth -= damage;
            if (moduleHealth < 0) moduleHealth = 0;
        }

        public override string ToString() {
            return name + ": " + moduleHealth + "/" + maxHealth;
        }
    }
}