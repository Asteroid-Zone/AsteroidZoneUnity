using Photon.GameControllers;
using PlayGame.Player;
using Statics;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI
{
    public class QuestManager : MonoBehaviour {
    
        public GameObject player;

        private PlayerData _playerData;
        
        private Text _text;

        private void Start() {
            player = !DebugSettings.Debug ? PhotonPlayer.Instance.myAvatar : TestPlayer.GetPlayerShip();
            _text = GetComponent<Text>();
            _playerData = player.GetComponent<PlayerData>();
        }

        private void Update()
        {
            string questStr = questToString(_playerData.currentQuest);
            _text.text = questStr;
        }

        private string questToString(QuestType quest)
        {
            switch (quest)
            {   
                case QuestType.DefendStation:
                    return "Defend the station.";
                case QuestType.MineAsteroids:
                    return "Mine asteroids.";
                case QuestType.PirateWarning:
                    return "Deal with pirates in the area.";
                case QuestType.ReturnToStation:
                    return "Return to the space station.";
            }

            return "";
        }
        
    }
}