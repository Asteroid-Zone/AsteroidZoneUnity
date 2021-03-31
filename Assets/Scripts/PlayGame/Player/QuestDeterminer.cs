using System.Collections;
using System.Collections.Generic;
using Photon.GameControllers;
using PlayGame.Player;
using PlayGame.SpaceStation;
using Statics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuestDeterminer : MonoBehaviour
{

    private PlayerData _playerData;
    private bool _stationDamaged;
    private UnityEvent _stationAttacked;

    // Start is called before the first frame update
    void Start()
    {
        _playerData = GetComponent<PlayerData>();
        _stationDamaged = false;
        // Can't find a better way of doing this
        SpaceStation spaceStation = GameObject.Find("SpaceStation").GetComponent<SpaceStation>();
        _stationAttacked = spaceStation.stationAttacked;
        _stationAttacked.AddListener(OnStationDamaged);
        StartCoroutine(DetermineQuest());
    }
    
    public void OnStationDamaged()
    {
        _stationDamaged = true;
    }
    

    private IEnumerator DetermineQuest()
    {
        while (true)
        {
            if (_stationDamaged)
            {
                _playerData.SetQuest(QuestType.DefendStation);
                yield return new WaitForSeconds(4);
                _stationDamaged = false;
                // TODO: Add flag for station commander saying pirates around 
            }

            else
            {
                if (_playerData.GetResources() > 75)
                {
                    _playerData.SetQuest(QuestType.ReturnToStation);
                }
                else
                {
                    _playerData.SetQuest(QuestType.MineAsteroids);
                }
            }

            yield return null;
        }

    }
}
