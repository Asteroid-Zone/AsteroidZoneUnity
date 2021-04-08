﻿using System.Collections;
using PlayGame.Player.Movement;
using UnityEngine;
using UnityEngine.Events;

namespace PlayGame.Player
{
    public class QuestDeterminer : MonoBehaviour {

        private PlayerData _playerData;
        private MoveObject _moveObject;
        private bool _stationDamaged;
        private UnityEvent _stationAttacked;

        // Start is called before the first frame update
        void Start() {
            _playerData = GetComponent<PlayerData>();
            if (_playerData.GetRole() != Role.StationCommander) {
                _moveObject = GetComponent<MoveObject>();
                _stationDamaged = false;
                // Can't find a better way of doing this
                SpaceStation.SpaceStation spaceStation =
                    GameObject.Find("SpaceStation").GetComponent<SpaceStation.SpaceStation>();
                _stationAttacked = spaceStation.stationAttacked;
                _stationAttacked.AddListener(OnStationDamaged);

                StartCoroutine(DetermineQuestMiner());
            } else StartCoroutine(DetermineQuestCommander());
        }
    
        public void OnStationDamaged()
        {
            _stationDamaged = true;
        }
    
        private IEnumerator DetermineQuestMiner() {
            while (true)
            {
                Transform nearestEnemy = _moveObject.GetNearestEnemyTransform();
                if (_stationDamaged) {
                    // If within laser range of station hint should be shoot pirates otherwise return to station
                    if (_moveObject.DistanceToStation() > _playerData.GetLaserRange()) _playerData.SetQuest(QuestType.ReturnToStationDefend);
                    else _playerData.SetQuest(QuestType.DefendStation);
                    
                    yield return new WaitForSeconds(4);
                    _stationDamaged = false;
                } else if (nearestEnemy != null && Vector3.Distance(nearestEnemy.position, transform.position) < 20) {
                    _playerData.SetQuest(QuestType.PirateWarning);
                } else if (_playerData.GetResources() > 75) {
                    // Choose the hint depending on if player is at station or not
                    if (_moveObject.NearStation()) _playerData.SetQuest(QuestType.TransferResources);
                    else _playerData.SetQuest(QuestType.ReturnToStationResources);
                } else {
                    _playerData.SetQuest(QuestType.MineAsteroids);
                }

                yield return new WaitForSeconds(0.25f);
            }
        }

        // todo station commander quests
        private IEnumerator DetermineQuestCommander() {
            while (true) {
                _playerData.SetQuest(QuestType.DefendStation);
                
                yield return new WaitForSeconds(0.25f);
            }
        }
    }
}
