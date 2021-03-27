using System.Collections;
using System.Collections.Generic;
using Photon.GameControllers;
using PlayGame.Player;
using Statics;
using UnityEngine;
using UnityEngine.UI;

public class QuestDeterminer : MonoBehaviour
{

    private PlayerData _playerData;
    private bool stationDamaged;

    // Start is called before the first frame update
    void Start()
    {
        _playerData = GetComponent<PlayerData>();
        stationDamaged = false;

    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(calculateQuest());
    }
    
    // If station is damaged, this overrides the current event
    public void stationDamagedToggle()
    {
        Debug.Log("Station damaged!");
        stationDamaged = true;
    }
    private IEnumerator calculateQuest()
    {
        if (stationDamaged)
        {
            _playerData.SetQuest(QuestType.DefendStation);
            yield return new WaitForSeconds(4);
            stationDamaged = false;
        }
        // TODO: Add flag for station commander saying pirates around 
            
        if (_playerData.GetResources() > 75)
        {
            _playerData.SetQuest(QuestType.ReturnToStation);
        }
        else
        {
            _playerData.SetQuest(QuestType.MineAsteroids);
        }
            
    }
}
