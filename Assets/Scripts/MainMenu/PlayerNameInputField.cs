using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace MainMenu {
    
    /// <summary>
    /// <para>This class provides the NameInputField functions.</para>
    /// Requires <c>InputField</c> for player name input.
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour {
        
        // Store the PlayerPref Key to avoid typos
        private const string PlayerNamePrefKey = "PlayerName";

        private void Start() {
            string defaultName = string.Empty;
            InputField inputField = GetComponent<InputField>();
            if (inputField != null) {
                // Set the players name to their last used name if they have played before
                if (PlayerPrefs.HasKey(PlayerNamePrefKey)) {
                    defaultName = PlayerPrefs.GetString(PlayerNamePrefKey);
                    inputField.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
        }

        /// <summary>
        /// Sets the players Photon name and saves it in the PlayerPrefs.
        /// </summary>
        public void SetPlayerName(string value) {
            if (string.IsNullOrEmpty(value)) {
                Debug.LogError("Player Name is null or empty");
                return;
            }
            
            PhotonNetwork.NickName = value;
            PlayerPrefs.SetString(PlayerNamePrefKey, value);
        }
    }
}