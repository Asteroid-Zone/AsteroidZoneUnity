using System;
using Photon.GameControllers;
using PlayGame.Player;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayGame.UI
{
    public class DisplayPlayerQuest : MonoBehaviour {
    
        public GameObject player;
       // public AudioSource battleMusic;
       // public AudioSource gameMusic;

        private PlayerData _playerData;
        private QuestType _currentQuest;

        private Text _text;
        
        private void Start() {
            if (SceneManager.GetActiveScene().name == Scenes.TutorialScene) return;
            player = (!DebugSettings.Debug) ? PhotonPlayer.Instance.myAvatar : TestPlayer.GetPlayerShip();
            _text = GetComponent<Text>();
            _playerData = player.GetComponent<PlayerData>();
        }

        private void Update() {
            if (_playerData == null) return;
            _currentQuest = _playerData.GetQuest();
            string questStr = questToString(_currentQuest);
            _text.text = questStr;
        }

        private string questToString(QuestType quest) {
            switch (quest) {
                case QuestType.ReturnToStationDefend:
                   /* if (!battleMusic.isPlaying)
                    {
                        gameMusic.Stop();
                        battleMusic.Play();
                    }*/
                    return "The station is under attack! Get back and help! (try saying 'Go to space station')";
                case QuestType.DefendStation:
                  /*  if (!battleMusic.isPlaying)
                    {
                        gameMusic.Stop();
                        battleMusic.Play();
                    }*/
                    return "The station is under attack! Get back and help! (try saying 'Shoot pirates')";
                case QuestType.MineAsteroids:
                  /*  if (!gameMusic.isPlaying)
                    {
                        battleMusic.Stop();
                        gameMusic.Play();
                    }*/
                    return "Mine asteroids (try saying 'Mine asteroids')";
                case QuestType.FindAsteroids:
                  /*  if (!gameMusic.isPlaying)
                    {
                        battleMusic.Stop();
                        gameMusic.Play();
                    }*/
                    return "Explore to find asteroids or ask your station commander for help (try saying 'Move forwards' and 'Turn left/right' to move)";
                case QuestType.PirateWarning:
                   /* if (!battleMusic.isPlaying)
                    {
                        gameMusic.Stop();
                        battleMusic.Play();
                    }*/
                    return "Be careful there are pirates in the area (try saying 'Shoot pirates')";
                case QuestType.ReturnToStationResources:
                   /* if (!gameMusic.isPlaying)
                    {
                        battleMusic.Stop();
                        gameMusic.Play();
                    }*/
                    return "Deliver resources to the space station (try saying 'Go to space station')";
                case QuestType.TransferResources:
                  /*  if (!gameMusic.isPlaying)
                    {
                        battleMusic.Stop();
                        gameMusic.Play();
                    }*/
                    return "Deliver resources to the space station (try saying 'Transfer resources')";
                case QuestType.HelpPlayers:
                  /*  if (!gameMusic.isPlaying)
                    {
                        battleMusic.Stop();
                        gameMusic.Play();
                    }*/
                    return "Give information to your miners. Warn them about nearby pirates or advise them on where to mine. You can see more than they can. (Make sure you're in chat mode)";
                case QuestType.RepairStation:
                   /* if (!gameMusic.isPlaying)
                    {
                        battleMusic.Stop();
                        gameMusic.Play();
                    }*/
                    return "Use the resources to repair the station (try saying 'repair [module name]') You can see which modules are damaged in the top left of your screen.";
                case QuestType.ActivateHyperdrive:
                   /* if (!gameMusic.isPlaying)
                    {
                        battleMusic.Stop();
                        gameMusic.Play();
                    }*/
                    return "Activate the hyperdrive and escape! (try saying 'activate hyperdrive')";
                case QuestType.Respawn:
                   /* if (!gameMusic.isPlaying)
                    {
                        battleMusic.Stop();
                        gameMusic.Play();
                    }*/
                    return "Ask the station commander to respawn you (resources required)";
                default:
                    throw new ArgumentOutOfRangeException(nameof(quest), quest, null);
            }
        }
        
    }
}