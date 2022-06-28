using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;

    [SerializeField] private TMP_InputField _roomInputField;
    [SerializeField] private TMP_Text _errorText;
    [SerializeField] private TMP_Text _roomNameText;
    [SerializeField] private Transform _roomList;
    [SerializeField] private GameObject _roomButtonPrefab;
    [SerializeField] private Transform _playerList;
    [SerializeField] private GameObject _playerNamePrefab;
    [SerializeField] private GameObject _startGameButton;
    void Start()
    {
        instance = this;
        Debug.Log("Connecting to master server");
        PhotonNetwork.ConnectUsingSettings();
        MenuManager.instance.OpenMenu("Loading");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master server");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Connected to lobby");
        MenuManager.instance.OpenMenu("Title");
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString();
    }
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(_roomInputField.text))
            return;

        PhotonNetwork.CreateRoom(_roomInputField.text);
        MenuManager.instance.OpenMenu("Loading");
    }

    public override void OnJoinedRoom()
    {
        _roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        MenuManager.instance.OpenMenu("Room");

        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < _playerList.childCount; i++)
        {
            Destroy(_playerList.GetChild(i).gameObject);
        }

        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(_playerNamePrefab, _playerList).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        _startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        _startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        _errorText.text = "Error: " + message;
        MenuManager.instance.OpenMenu("Error");
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.instance.OpenMenu("Loading");
    }
    public override void OnLeftRoom()
    {
        MenuManager.instance.OpenMenu("Title");
    }

    public void JoinRoom(RoomInfo roomInfo)
    {
        PhotonNetwork.JoinRoom(roomInfo.Name);
        MenuManager.instance.OpenMenu("Loading");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < _roomList.childCount; i++)
        {
            Destroy(_roomList.GetChild(i).gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(_roomButtonPrefab, _roomList).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(_playerNamePrefab, _playerList).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
