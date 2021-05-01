using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

namespace MainMenu {
    
    /// <summary>
    /// This class provides the <c>RoomListing</c> functions.
    /// </summary>
    public class RoomButton : MonoBehaviourPunCallbacks {
       
        public Text nameText;
        public Text playersText;

        public RoomInfo roomInfo;

        /// <summary>
        /// Sets the text to the room name.
        /// </summary>
        public void SetRoom() {
            nameText.text = roomInfo.Name;
            playersText.text = $"{roomInfo.PlayerCount}/{roomInfo.MaxPlayers}";
        }

        /// <summary>
        /// <para>Method is called when the player clicks the RoomListing.</para>
        /// Joins the PhotonRoom and destroys the RoomListing.
        /// </summary>
        public void JoinRoom() {
            PhotonNetwork.JoinRoom(roomInfo.Name);
            Destroy(gameObject);
        }
    }
}
