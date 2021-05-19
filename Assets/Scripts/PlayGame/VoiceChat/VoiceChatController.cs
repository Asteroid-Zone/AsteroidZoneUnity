using UnityEngine;

namespace PlayGame.VoiceChat {
    
    /// <summary>
    /// This class controls the voice chat in the browser.
    /// </summary>
    public class VoiceChatController : MonoBehaviour {

        /// <summary>
        /// Calls joinVoiceChat() in the browser.
        /// </summary>
        /// <param name="roomId">ID of the voice chat room to join.</param>
        public static void JoinVoiceChat(string roomId) {
            Application.ExternalCall("joinVoiceChat", roomId);
        }

        /// <summary>
        /// Calls leaveVoiceChat() in the browser.
        /// </summary>
        public static void LeaveVoiceChat() {
            Application.ExternalCall("leaveVoiceChat");
        }

        /// <summary>
        /// Calls muteMyselfInVoiceChat() in the browser.
        /// </summary>
        public static void MuteMyselfInVoiceChat() {
            Application.ExternalCall("muteMyselfInVoiceChat");
        }
        
        /// <summary>
        /// Calls unmuteMyselfInVoiceChat() in the browser.
        /// </summary>
        public static void UnmuteMyselfInVoiceChat() {
            Application.ExternalCall("unmuteMyselfInVoiceChat");
        }
    }
}
