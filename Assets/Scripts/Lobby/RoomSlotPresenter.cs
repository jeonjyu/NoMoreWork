using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomSlotView : MonoBehaviour 
{
    [SerializeField] TMP_Text _roomName;
    [SerializeField] TMP_Text _playerCount;
    [SerializeField] Button button;

    public void SetRoomSlot(RoomInfo roomInfo)
    {
        _roomName.text = roomInfo.Name;
        _playerCount.text = roomInfo.PlayerCount + " / " + roomInfo.MaxPlayers;
    }

    public void UpdatePlayerCount(RoomInfo roomInfo)
    {
        _playerCount.text = roomInfo.PlayerCount + " / " + roomInfo.MaxPlayers;
    }
}

public class RoomSlotModel 
{
    public string roomName;
    public int playerCount;
    
    // 플레이어 수 업데이트는 어디서? 
}

// 생성자로 가져오고 
// 초기화로 view, model 초기 세팅
public class RoomSlotPresenter : MonoBehaviour
{
    private RoomSlotModel _model;
    private RoomSlotView _view;

    // 방 이름, count 설정
    public void SetRoomSlot(RoomInfo roomInfo)
    {
        _view.SetRoomSlot(roomInfo);
    }

    // 슬롯 버튼 클릭시 입장 가능 판별, 입장 가능 시 입장, 네트워크 처리 후 씬 전환?
    public void OnButtonClick()
    {

    }


    // 방이 삭제되는 경우 처리
    public void OnDestroy()
    {
        
    }
}
