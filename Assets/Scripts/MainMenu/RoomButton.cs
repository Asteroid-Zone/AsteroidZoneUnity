using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine;

public class RoomButton : MonoBehaviourPunCallbacks
{
    public Text nameText;

    public string roomName;

    public void SetRoom()
    {
      nameText.text = roomName;
    }

    public void JoinRoom()
    {
      PhotonNetwork.JoinRoom(roomName);
      Destroy(this.gameObject);
    }
}
