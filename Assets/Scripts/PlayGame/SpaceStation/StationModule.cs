using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

namespace PlayGame.SpaceStation {
    public class StationModule {
        
        protected readonly SpaceStation spaceStation;

        private readonly Renderer _renderer;
        private readonly Material _damagedMaterial;
        private readonly Material _functionalMaterial;
        
        public readonly string name;
        public readonly int maxHealth;
        public int moduleHealth;

        protected StationModule(string name, int maxHealth, SpaceStation station, string path) {
            this.name = name;
            this.maxHealth = maxHealth;
            spaceStation = station;
            moduleHealth = Random.Range(0, maxHealth / 2); // Set initial health to be less than half
            _renderer = station.transform.Find(path).gameObject.GetComponent<Renderer>();
            _damagedMaterial = Resources.Load<Material>("Materials/DamagedStationMaterial");
            _functionalMaterial = Resources.Load<Material>("Materials/StationMaterial");
            UpdateMesh();
        }

        private void UpdateMesh() {
            _renderer.material = IsFunctional() ? _functionalMaterial : _damagedMaterial;
        }

        public void Repair(int resources) {
            int repairAmount = GetRepairAmount(resources);
            
            moduleHealth += repairAmount;
            spaceStation.resources -= repairAmount;
            if (IsFunctional()) UpdateMesh();
        }

        // Returns the repair amount, minimum of station resources, remaining health to repair and the amount chosen by the player
        private int GetRepairAmount(int resources) {
            return Math.Min(Math.Min(maxHealth - moduleHealth, spaceStation.resources), resources);
        }

        protected bool IsFunctional() {
            return moduleHealth >= maxHealth;
        }

        public virtual void TakeDamage(int damage) {
            moduleHealth -= damage;
            if (moduleHealth < 0) moduleHealth = 0;
            if (!IsFunctional()) UpdateMesh();
        }

        public override string ToString() {
            return name + ": " + moduleHealth + "/" + maxHealth;
        }
    }
}