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
        
        // Make the class a singleton
        #region Singleton
        private static TutorialManager _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        public static TutorialManager GetInstance()
        {
            return _instance;
        }
        #endregion

        public Text inputText;
        public Text subtitlesText;
        
        public List<AudioClip> introAudioLines;
        public List<string> introTextLines;
        
        public List<AudioClip> commanderAudioLines;
        public List<string> commanderTextLines;
        
        public List<AudioClip> minerAudioLines;
        public List<string> minerTextLines;
        
        public List<AudioClip> outroAudioLines;
        public List<string> outroTextLines;

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
            _asteroidSpawner.SpawnAsteroid(new GridCoord('e', 6));
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
            
            yield return PlayLine(commanderTextLines[0],commanderAudioLines[0]);
            while(!WaitForPingCommand(PingType.Asteroid, new GridCoord('e', 6))) yield return null;
            
            yield return PlayLine(commanderTextLines[1],commanderAudioLines[1]);
            yield return PlayLine(commanderTextLines[2],commanderAudioLines[2]);

            ResetVars();
            yield return PlayLine(commanderTextLines[3],commanderAudioLines[3]);
            while(!Track) yield return null;
            ResetVars();
            yield return PlayLine(commanderTextLines[4],commanderAudioLines[4]);
            while(!Move) yield return null;
            ResetVars();
            yield return PlayLine(commanderTextLines[5],commanderAudioLines[5]);
            while(!Rotate) yield return null;
            ResetVars();
            yield return PlayLine(commanderTextLines[6],commanderAudioLines[6]);
            while(!Zoom) yield return null;
            
            yield return PlayLine(commanderTextLines[7],commanderAudioLines[7]);

            GridCoord gridCentre = GridCoord.GetCoordFromVector(GridManager.GetGridCentre());
            GameObject pirate = _pirateSpawner.SpawnPirate(PirateData.PirateType.Scout, new GridCoord(Random.Range(gridCentre.GetX() - 2, gridCentre.GetX() + 2), Random.Range(gridCentre.GetZ() - 2, gridCentre.GetZ() + 2)));
            yield return PlayLine(commanderTextLines[8],commanderAudioLines[8]);
            while(!WaitForPingCommand(PingType.Pirate, GridCoord.NullCoord)) yield return null;
            
            LatestCommand = null;
            yield return new WaitForSeconds(3f);
            Destroy(pirate);
            
            GameObject pirate2 = _pirateSpawner.SpawnPirate(PirateData.PirateType.Elite, new GridCoord(Random.Range(gridCentre.GetX() - 2, gridCentre.GetX() + 2), Random.Range(gridCentre.GetZ() - 2, gridCentre.GetZ() + 2)));
            yield return PlayLine(commanderTextLines[9],commanderAudioLines[9]);
            while(!WaitForPingCommand(PingType.Pirate, GridCoord.NullCoord)) yield return null;
            Destroy(pirate2);
            
            yield return PlayLine(commanderTextLines[10],commanderAudioLines[10]);
            yield return PlayLine(commanderTextLines[11],commanderAudioLines[11]);

            int hyperdriveHealthRemaining = _spaceStation.GetHyperdrive().MaxHealth - _spaceStation.GetHyperdrive().ModuleHealth;
            _spaceStation.AddResources(hyperdriveHealthRemaining / 2);
            yield return PlayLine(commanderTextLines[12],commanderAudioLines[12]);
            while(!WaitForRepairCommand(RepairCommand.StationModule.Hyperdrive)) yield return null;
            
            yield return PlayLine(commanderTextLines[13],commanderAudioLines[13]);
            StartCoroutine(TutorialOutro());
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
            _pingManager.AddPing(new Ping(pingLocation, PingType.Asteroid));
            while (!WaitForMovementDestination(MovementCommand.DestinationType.Ping)) {
                if (_pingManager.GetPings().Count == 0) _pingManager.AddPing(new Ping(pingLocation, PingType.Asteroid));
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
            StartCoroutine(TutorialOutro());
        }
        
        private IEnumerator TutorialOutro() {
            yield return PlayLine(outroTextLines[0], outroAudioLines[0]);
            yield return PlayLine(outroTextLines[1], outroAudioLines[1]);
            yield return PlayLine(outroTextLines[2], outroAudioLines[2]);

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

            if (command.pingType != type && command.pingType != PingType.Generic) return false;
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