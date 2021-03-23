using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace MainMenu {
    /// Player name input field. Let the user input their name, will appear above the player in the game.
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        // Store the PlayerPref Key to avoid typos
        private const string PlayerNamePrefKey = "PlayerName";

        private void Start()
        {
            string defaultName = string.Empty;
            InputField inputField = GetComponent<InputField>();
            if (inputField != null)
            {
                if (PlayerPrefs.HasKey(PlayerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(PlayerNamePrefKey);
                    inputField.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
        }

        /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
        public void SetPlayerName(string value)
        {
            // #Important
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }
            PhotonNetwork.NickName = value;
            
            PlayerPrefs.SetString(PlayerNamePrefKey, value);
        }
    }
}