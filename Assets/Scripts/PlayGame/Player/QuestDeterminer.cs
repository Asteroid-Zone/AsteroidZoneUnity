using System.Collections;
using PlayGame.Player.Movement;
using Statics;
using UnityEngine;
using UnityEngine.Events;

namespace PlayGame.Player {
    
    /// <summary>
    /// This class controls the players current quest.
    /// </summary>
    public class QuestDeterminer : MonoBehaviour {

        private PlayerData _playerData;
        private MoveObject _moveObject;
        private SpaceStation.SpaceStation _spaceStation;
        private bool _stationDamaged;
        private UnityEvent _stationAttacked;

        /// <summary>
        /// Fetches components used for determining the players quest.
        /// <para>Starts a coroutine to determine the players quest.</para>
        /// </summary>
        private void Start() {
            _playerData = GetComponent<PlayerData>();
            _spaceStation = GameObject.FindGameObjectWithTag(Tags.StationTag).GetComponent<SpaceStation.SpaceStation>();
            
            if (_playerData.GetRole() != Role.StationCommander) {
                _moveObject = GetComponent<MoveObject>();
                _stationDamaged = false;
                SpaceStation.SpaceStation spaceStation = GameObject.FindGameObjectWithTag(Tags.StationTag).GetComponent<SpaceStation.SpaceStation>();
                _stationAttacked = spaceStation.stationAttacked;
                _stationAttacked.AddListener(OnStationDamaged);

                StartCoroutine(DetermineQuestMiner());
            } else StartCoroutine(DetermineQuestCommander());
        }
    
        /// <summary>
        /// Method is called by a UnityEvent when the station is attacked.
        /// <para>Sets _stationDamaged to true.</para>
        /// </summary>
        public void OnStationDamaged() {
            _stationDamaged = true;
        }
    
        /// <summary>
        /// This method runs in a coroutine to determine a miners current quest.
        /// </summary>
        private IEnumerator DetermineQuestMiner() {
            while (true) {
                Transform nearestEnemy = _moveObject.GetNearestEnemyTransform();
                Transform nearestAsteroid = _moveObject.GetNearestAsteroidTransform();
                if (_playerData.dead) {
                    _playerData.SetQuest(QuestType.Respawn);
                } else if (_stationDamaged) {
                    // If within laser range of station hint should be shoot pirates otherwise return to station
                    if (_moveObject.DistanceToStation() > _playerData.GetLaserRange()) _playerData.SetQuest(QuestType.ReturnToStationDefend);
                    else _playerData.SetQuest(QuestType.DefendStation);
                    
                    yield return new WaitForSeconds(4);
                    _stationDamaged = false;
                } else if (nearestEnemy != null && Vector3.Distance(nearestEnemy.position, transform.position) < _playerData.GetLookRadius()) {
                    _playerData.SetQuest(QuestType.PirateWarning);
                } else if (_playerData.GetResources() > 75) {
                    // Choose the hint depending on if player is at station or not
                    if (_moveObject.NearStation()) _playerData.SetQuest(QuestType.TransferResources);
                    else _playerData.SetQuest(QuestType.ReturnToStationResources);
                } else if (nearestAsteroid != null && Vector3.Distance(nearestAsteroid.position, transform.position) < _playerData.GetLookRadius()) {
                    _playerData.SetQuest(QuestType.MineAsteroids);
                } else {
                    _playerData.SetQuest(QuestType.FindAsteroids);
                }

                yield return new WaitForSeconds(0.25f);
            }
        }

        /// <summary>
        /// This method runs in a coroutine to determine the station commanders current quest.
        /// </summary>
        private IEnumerator DetermineQuestCommander() {
            while (true) {
                if (_spaceStation.GetHyperdrive().IsFunctional()) _playerData.SetQuest(QuestType.ActivateHyperdrive);
                else if (_spaceStation.resources > 0) _playerData.SetQuest(QuestType.RepairStation);
                else _playerData.SetQuest(QuestType.HelpPlayers);

                yield return new WaitForSeconds(0.25f);
            }
        }
    }
}
