using System;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI {
    public class EventsManager : MonoBehaviour
    {
        
        private static EventsManager _instance;

        private static EventsManager Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            } else {
                _instance = this;
            }
        }
        
        public GameObject scrollParent;
        
        private Text _eventsListText;
        private ScrollRect _scrollRect;

        private void Start()
        {
            // Get the sub-objects
            _eventsListText = GetComponent<Text>();
            _scrollRect = scrollParent.GetComponent<ScrollRect>();
        }
        

        public static void AddMessage(string message)
        {
            Instance._eventsListText.text += $"[{DateTime.Now}]"
                                    + Environment.NewLine
                                    + message
                                    + Environment.NewLine + Environment.NewLine;
            
            Instance._scrollRect.normalizedPosition = new Vector2(0, 0);
        }
    }
}
