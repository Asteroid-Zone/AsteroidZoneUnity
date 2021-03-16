using PlayGame.UI;

namespace PlayGame.SpaceStation {
    public class Hyperdrive : StationModule {

        private const int MaxHealth = 100;

        public Hyperdrive(SpaceStation station) : base("Hyperdrive", MaxHealth, station, "station/hyperdrive") {
        }

        // Activate the hyperdrive and win the game
        public void Activate() {
            if (IsFunctional()) spaceStation.GameOver(true);
            else EventsManager.AddMessage("Hyperdrive is not functional");
        }
        
    }
}