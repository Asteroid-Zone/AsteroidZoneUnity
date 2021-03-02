using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI {
    public class EventsManager : MonoBehaviour
    {
        public GameObject scrollParent;
        
        private Text _eventsListText;
        private ScrollRect _scrollRect;
        private static readonly List<string> EventMessagesQueue = new List<string>();

        private void Start()
        {
            // Get the sub-objects
            _eventsListText = GetComponent<Text>();
            _scrollRect = scrollParent.GetComponent<ScrollRect>();
        }

        private void Update()
        {
            // Add each message to the events object
            foreach (var eventMessage in EventMessagesQueue)
            {
                AddEventMessage(eventMessage);
            }

            // Scroll to the bottom if a new message was added
            if (EventMessagesQueue.Count > 0)
            {
                _scrollRect.normalizedPosition = new Vector2(0, 0);
            }
        
            EventMessagesQueue.Clear();
        }

        private void AddEventMessage(string message)
        {
            _eventsListText.text += $"[{DateTime.Now}]"
                                    + Environment.NewLine
                                    + message
                                    + Environment.NewLine + Environment.NewLine;
        }

        public static void AddMessageToQueue(string message)
        {
            EventMessagesQueue.Add(message);
        }
    }
}
