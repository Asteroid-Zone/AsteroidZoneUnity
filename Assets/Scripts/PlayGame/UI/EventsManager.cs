using System;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI {
    
    /// <summary>
    /// This class displays the event messages on the canvas.
    /// </summary>
    public class EventsManager : MonoBehaviour {
        
        #region Singleton
        private static EventsManager _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            } else {
                _instance = this;
            }
        }

        public static void ResetStaticVariables() {
            _instance = null;
        }
        #endregion
        
        public GameObject scrollParent;
        
        private Text _eventsListText;
        private ScrollRect _scrollRect;

        private void Start() {
            _eventsListText = GetComponent<Text>();
            _scrollRect = scrollParent.GetComponent<ScrollRect>();
        }
        
        /// <summary>
        /// Adds a new event message to the event panel.
        /// </summary>
        /// <param name="message"></param>
        public static void AddMessage(string message) {
            _instance._eventsListText.text += $"[{DateTime.Now}]"
                                              + Environment.NewLine
                                              + message
                                              + Environment.NewLine + Environment.NewLine;
            
            _instance._scrollRect.normalizedPosition = new Vector2(0, 0);
        }
    }
}
