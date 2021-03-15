using PlayGame.UI;

namespace PlayGame.SpaceStation {
    public class Hyperdrive : StationModule {

        private const int MaxHealth = 100;
        
        private readonly SpaceStation _spaceStation;
        
        public Hyperdrive(SpaceStation station) : base("Hyperdrive", MaxHealth) {
            _spaceStation = station;
        }

        // Activate the hyperdrive and win the game
        public void Activate() {
            if (isFunctional()) _spaceStation.GameOver(true);
            else EventsManager.AddMessage("Hyperdrive is not functional");
        }
        
    }
}