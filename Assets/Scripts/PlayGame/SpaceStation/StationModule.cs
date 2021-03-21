using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

namespace PlayGame.SpaceStation {
    public class StationModule {
        
        protected readonly SpaceStation spaceStation;

        protected Material _material;
        protected Texture _damagedTexture;
        protected Texture _functionalTexture;
        
        public readonly string name;
        public readonly int maxHealth;
        public int moduleHealth;

        protected StationModule(string name, int maxHealth, SpaceStation station, string path) {
            this.name = name;
            this.maxHealth = maxHealth;
            spaceStation = station;
            moduleHealth = Random.Range(0, maxHealth / 2); // Set initial health to be less than half
            Debug.Log(path);
            _material = station.transform.Find(path).gameObject.GetComponent<Renderer>().material;
            _damagedTexture = Texture2D.redTexture;
            _functionalTexture = Texture2D.whiteTexture;
            UpdateMesh();
        }
        
        protected StationModule(string name, int maxHealth, SpaceStation station) {
            this.name = name;
            this.maxHealth = maxHealth;
            spaceStation = station;
            moduleHealth = Random.Range(0, maxHealth / 2); // Set initial health to be less than half
        }

        protected virtual void UpdateMesh() {
            _material.mainTexture = IsFunctional() ? _functionalTexture : _damagedTexture;
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