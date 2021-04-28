using Photon.Pun;
using UnityEngine.UI;

namespace MainMenu {
    
    /// <summary>
    /// This class provides the <c>RoomListing</c> functions.
    /// </summary>
    public class RoomButton : MonoBehaviourPunCallbacks {
       
        public Text nameText;

        public string roomName;

        /// <summary>
        /// Sets the text to the room name.
        /// </summary>
        public void SetRoom() {
            nameText.text = roomName;
        }

        /// <summary>
        /// <para>Method is called when the player clicks the RoomListing.</para>
        /// Joins the PhotonRoom and destroys the RoomListing.
        /// </summary>
        public void JoinRoom() {
            PhotonNetwork.JoinRoom(roomName);
            Destroy(gameObject);
        }
    }
}
