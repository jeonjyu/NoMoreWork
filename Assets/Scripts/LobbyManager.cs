using Firebase.Auth;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField _roomInput;
    [SerializeField] GameObject _roomSlotPrefab;
    [SerializeField] Transform _listViewport;

    Dictionary<string, RoomSlot> rooms;
    List<RoomSlot> slots;

    void Awake()
    {
        rooms = new Dictionary<string, RoomSlot>();
        slots = new List<RoomSlot>();
    }

    IEnumerator Start()
    {
        //Debug.Log("[LobbyManager] Start | 현재 연결 상태 : " + PhotonNetwork.NetworkClientState);

        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);

        if (PhotonNetwork.NetworkClientState == ClientState.Disconnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }
        //Init();
    }

    void Init()
    {
        _listViewport = GetComponent<Transform>();
    }

    RoomOptions roomOptions = new RoomOptions { MaxPlayers = 4, PublishUserId = true };

    public void CreateRoom()
    {
        Debug.Log(_roomInput.text);
        // 생성 후 입장
        PhotonNetwork.CreateRoom(_roomInput.text, roomOptions);
    }

    public void JoinRoom()
    {
        // 클릭한 룸에 들어감
        //PhotonNetwork.JoinRoom(gameObject.GetComponent<RoomSlot>);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public void ExitLobby()
    {
        PhotonNetwork.LeaveLobby();
        SceneManager.LoadScene(0);
    }

    public override void OnJoinedLobby()
    {
        //Debug.Log("[LobbyManager] 로비 입장");
    }

    public override void OnJoinedRoom()
    {
        //Debug.Log("[LobbyManager] 방에 입장하여 룸 씬으로 전환 요청");
        SceneManager.LoadScene(2);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            if (room.RemovedFromList)
            {
                // 널참조 오류 버그
                rooms.TryGetValue(room.Name, out RoomSlot roomSlot);
                Destroy(slots.Find(x => x.name.Contains(room.Name)).gameObject);
                rooms.Remove(room.Name);
            }
            else if (rooms.TryGetValue(room.Name, out RoomSlot roomSlot) && roomSlot.name == room.Name) 
            {
                GameObject roomObj = slots.Find(x => x.name.Contains(room.Name)).gameObject;
                roomSlot.currentPlayerCount = room.PlayerCount;

                // 방에 참가할 수 없을 경우 disable화
                if (!room.IsOpen)
                {
                    roomSlot.gameObject.GetComponent<Button>().interactable = false;
                } 
                else if (room.IsOpen)
                {
                    roomSlot.gameObject.GetComponent<Button>().interactable = true;
                }

            }
            else
            {
                Debug.Log($"[LobbyManager] {room.Name} {room.masterClientId}");
                CreateRoomSlot(room);
            }
        }
    }

    private void CreateRoomSlot(RoomInfo roomInfo)
    {
        Debug.Log("[LobbyManager] 방 " + roomInfo.Name + " " + roomInfo.PlayerCount + " " + roomInfo.masterClientId);
        GameObject roomSlot = Instantiate(_roomSlotPrefab, _listViewport);
        RoomSlot slot = roomSlot.GetComponent<RoomSlot>();
        
        slot.name = roomInfo.Name;
        slot.currentPlayerCount = roomInfo.PlayerCount;
        slot.masterClientId = roomInfo.masterClientId;

        slots.Add(slot);
        rooms.Add(roomInfo.Name, slot);

        TMP_Text[] texts = roomSlot.GetComponentsInChildren<TMP_Text>();

        if (texts.Length > 0)
        {
            texts[0].text = roomInfo.Name;
            //texts[1].text = roomInfo.PlayerCount + " / " + roomInfo.MaxPlayers;
        }

        //버튼에 이벤트 리스너 추가
        Button btn = roomSlot.GetComponentInChildren<Button>();
        btn.onClick.AddListener(() => PhotonNetwork.JoinRoom(roomInfo.Name));

        // 방에 참가할 수 없을 경우 disable화
        if (roomInfo.IsOpen == false)
        {
            roomSlot.gameObject.GetComponent<Button>().interactable = false;
        }
    }

    public void SignOut()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        auth.SignOut();

        if(FirebaseAuth.DefaultInstance.CurrentUser == null)
            Debug.Log($"[LobbyManager] 로그아웃 성공");
        else 
            Debug.Log($"[LobbyManager] 로그아웃 실패");
    }
}
