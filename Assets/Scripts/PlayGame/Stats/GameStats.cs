﻿namespace PlayGame.Stats {
    public class GameStats {

        public bool victory;
        public GameManager.GameOverType gameOverType;
        
        public float startTime;
        public float gameTime;
        
        public int resourcesHarvested;
        public int asteroidsDestroyed;
        public int piratesDestroyed;

        public void Reset() {
            victory = false;
            startTime = 0;
            gameTime = 0;
            resourcesHarvested = 0;
            asteroidsDestroyed = 0;
            piratesDestroyed = 0;
        }

    }
}