using System.Collections;
using System.Collections.Generic;
using PlayGame;
using PlayGame.Camera;
using PlayGame.Pings;
using PlayGame.Pirates;
using PlayGame.Player;
using PlayGame.Player.Movement;
using PlayGame.Speech.Commands;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Ping = PlayGame.Pings.Ping;

namespace Tutorial {
    public class TutorialManager : MonoBehaviour {

        public Text inputText;
        public Text subtitlesText;

        private readonly List<string> _yes = new List<string>{"yes", "yeah"};
        private readonly List<string> _no = new List<string>{"no", "nah", "nope"};

        private PirateSpawner _pirateSpawner;
        private AsteroidSpawner _asteroidSpawner;
        private PingManager _pingManager;
        private CameraManager _cameraManager;
        private MoveObject _moveObject;
        public GameObject blackScreenPanel;

        public static Command LatestCommand;

        private readonly GridCoord _asteroidLocation = new GridCoord(7, 8);

        private bool _tutorialComplete = false;

        // todo set debug false when finished
        
        private void Start() {
            _cameraManager = GameObject.FindGameObjectWithTag(Tags.CameraManagerTag).GetComponent<CameraManager>();
            _pingManager = GameObject.FindGameObjectWithTag(Tags.PingManagerTag).GetComponent<PingManager>();
            _asteroidSpawner = AsteroidSpawner.GetInstance();
            _pirateSpawner = PirateSpawner.GetInstance();
            _moveObject = TestPlayer.GetPlayerShip().GetComponent<MoveObject>();
            StartCoroutine(TutorialIntro());
            
            _asteroidSpawner.SpawnAsteroid(_asteroidLocation);
        }

        private void Update() {
            if (!_tutorialComplete) return;
            
            StopAllCoroutines();
            DebugSettings.Debug = false;
            
            CleanUpGameObjects();
            GameManager.ResetStaticVariables();
            SceneManager.LoadScene(Scenes.MainMenuScene);
        }
        
        private void CleanUpGameObjects() {
            foreach (GameObject g in FindObjectsOfType<GameObject>()) {
                if (!g.name.Equals("PhotonMono")) Destroy(g);
            }
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
                
                StartCoroutine(CommanderTutorial());
            } else {
                subtitlesText.text = "Acknowledged. Let's run your ship through some diagnostics first to ensure that all systems are operational. Entering virtualisation.";
                yield return new WaitForSeconds(2f);

                StartCoroutine(MinerTutorial());
            }
        }

        private IEnumerator CommanderTutorial() {
            blackScreenPanel.SetActive(false);
            _cameraManager.SetMode(false);
                
            subtitlesText.text = "Welcome to the commander tutorial!";
            yield return null;

            _tutorialComplete = true;
        }

        private IEnumerator MinerTutorial() {
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
            yield return new WaitForSeconds(2f);
            
            subtitlesText.text = "We'll be able to track where you are. As a result, you can make use of this coordinate system. Try to move to C3.";
            GridCoord c3 = new GridCoord('c', 3);
            while (!WaitForMovementGrid()) yield return null;
            while (!GridCoord.GetCoordFromVector(_moveObject.transform.position).Equals(c3)) yield return null;
            
            subtitlesText.text = "Your station commander, who's also keeping an eye out, may mark a point of interest (a 'ping'). Try moving to it.";
            GridCoord pingLocation = new GridCoord(_asteroidLocation.GetX() - 1, _asteroidLocation.GetZ());
            Ping testPing = new Ping(pingLocation, PingType.Asteroid);
            _pingManager.AddPing(testPing);
            while (!WaitForMovementDestination(MovementCommand.DestinationType.Ping)) {
                if (_pingManager.GetPings().Count == 0) _pingManager.AddPing(testPing);
                yield return null;
            }
            while (!GridCoord.GetCoordFromVector(_moveObject.transform.position).Equals(pingLocation)) yield return null;

            subtitlesText.text = "That covers movement. Note that there's an asteroid here. Try mining it. Your ship should automatically detect and face it.";
            while (!WaitForToggleCommand(ToggleCommand.ObjectType.MiningLaser)) yield return null;
            
            subtitlesText.text = "This is the lock-on system. You can also engage or disengage it manually, but your ship should be able to handle it automatically with the right commands.";
            yield return new WaitForSeconds(4f);
            subtitlesText.text = "You may have noticed that this asteroid was not visible before. As you broke down in multidimensional travel, you can't actually see your surroundings. Your ship's emergency proximity sensor is handling everything, indicated by this blue circle. Pirates and asteroids will only be visible inside that.";
            yield return new WaitForSeconds(8f);
            subtitlesText.text = "Your station commander can see everything fine, however. They have highly specialised and unwieldy equipment allowing them to do so.";
            yield return new WaitForSeconds(6f);
            subtitlesText.text = "Anyway, your resource count should have gone up now. You'll need to deliver this back to the station. You can simply tell your ship to go back to it.";
            while (!WaitForMovementDestination(MovementCommand.DestinationType.SpaceStation)) yield return null;
            while (!_moveObject.NearStation()) yield return null;
            
            subtitlesText.text = "Tell it to transfer the resources.";
            while (!WaitForTransferCommand()) yield return null;

            GridCoord playerLocation = GridCoord.GetCoordFromVector(_moveObject.transform.position);
            GameObject pirate = _pirateSpawner.SpawnPirate(PirateData.PirateType.Scout, new GridCoord(playerLocation.GetX() + 1, playerLocation.GetZ()));
            subtitlesText.text = "And that's that. Now unfortunately, it's not going to be this simple. Remember when I said you're stranded in a problem sector? Well, here comes a pirate. Command your ship to shoot it.";
            while (pirate != null) yield return null;
            
            playerLocation = GridCoord.GetCoordFromVector(_moveObject.transform.position);
            pirate = _pirateSpawner.SpawnPirate(PirateData.PirateType.Elite, new GridCoord(playerLocation.GetX() + 1, playerLocation.GetZ()));
            subtitlesText.text = "That was one of the grunts. Here comes a more powerful one.";
            while (pirate != null) yield return null;

            subtitlesText.text = "And that was an elite. Our data says that only these two types are currently in that area. You're going to have to protect the station from them. If they find it, they'll most likely swarm it. Failure is not an option.";
            yield return new WaitForSeconds(6f);
            subtitlesText.text = "W-wait. There's... a large energy reading heading your way. If I had to guess... that's their leader. You definitely aren't going to be able to defend the station from THAT thing. It's looking like you only have about 5 minutes.";
            yield return new WaitForSeconds(6f);
            subtitlesText.text = "Is everything clear? Deliver 1500 grams of Asteral to the station as a team. Ensure that the pirates don't destroy it. You only have 5 minutes.";
            yield return new WaitForSeconds(5f);
            subtitlesText.text = "Acknowledged. Commence operation.";
            yield return new WaitForSeconds(2f);

            _tutorialComplete = true;
        }

        private bool WaitForTransferCommand() {
            if (LatestCommand == null) return false;
            if (LatestCommand.GetCommandType() == Command.CommandType.Transfer) return true;
            return false;
        }
        
        private bool WaitForToggleCommand(ToggleCommand.ObjectType objectType) {
            if (LatestCommand == null) return false;
            if (LatestCommand.GetCommandType() != Command.CommandType.Toggle) return false;
            ToggleCommand command = (ToggleCommand) LatestCommand;

            if (!command.on) return false;
            if (command.objectType == objectType) return true;
            return false;
        }

        private bool WaitForMovementDestination(MovementCommand.DestinationType destinationType) {
            if (LatestCommand == null) return false;
            if (LatestCommand.GetCommandType() != Command.CommandType.Movement) return false;
            MovementCommand command = (MovementCommand) LatestCommand;
            
            if (command.movementType != MovementCommand.MovementType.Destination) return false;
            if (command.destinationType == destinationType) return true;
            return false;
        }
        
        private bool WaitForMovementGrid() {
            if (LatestCommand == null) return false;
            if (LatestCommand.GetCommandType() != Command.CommandType.Movement) return false;
            MovementCommand command = (MovementCommand) LatestCommand;
            
            if (command.movementType != MovementCommand.MovementType.Grid) return false;
            if (command.gridCoord.Equals(new GridCoord('c', 3))) return true;
            return false;
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