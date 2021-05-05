using System.Collections;
using System.Collections.Generic;
using PlayGame.Camera;
using PlayGame.Speech.Commands;
using Statics;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial {
    public class TutorialManager : MonoBehaviour {

        public Text inputText;
        public Text subtitlesText;

        private readonly List<string> _yes = new List<string>{"yes", "yeah"};
        private readonly List<string> _no = new List<string>{"no", "nah", "nope"};

        private CameraManager _cameraManager;
        public GameObject blackScreenPanel;

        public static Command LatestCommand;

        private void Start() {
            _cameraManager = GameObject.FindGameObjectWithTag(Tags.CameraManagerTag).GetComponent<CameraManager>();
            StartCoroutine(TutorialIntro());
        }
        
        private IEnumerator TutorialIntro() {
            blackScreenPanel.SetActive(true);
            
            yield return new WaitForSeconds(0.1f);
            
            subtitlesText.text = "[Incoming Transmission From The Mothership]";
            yield return new WaitForSeconds(2.5f);
            
            subtitlesText.text = "Hello? Are you awake? Listen, we don't have much time.";
            yield return new WaitForSeconds(3f);
            
            subtitlesText.text = "That recon mission you were on? Yeah, it's cancelled. Your station malfunctioned while attempting to warp through hyperspace... it can repair itself, but it's going to need a specific mineral found in some nearby asteroids - Asteral. Thankfully, your ships should be well equipped for emergencies like these.";
            yield return new WaitForSeconds(10f);
            
            subtitlesText.text = "The bad news is that your team is stranded in a particularly troublesome sector of the multiverse. You're going to have some chance encounters with the pirates endemic to that area.";
            yield return new WaitForSeconds(6f);
            
            subtitlesText.text = "Three of you will be doing the busywork. One is the station commander who has special equipment allowing them to strategise.";
            yield return new WaitForSeconds(5f);
            
            subtitlesText.text = "The data isn't showing up for me at the moment... are you the station commander?";
            while (!_yes.Contains(inputText.text) && !_no.Contains(inputText.text)) {
                yield return null;
            }

            if (_yes.Contains(inputText.text)) {
                subtitlesText.text = "Acknowledged. Let's get you up to speed on what will be expected of you during this mission. Entering virtualisation.";
                yield return new WaitForSeconds(2f);
                
                blackScreenPanel.SetActive(false);
                _cameraManager.SetMode(false);
                
                subtitlesText.text = "Welcome to the commander tutorial!";
            } else {
                subtitlesText.text = "Acknowledged. Let's run your ship through some diagnostics first to ensure that all systems are operational. Entering virtualisation.";
                yield return new WaitForSeconds(2f);
                
                blackScreenPanel.SetActive(false);
                _cameraManager.SetMode(true);
            
                subtitlesText.text = "This is your ship. Attempt to interface with it through voice commands. Tell it to move forwards.";
                while(!WaitForMovement(Strings.Forward, null, false)) yield return null;
            
                subtitlesText.text = "Great! Now tell it to stop.";
                while(!WaitForStopCommand()) yield return null;
            
                subtitlesText.text = "And now backwards?";
                while(!WaitForMovement(Strings.Back, null, false)) yield return null;
            
                subtitlesText.text = "Okay, perfect. And now stop.";
                while(!WaitForStopCommand()) yield return null;
            
                subtitlesText.text = "Excellent, that's all functional. You can make it turn left or right. Try that.";
                while(!WaitForMovement(Strings.Right, Strings.Left, true)) yield return null;
            
                subtitlesText.text = "And stop?";
                while(!WaitForStopCommand()) yield return null;
            
                subtitlesText.text = "There we go. You can also directly move left or right as well. See for yourself.";
                while(!WaitForMovement(Strings.Right, Strings.Left, false)) yield return null;
            
                subtitlesText.text = "Great, that's enough.";
            }
        }

        private bool WaitForMovement(string direction, string direction2, bool turn) {
            if (LatestCommand == null) return false;
            if (LatestCommand.GetCommandType() != Command.CommandType.Movement) return false;
            MovementCommand command = (MovementCommand) LatestCommand;
            
            if (command.movementType != MovementCommand.MovementType.Direction) return false;
            if (command.turnOnly != turn) return false;
            if (command.directionString.Equals(direction)) return true;
            if (direction2 != null && command.directionString.Equals(direction2)) return true;
            return false;
        }

        private bool WaitForStopCommand() {
            if (LatestCommand == null) return false;
            if (LatestCommand.GetCommandType() != Command.CommandType.Speed) return false;
            SpeedCommand command = (SpeedCommand) LatestCommand;

            if (command.Speed == 0) return true;
            return false;
        }

    }
}