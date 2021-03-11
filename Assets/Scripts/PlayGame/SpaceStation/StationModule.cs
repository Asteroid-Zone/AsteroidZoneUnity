using UnityEngine;

namespace PlayGame.SpaceStation {
    public class StationModule
    {

        public readonly string name;
        public readonly int maxHealth;
        public int moduleHealth;

        protected StationModule(string name, int maxHealth) {
            this.name = name;
            this.maxHealth = maxHealth;
            moduleHealth = Random.Range(0, maxHealth / 2); // Set initial health to be less than half
        }

        public void Repair(int resources) {
            moduleHealth += resources;
            
            if (moduleHealth > maxHealth) moduleHealth = maxHealth;
        }
        
        public bool isFunctional() {
            return moduleHealth >= maxHealth;
        }
        
    }
}