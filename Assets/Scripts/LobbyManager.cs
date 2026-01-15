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


    IEnumerator Start()
    {
        Debug.Log("[LobbyManager] Start | 현재 연결 상태 : " + PhotonNetwork.NetworkClientState);

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
        //PhotonNetwork.JoinRoom(gameObject.GetComponent<RoolSlot>);
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
        Debug.Log("[LobbyManager] 로비 입장");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("[LobbyManager] 방에 입장하여 룸 씬으로 전환 요청");
        SceneManager.LoadScene(2);
    }

    /// <summary>
    /// 방 리스트 업데이트 콜백 함수
    /// todo : MVP로 만들어 UI 업데이트 부분 분리하기
    /// </summary>
    /// <param name="roomList">현재 생성된 방 목록</param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (GameObject child in _listViewport) Destroy(child);

        Debug.Log("[LobbyManager] 방 목록");
        foreach (RoomInfo roomInfo in roomList) 
        {
            if(roomInfo.RemovedFromList == true)
            {
                //Destroy()
            }
            var roomSlot = Instantiate(_roomSlotPrefab, _listViewport);
            // 직접 배열로 접근해도 되는건가? > to-do : MVP로 만들어 룸 슬롯 모델/뷰로 리팩토링하기 
            TMP_Text[] texts = roomSlot.GetComponentsInChildren<TMP_Text>();
            
            if(texts.Length == 2)
            {
                texts[0].text = roomInfo.Name;
                texts[1].text = roomInfo.PlayerCount + " / " + roomInfo.MaxPlayers;
            }
            
            // 내가 만든 방에서 나왔을 때 반영이 안되는 버그
            // 슬롯이 업데이트가 안되는 건지는 모르겠다

            // 방에 참가할 수 없을 경우 disable화
            if(roomInfo.IsOpen == false)
            {
                roomSlot.gameObject.GetComponent<Button>().interactable = false;
            }
        }
    }


}
