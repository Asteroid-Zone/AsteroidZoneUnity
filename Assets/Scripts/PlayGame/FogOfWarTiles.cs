using System;
using System.Collections;
using System.Collections.Generic;
using Photon.GameControllers;
using PlayGame;
using PlayGame.Player;
using Statics;
using UnityEngine;

public class FogOfWarTiles : MonoBehaviour
{
    /* Fog of War works by configuring the 4 different FollowCameras with different culling masks, and "covering up"
     each tile with 4 different "cover" objects on different layers. When a tile is exposed, the corresponding "cover" 
     is disabled and a certain camera is able to see it (the other 3 "covers" remain enabled so the others can't see it).
     
     This whole section can probably be optimised.
     */
    
    public int FogOfWarRadius;
    private GameObject _player;
    private GridManager _gridManager;
    private GameObject[,] _grid;
    private int _playerID;
    private int _width;
    private int _height;

    private List<Vector2> _visibleTiles;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = !DebugSettings.Debug ? PhotonPlayer.Instance.myAvatar : TestPlayer.GetPlayerShip();
        _gridManager = GetComponent<GridManager>();
        _width = _gridManager.GetWidth();
        _height = _gridManager.GetHeight();
        _grid = _gridManager.GetGrid();
        Camera _camera = GameObject.Find("Follow Camera").GetComponent<Camera>();
        _playerID = _player.GetComponent<PlayerData>().GetPlayerID();
        
        
        int FoW1Layer = 9;
        
        for (int i = 0; i < 4; i++)
        {
            if (i == _playerID)
            {
                _camera.cullingMask = _camera.cullingMask | (1 << (i + FoW1Layer));
            }
            else
            {
                _camera.cullingMask = _camera.cullingMask & ~(1 << (i + FoW1Layer));
            }
        }
        // TODO: Do by role accordingly

    }

    // Update is called once per frame
    void Update()
    {
        // Toggle visibility for "cover" objects on tiles in defined radius, make tiles on the border black again
        // Actually fix this so each player calculates their own
        Vector2 position = _gridManager.GlobalToGridCoord(_player.transform.position);
        (List<Vector2>, List<Vector2>) tileLists = calcVisibleTiles(position);
        // Visible case
        foreach (Vector2 tile in tileLists.Item1)
        {
            // Get the ith cover
            _grid[(int) tile.y, (int) tile.x].transform.GetChild(0).GetChild(1).GetChild(_playerID).gameObject.SetActive(false);
        }
        // Invisible case
        foreach (Vector2 tile in tileLists.Item2)
        {
            // Get the ith cover
            _grid[(int) tile.y, (int) tile.x].transform.GetChild(0).GetChild(1).GetChild(_playerID).gameObject.SetActive(true);
        }
    }

    // Generates all the visible tiles but also an outline of invisible tiles
    // (I can explain the logic for this if needed)
    private (List<Vector2>, List<Vector2>) calcVisibleTiles(Vector2 inputTile)
    {
        int newRadius = FogOfWarRadius + 1;
        List<Vector2> visibleTilesList = new List<Vector2>();
        List<Vector2> invisibleTilesList = new List<Vector2>();
        int diameter = (newRadius * 2) + 1;
        for (int v = 0; v < diameter; v++)
        {
            int y = v - newRadius + (int) inputTile.y;
            
            if (y < 0 || y >= _height)
            {
                continue;
            }

            int verticalDifference = (int) Math.Abs(inputTile.y - y);
            for (int u = 0; u < diameter - 2*verticalDifference; u++)
            {
                int x = u + verticalDifference - newRadius + (int) inputTile.x;
                if (x < 0 || x >= _width)
                {
                    continue;
                }
                
                // If tile is first or last tile, it's invisible
                if (u == 0 || u == diameter - 2*verticalDifference - 1)
                {
                    invisibleTilesList.Add(new Vector2(x, y));
                }
                else
                {
                    visibleTilesList.Add(new Vector2(x, y));
                }
            }
        }

        return (visibleTilesList, invisibleTilesList);
    }

    public List<Vector2> GetVisibleTiles()
    {
        return _visibleTiles;
    }

}
