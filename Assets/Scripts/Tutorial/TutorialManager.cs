using System.Collections;
using System.Collections.Generic;
using PlayGame.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial {
    public class TutorialManager : MonoBehaviour {

        private bool _introComplete = false;
        private bool _tutorialComplete = false;

        public Text inputText;
        public Text subtitlesText;

        private readonly List<string> _yes = new List<string>{"yes", "yeah"};
        private readonly List<string> _no = new List<string>{"no", "nah", "nope"};

        private void Start() {
            StartCoroutine(TutorialIntro());
        }
        
        private IEnumerator TutorialIntro() {
            while (!_introComplete) {
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

                _introComplete = true;
                StartCoroutine(_yes.Contains(inputText.text) ? CommanderTutorial() : MinerTutorial());
            }
        }

        private IEnumerator MinerTutorial() {
            while (!_tutorialComplete) {
                yield return new WaitForSeconds(0.1f);
                subtitlesText.text = "Welcome to the miner tutorial!";
            }
        }
        
        private IEnumerator CommanderTutorial() {
            while (!_tutorialComplete) {
                yield return new WaitForSeconds(0.1f);
                subtitlesText.text = "Welcome to the commander tutorial!";
            }
        }
        
    }
}