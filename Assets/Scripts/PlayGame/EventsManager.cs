using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame
{
    public class EventsManager : MonoBehaviour
    {
        private Text _eventsListText;
        private static readonly List<string> EventMessagesQueue = new List<string>();

        private void Start()
        {
            _eventsListText = GetComponent<Text>();
        }

        private void Update()
        {
            foreach (var eventMessage in EventMessagesQueue)
            {
                AddEventMessage(eventMessage);
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
