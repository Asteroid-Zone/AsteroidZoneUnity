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

        private void Update()
        {
            _currentQuest = _playerData.GetQuest();
            string questStr = questToString(_currentQuest);
            _text.text = questStr;
        }

        private string questToString(QuestType quest)
        {
            switch (quest)
            {   
                case QuestType.DefendStation:
                    return "Defend the station";
                case QuestType.MineAsteroids:
                    return "Mine asteroids";
                case QuestType.PirateWarning:
                    return "Deal with pirates in the area";
                case QuestType.ResourcesToStation:
                    return "Deliver resources to the space station";
            }

            return "";
        }
        
    }
}