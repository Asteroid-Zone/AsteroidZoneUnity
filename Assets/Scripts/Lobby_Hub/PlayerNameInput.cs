using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

namespace MainMenu
{
    /// Player name input field. Let the user input their name, will appear above the player in the game.
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInput : MonoBehaviour
    {
        // Store the PlayerPref Key to avoid typos
        const string playerNamePrefKey = "PlayerName";


        void Start()
        {

            string defaultName = string.Empty;
            InputField _inputField = this.GetComponent<InputField>();
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
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


            PlayerPrefs.SetString(playerNamePrefKey, value);
        }


    }
}
