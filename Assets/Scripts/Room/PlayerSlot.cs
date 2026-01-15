using UnityEngine;
using Photon.Realtime;
using HashTable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using System;

// 플레이어 화면 관리
//
// 하는 일
// 플레이어 슬롯 생성
// 슬롯 업데이트
// 플레이어 준비, 방에서 나가기

// model
public class PlayerSlotModel 
{
    UserInfo userInfo;
    // field
    private string _userName;
    
    // property
    public string UserName => _userName;

    // 커스텀 프로퍼티로 플레이어 준비 상태 같이 전송
    HashTable playerSlotProps = new HashTable() { { "IsReady", false } };
    public bool IsReady
    {
        get => (bool)playerSlotProps["IsReady"];
        set => playerSlotProps["IsReady"] = value;
    }

    public event Action PlayerSlotChanged;


    public PlayerSlotModel(Player player)
    {
        _userName = player.NickName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSlotProps);
    }
}
