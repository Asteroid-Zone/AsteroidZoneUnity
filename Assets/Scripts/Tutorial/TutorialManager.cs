using System.Collections;
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
                yield return PlayLine(introTextLines[i], introAudioLines[i]);
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

            yield return PlayLine(minerTextLines[0], minerAudioLines[0]);
            while(!WaitForMovement(Strings.Forward, null, false)) yield return null;
            
            yield return PlayLine(minerTextLines[1], minerAudioLines[1]);
            while(!WaitForStopCommand()) yield return null;
            
            yield return PlayLine(minerTextLines[2], minerAudioLines[2]);
            while(!WaitForMovement(Strings.Back, null, false)) yield return null;
            
            yield return PlayLine(minerTextLines[3], minerAudioLines[3]);
            while(!WaitForStopCommand()) yield return null;
            
            yield return PlayLine(minerTextLines[4], minerAudioLines[4]);
            while(!WaitForMovement(Strings.Right, Strings.Left, true)) yield return null;
            
            yield return PlayLine(minerTextLines[5], minerAudioLines[5]);
            while(!WaitForStopCommand()) yield return null;
            
            yield return PlayLine(minerTextLines[6], minerAudioLines[6]);
            while(!WaitForMovement(Strings.Right, Strings.Left, false)) yield return null;
            
            yield return PlayLine(minerTextLines[7], minerAudioLines[7]);
            
            yield return PlayLine(minerTextLines[8], minerAudioLines[8]);
            GridCoord c3 = new GridCoord('c', 3);
            while (!WaitForMovementGrid()) yield return null;
            while (!GridCoord.GetCoordFromVector(_moveObject.transform.position).Equals(c3)) yield return null;
            
            yield return PlayLine(minerTextLines[9], minerAudioLines[9]);
            GridCoord pingLocation = new GridCoord(_asteroidLocation.GetX() - 1, _asteroidLocation.GetZ());
            Ping testPing = new Ping(pingLocation, PingType.Asteroid);
            _pingManager.AddPing(testPing);
            while (!WaitForMovementDestination(MovementCommand.DestinationType.Ping)) {
                if (_pingManager.GetPings().Count == 0) _pingManager.AddPing(testPing);
                yield return null;
            }
            while (!GridCoord.GetCoordFromVector(_moveObject.transform.position).Equals(pingLocation)) yield return null;

            yield return PlayLine(minerTextLines[10], minerAudioLines[10]);
            while (!WaitForToggleCommand(ToggleCommand.ObjectType.MiningLaser)) yield return null;
            
            yield return PlayLine(minerTextLines[11], minerAudioLines[11]);
            yield return PlayLine(minerTextLines[12], minerAudioLines[12]);
            yield return PlayLine(minerTextLines[13], minerAudioLines[13]);
            yield return PlayLine(minerTextLines[14], minerAudioLines[14]);
            while (!WaitForMovementDestination(MovementCommand.DestinationType.SpaceStation)) yield return null;
            while (!_moveObject.NearStation()) yield return null;
            
            yield return PlayLine(minerTextLines[15], minerAudioLines[15]);
            while (!WaitForTransferCommand()) yield return null;

            GridCoord playerLocation = GridCoord.GetCoordFromVector(_moveObject.transform.position);
            GameObject pirate = _pirateSpawner.SpawnPirate(PirateData.PirateType.Scout, new GridCoord(playerLocation.GetX() + 1, playerLocation.GetZ()));
            yield return PlayLine(minerTextLines[16], minerAudioLines[16]);
            while (pirate != null) yield return null;
            
            playerLocation = GridCoord.GetCoordFromVector(_moveObject.transform.position);
            pirate = _pirateSpawner.SpawnPirate(PirateData.PirateType.Elite, new GridCoord(playerLocation.GetX() + 1, playerLocation.GetZ()));
            yield return PlayLine(minerTextLines[17], minerAudioLines[17]);
            while (pirate != null) yield return null;

            yield return PlayLine(minerTextLines[18], minerAudioLines[18]);
            yield return PlayLine(minerTextLines[19], minerAudioLines[19]);
            yield return PlayLine(minerTextLines[20], minerAudioLines[20]);
            yield return PlayLine(minerTextLines[21], minerAudioLines[21]);

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

        private WaitForSeconds PlayLine(string subtitleLine, AudioClip audioLine) {
            subtitlesText.text = subtitleLine;
            _audioSource.clip = audioLine;
            _audioSource.Play();
            return new WaitForSeconds(_audioSource.clip.length);
        }
    }
}