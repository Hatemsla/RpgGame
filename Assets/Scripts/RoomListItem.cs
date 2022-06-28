using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _roomName;

    public RoomInfo info;
    public void SetUp(RoomInfo roomInfo)
    {
        info = roomInfo;
        _roomName.text = info.Name;
    }

    public void OnClick()
    {
        Launcher.instance.JoinRoom(info);
    }
}
