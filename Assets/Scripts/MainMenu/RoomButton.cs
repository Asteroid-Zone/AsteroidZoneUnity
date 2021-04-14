using Photon.Pun;
using UnityEngine.UI;

namespace MainMenu {
    public class RoomButton : MonoBehaviourPunCallbacks {
       
        public Text nameText;

        public string roomName;

        public void SetRoom()
        {
            nameText.text = roomName;
        }

        public void JoinRoom()
        {
            PhotonNetwork.JoinRoom(roomName);
            Destroy(gameObject);
        }
    }
}
