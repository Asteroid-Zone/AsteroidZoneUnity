using Photon;
using Statics;
using UnityEngine;

namespace PlayGame.VoiceChat
{
    public class VoiceChatController : MonoBehaviour
    {
        private static bool _chatRunning;
        private void Start()
        {
            if (!DebugSettings.Debug && PhotonRoom.Instance != null)
            {
                JoinVoiceChat(PhotonRoom.Instance.name);
                
                // Initially mute the voice chat
                MuteUnmuteMyselfInVoiceChat();
                _chatRunning = true;
            }
        }

        private void OnDestroy()
        {
            if (!DebugSettings.Debug && PhotonRoom.Instance != null)
            {
                LeaveVoiceChat();
                _chatRunning = false;
            }
        }

        private static void JoinVoiceChat(string roomId) {
            Application.ExternalCall("joinVoiceChat", roomId);
        }

        private static void LeaveVoiceChat()
        {
            Application.ExternalCall("leaveVoiceChat");
        }

        public static void MuteUnmuteMyselfInVoiceChat()
        {
            if (_chatRunning)
            {
                Application.ExternalCall("muteUnmuteMyselfInVoiceChat");
            }
        }
    }
}
