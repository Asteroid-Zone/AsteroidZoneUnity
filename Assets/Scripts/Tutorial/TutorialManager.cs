﻿using System.Collections;
using System.Collections.Generic;
using PlayGame;
using PlayGame.Camera;
using PlayGame.Pings;
using PlayGame.Pirates;
using PlayGame.Player;
using PlayGame.Player.Movement;
using PlayGame.SpaceStation;
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
        public List<AudioClip> introAudioLines;
        public List<string> introTextLines;
        public List<AudioClip> minerAudioLines;
        public List<string> minerTextLines;
        

        private PirateSpawner _pirateSpawner;
        private AsteroidSpawner _asteroidSpawner;
        private PingManager _pingManager;
        private CameraManager _cameraManager;
        private MoveObject _moveObject;
        private PlayerData _playerData;
        private SpaceStation _spaceStation;
        private AudioSource _audioSource;
        public GameObject blackScreenPanel;

        public static Command LatestCommand;
        public static bool Zoom = false;
        public static bool Track = false;
        public static bool Rotate = false;
        public static bool Move = false;

        private bool _commander;
        
        private readonly GridCoord _asteroidLocation = new GridCoord(7, 8);

        private bool _tutorialComplete = false;
        
        private void Start() {
            _cameraManager = GameObject.FindGameObjectWithTag(Tags.CameraManagerTag).GetComponent<CameraManager>();
            _pingManager = GameObject.FindGameObjectWithTag(Tags.PingManagerTag).GetComponent<PingManager>();
            _spaceStation = GameObject.FindGameObjectWithTag(Tags.StationTag).GetComponent<SpaceStation>();
            _asteroidSpawner = AsteroidSpawner.GetInstance();
            _pirateSpawner = PirateSpawner.GetInstance();
            _moveObject = TestPlayer.GetPlayerShip().GetComponent<MoveObject>();
            _playerData = TestPlayer.GetPlayerShip().GetComponent<PlayerData>();
            _audioSource = GetComponent<AudioSource>();
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

            for (var i = 0; i < introTextLines.Count - 2; i++)
            {
                subtitlesText.text = introTextLines[i];
                _audioSource.clip = introAudioLines[i];
                _audioSource.Play();
                yield return new WaitForSeconds(_audioSource.clip.length);
            }
            
            while(!WaitForYesOrNo()) yield return null;

            var index = _commander ? 5 : 6;
            subtitlesText.text = introTextLines[index];
            yield return new WaitForSeconds(2f);

            StartCoroutine(_commander ? CommanderTutorial() : MinerTutorial());
        }

        private IEnumerator CommanderTutorial() {
            blackScreenPanel.SetActive(false);
            _playerData.viewableArea.SetActive(false);
            _playerData.viewableAreaMinimap.SetActive(false);
            PlayerData.SetActiveRecursively(_playerData.shipModel.gameObject, false); // Hide the ship model
            _cameraManager.SetMode(false);
            
            subtitlesText.text = "You're going to play a key strategic role in this escape. Use a voice command, and 'ping' or 'mark' the asteroid visible at E6. This will be visible to your team.";
            while(!WaitForPingCommand(PingType.Asteroid, new GridCoord('e', 6))) yield return null;
            
            subtitlesText.text = "Good.";
            yield return new WaitForSeconds(1.5f);
            subtitlesText.text = "Your squadmates can only see objects in areas close to them. As you all broke down in a multidimensional zone, your surroundings aren't actually fully visible and everything is being decoded by your ships on the fly. We weren't able to give them equipment as good as yours however - you can see the full map, they can only see what's directly around them. Use this to your advantage by pointing out things on certain tiles that your team can't see.";
            yield return new WaitForSeconds(12f);

            ResetVars();
            subtitlesText.text = "You can control the tactical view to track specific objects. Try left clicking the space station.";
            while(!Track) yield return null;
            ResetVars();
            subtitlesText.text = "You can also manually move the view. Try holding down the middle mouse button and dragging.";
            while(!Move) yield return null;
            ResetVars();
            subtitlesText.text = "You can rotate the camera by holding the right mouse button and dragging the mouse. Try doing this now.";
            while(!Rotate) yield return null;
            ResetVars();
            subtitlesText.text = "Finally try zooming in and out using the scroll wheel.";
            while(!Zoom) yield return null;
            
            subtitlesText.text = "Your teammates are going to have to mine asteroids and return those resources back to the station. You'll all be able to escape once they reach the necessary levels of Asteral. But in this sector, they will be met by pirates at every turn. There are two types - the grunts are more common and easier to take down. The elites are less common and harder to take down.";
            yield return new WaitForSeconds(10f);

            GridCoord gridCentre = GridCoord.GetCoordFromVector(GridManager.GetGridCentre());
            GameObject pirate = _pirateSpawner.SpawnPirate(PirateData.PirateType.Scout, new GridCoord(Random.Range(gridCentre.GetX() - 2, gridCentre.GetX() + 2), Random.Range(gridCentre.GetZ() - 2, gridCentre.GetZ() + 2)));
            subtitlesText.text = "Looks like there's one coming to attack the station and no-one's nearby! Ping it.";
            while(!WaitForPingCommand(PingType.Pirate, GridCoord.NullCoord)) yield return null;
            Destroy(pirate);

            LatestCommand = null;
            
            GameObject pirate2 = _pirateSpawner.SpawnPirate(PirateData.PirateType.Elite, new GridCoord(Random.Range(gridCentre.GetX() - 2, gridCentre.GetX() + 2), Random.Range(gridCentre.GetZ() - 2, gridCentre.GetZ() + 2)));
            subtitlesText.text = "That was a grunt. Oh no, here comes an elite! Ping it.";
            while(!WaitForPingCommand(PingType.Pirate, GridCoord.NullCoord)) yield return null;
            Destroy(pirate2);
            
            subtitlesText.text = "That'll do. Shooting down pirates before they reach and discover the station is ideal. Otherwise they'll report its location and you'll be dealing with an onslaught of them.";
            yield return new WaitForSeconds(6f);
            subtitlesText.text = "In the real attempt, you're most likely going to be giving your team ideas on how to tackle certain scenarios. Exactly who should go out and mine, and who should stay back and defend? Is someone carrying enough resources? Is the current plan working? These are all things beyond the scope of this tutorial, but will need to be considered.";
            yield return new WaitForSeconds(7f);

            int hyperdriveHealthRemaining = _spaceStation.GetHyperdrive().MaxHealth - _spaceStation.GetHyperdrive().ModuleHealth;
            _spaceStation.AddResources(hyperdriveHealthRemaining / 2);
            subtitlesText.text = "It looks like you have some resources stored in the station. Try to repair the hyperdrive.";
            while(!WaitForRepairCommand(RepairCommand.StationModule.Hyperdrive)) yield return null;
            
            subtitlesText.text = "Good, it looks like you didn't have enough resources to fully repair the hyperdrive. Your teammates need to deliver more resources so that you can fix it and activate it to escape.";
            yield return new WaitForSeconds(6f);
            subtitlesText.text = "Wait. There's... a large energy reading heading your team's way. If I had to guess... that's the pirate leader. Your team is definitely not going to be able to defend the station from THAT thing. It's looking like you only have about 5 minutes.";
            yield return new WaitForSeconds(7f);
            subtitlesText.text = "Is everything clear? Your team needs to deliver 1500 grams of Asteral to the station. Ensure that the pirates don't destroy it. You only have 5 minutes.";
            yield return new WaitForSeconds(5f);
            subtitlesText.text = "Acknowledged. Commence operation.";
            yield return new WaitForSeconds(2f);

            _tutorialComplete = true;
        }

        private IEnumerator MinerTutorial() {
            blackScreenPanel.SetActive(false);
            _cameraManager.SetMode(true);
            
            subtitlesText.text = "This is your ship. Attempt to interface with it through voice commands. Tell it to move forwards.";
            _audioSource.clip = minerAudioLines[0];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            while(!WaitForMovement(Strings.Forward, null, false)) yield return null;
            
            subtitlesText.text = "Great! Now tell it to stop.";
            _audioSource.clip = minerAudioLines[1];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            while(!WaitForStopCommand()) yield return null;
            
            subtitlesText.text = "And now backwards?";
            _audioSource.clip = minerAudioLines[2];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            while(!WaitForMovement(Strings.Back, null, false)) yield return null;
            
            subtitlesText.text = "Okay, perfect. And now stop.";
            _audioSource.clip = minerAudioLines[3];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            while(!WaitForStopCommand()) yield return null;
            
            subtitlesText.text = "Excellent, that's all functional. You can make it turn left or right. Try that.";
            _audioSource.clip = minerAudioLines[4];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            while(!WaitForMovement(Strings.Right, Strings.Left, true)) yield return null;
            
            subtitlesText.text = "And stop?";
            _audioSource.clip = minerAudioLines[5];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            while(!WaitForStopCommand()) yield return null;
            
            subtitlesText.text = "There we go. You can also directly move left or right as well. See for yourself.";
            _audioSource.clip = minerAudioLines[6];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            while(!WaitForMovement(Strings.Right, Strings.Left, false)) yield return null;
            
            subtitlesText.text = "Great, that's enough.";
            _audioSource.clip = minerAudioLines[7];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            
            subtitlesText.text = "We'll be able to track where you are. As a result, you can make use of this coordinate system. Try to move to C3.";
            _audioSource.clip = minerAudioLines[8];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            GridCoord c3 = new GridCoord('c', 3);
            while (!WaitForMovementGrid()) yield return null;
            while (!GridCoord.GetCoordFromVector(_moveObject.transform.position).Equals(c3)) yield return null;
            
            subtitlesText.text = "Your station commander, who's also keeping an eye out, may mark a point of interest (a 'ping'). Try moving to it.";
            _audioSource.clip = minerAudioLines[9];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            GridCoord pingLocation = new GridCoord(_asteroidLocation.GetX() - 1, _asteroidLocation.GetZ());
            Ping testPing = new Ping(pingLocation, PingType.Asteroid);
            _pingManager.AddPing(testPing);
            while (!WaitForMovementDestination(MovementCommand.DestinationType.Ping)) {
                if (_pingManager.GetPings().Count == 0) _pingManager.AddPing(testPing);
                yield return null;
            }
            while (!GridCoord.GetCoordFromVector(_moveObject.transform.position).Equals(pingLocation)) yield return null;

            subtitlesText.text = "That covers movement. Note that there's an asteroid here. Try mining it. Your ship should automatically detect and face it.";
            _audioSource.clip = minerAudioLines[10];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            while (!WaitForToggleCommand(ToggleCommand.ObjectType.MiningLaser)) yield return null;
            
            subtitlesText.text = "This is the lock-on system. You can also engage or disengage it manually, but your ship should be able to handle it automatically with the right commands.";
            _audioSource.clip = minerAudioLines[11];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            subtitlesText.text = "You may have noticed that this asteroid was not visible before. As you broke down in multidimensional travel, you can't actually see your surroundings. Your ship's emergency proximity sensor is handling everything, indicated by this blue circle. Pirates and asteroids will only be visible inside that.";
            _audioSource.clip = minerAudioLines[12];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            subtitlesText.text = "Your station commander can see everything fine, however. They have highly specialised and unwieldy equipment allowing them to do so.";
            _audioSource.clip = minerAudioLines[13];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            subtitlesText.text = "Anyway, your resource count should have gone up now. You'll need to deliver this back to the station. You can simply tell your ship to go back to it.";
            _audioSource.clip = minerAudioLines[14];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            while (!WaitForMovementDestination(MovementCommand.DestinationType.SpaceStation)) yield return null;
            while (!_moveObject.NearStation()) yield return null;
            
            subtitlesText.text = "Tell it to transfer the resources.";
            _audioSource.clip = minerAudioLines[15];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            while (!WaitForTransferCommand()) yield return null;

            GridCoord playerLocation = GridCoord.GetCoordFromVector(_moveObject.transform.position);
            GameObject pirate = _pirateSpawner.SpawnPirate(PirateData.PirateType.Scout, new GridCoord(playerLocation.GetX() + 1, playerLocation.GetZ()));
            subtitlesText.text = "And that's that. Now unfortunately, it's not going to be this simple. Remember when I said you're stranded in a problem sector? Well, here comes a pirate. Command your ship to shoot it.";
            _audioSource.clip = minerAudioLines[16];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            while (pirate != null) yield return null;
            
            playerLocation = GridCoord.GetCoordFromVector(_moveObject.transform.position);
            pirate = _pirateSpawner.SpawnPirate(PirateData.PirateType.Elite, new GridCoord(playerLocation.GetX() + 1, playerLocation.GetZ()));
            subtitlesText.text = "That was one of the grunts. Here comes a more powerful one.";
            _audioSource.clip = minerAudioLines[17];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            while (pirate != null) yield return null;

            subtitlesText.text = "And that was an elite. Our data says that only these two types are currently in that area. You're going to have to protect the station from them. If they find it, they'll most likely swarm it. Failure is not an option.";
            _audioSource.clip = minerAudioLines[18];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            subtitlesText.text = "W-wait. There's... a large energy reading heading your way. If I had to guess... that's their leader. You definitely aren't going to be able to defend the station from THAT thing. It's looking like you only have about 5 minutes.";
            _audioSource.clip = minerAudioLines[19];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            subtitlesText.text = "Is everything clear? Deliver 1500 grams of Asteral to the station as a team. Ensure that the pirates don't destroy it. You only have 5 minutes.";
            _audioSource.clip = minerAudioLines[20];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);
            subtitlesText.text = "Acknowledged. Commence operation.";
            _audioSource.clip = minerAudioLines[21];
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length);

            _tutorialComplete = true;
        }

        private void ResetVars() {
            Track = false;
            Move = false;
            Rotate = false;
            Zoom = false;
        }

        private bool WaitForYesOrNo() {
            if (LatestCommand == null) return false;
            
            if (LatestCommand.GetCommandType() == Command.CommandType.Yes) {
                _commander = true;
                return true;
            }
            
            if (LatestCommand.GetCommandType() == Command.CommandType.No) {
                _commander = false;
                return true;
            }

            return false;
        }

        private bool WaitForRepairCommand(RepairCommand.StationModule module) {
            if (LatestCommand == null) return false;
            if (LatestCommand.GetCommandType() != Command.CommandType.Repair) return false;
            RepairCommand command = (RepairCommand) LatestCommand;

            if (command.stationModule.Equals(module)) return true;
            return false;
        }

        private bool WaitForPingCommand(PingType type, GridCoord gridCoord) {
            if (LatestCommand == null) return false;
            if (LatestCommand.GetCommandType() != Command.CommandType.Ping) return false;
            PingCommand command = (PingCommand) LatestCommand;

            if (command.pingType != type) return false;
            if (gridCoord.Equals(GridCoord.NullCoord)) return true;
            if (gridCoord.Equals(command.gridCoord)) return true;
            return false;
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