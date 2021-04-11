using Photon.Pun;
using Statics;
using UnityEngine;

namespace PlayGame.VoiceChat
{
    public class VoiceChatController : MonoBehaviour
    {
        private static bool _chatRunning;
        private void Start()
        {
            // Check if game is played in multiplayer
            if (DebugSettings.Debug || PhotonNetwork.CurrentRoom == null) return;
            
            // Join the voice chat
            JoinVoiceChat(PhotonNetwork.CurrentRoom.Name);
            _chatRunning = true;
        }

        private void OnDestroy()
        {
            // Check if chat is running at all
            if (!_chatRunning) return;
            
            LeaveVoiceChat();
            _chatRunning = false;
        }

        private static void JoinVoiceChat(string roomId) {
            Application.ExternalCall("joinVoiceChat", roomId);
        }

        private static void LeaveVoiceChat()
        {
            Application.ExternalCall("leaveVoiceChat");
        }

        public static void MuteMyselfInVoiceChat()
        {
            if (_chatRunning)
            {
                Application.ExternalCall("muteMyselfInVoiceChat");
            }
        }
        
        public static void UnmuteMyselfInVoiceChat()
        {
            if (_chatRunning)
            {
                Application.ExternalCall("unmuteMyselfInVoiceChat");
            }
        }
    }
}
