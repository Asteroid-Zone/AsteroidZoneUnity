using System;
using Photon.GameControllers;
using PlayGame.Player;
using Statics;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI
{
    public class DisplayPlayerQuest : MonoBehaviour {
    
        public GameObject player;

        private PlayerData _playerData;
        private QuestType _currentQuest;

        private Text _text;
        

        private void Start() {
            player = !DebugSettings.Debug ? PhotonPlayer.Instance.myAvatar : TestPlayer.GetPlayerShip();
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
                    return "The station is under attack! Get back and help! (try saying 'Go to space station')";
                case QuestType.DefendStation:
                    return "The station is under attack! Get back and help! (try saying 'Shoot pirates')";
                case QuestType.MineAsteroids:
                    return "Mine asteroids (try saying 'Mine asteroids')";
                case QuestType.PirateWarning:
                    return "Be careful there are pirates in the area (try saying 'Shoot pirates')";
                case QuestType.ReturnToStationResources:
                    return "Deliver resources to the space station (try saying 'Go to space station')";
                case QuestType.TransferResources:
                    return "Deliver resources to the space station (try saying 'Transfer resources')";
                case QuestType.HelpPlayers:
                    return "Give information to your miners. Warn them about nearby pirates or advise them on where to mine. You can see more than they can. (Make sure you're in chat mode)";
                case QuestType.RepairStation:
                    return "Use the resources to repair the station (try saying 'repair [module name]') You can see which modules are damaged in the top left of your screen.";
                case QuestType.ActivateHyperdrive:
                    return "Activate the hyperdrive and escape! (try saying 'activate hyperdrive')";
                default:
                    throw new ArgumentOutOfRangeException(nameof(quest), quest, null);
            }
        }
        
    }
}