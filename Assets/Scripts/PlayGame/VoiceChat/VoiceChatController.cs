using UnityEngine;

namespace PlayGame.VoiceChat
{
    public class VoiceChatController : MonoBehaviour
    {

        public static void JoinVoiceChat(string roomId) {
            Application.ExternalCall("joinVoiceChat", roomId);
        }

        public static void LeaveVoiceChat()
        {
            Application.ExternalCall("leaveVoiceChat");
        }

        public static void MuteMyselfInVoiceChat()
        {
            Application.ExternalCall("muteMyselfInVoiceChat");
        }
        
        public static void UnmuteMyselfInVoiceChat()
        {
            Application.ExternalCall("unmuteMyselfInVoiceChat");
        }
    }
}
