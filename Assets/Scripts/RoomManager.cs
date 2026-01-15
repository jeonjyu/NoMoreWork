using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class RoomManager : MonoBehaviourPunCallbacks
{
    Player[] players;
    [SerializeField] GameObject _playerSlotPrefab;
    [SerializeField] Transform _listPanel;
    [SerializeField] Button _readyBtn;
    [SerializeField] Button _startBtn;

    void Start()
    {
        players = PhotonNetwork.PlayerList;

        foreach (var p in players) Debug.Log(p.NickName);

        if (PhotonNetwork.IsMasterClient == false)
        {
            // 방장이 아니면 할 일
            _readyBtn.gameObject.SetActive(true);
            _startBtn.gameObject.SetActive(false);
        }
        else 
        {
            _readyBtn.gameObject.SetActive(false);
            _startBtn.gameObject.SetActive(true);
        }

            UpdateUI();
        // 준비 완료 등 처리
    }

    // 플레이어에 변동이 있으면 업데이트

    // 플레이어 목록 업데이트
    // 자기것만 presenter 주고 나머지는
    // ?? 나머지 업데이트 어떻게 하지?
    // 플레이어 이름 넣어주고 
    // 내것만 presenter 줌

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("[RoomManager]" + newPlayer.NickName + " 입장");

        // 플레이어 목록 업데이트
        UpdateUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer == PhotonNetwork.MasterClient)
        {
            Debug.Log("[RoomManager] 방장이 퇴장하여 로비로 이동합니다");
            ExitRoom();
        }
        Debug.Log("[RoomManager]" + otherPlayer.NickName + " 퇴장");
        // 플레이어 목록 업데이트
        UpdateUI();
    }

    // player에 있는 애들을 매번 Instantiate 안하고 불러오기할 수 없나?
    public void UpdateUI()
    {
        foreach (GameObject child in _listPanel) Destroy(child);

        //매번 Instantiate 하는 거랑 있는 presenter 만들어두고 그 애들을 불러오는 거 중에 뭐가 낫지
        foreach (Player player in players) 
        {
            GameObject slot = Instantiate(_playerSlotPrefab, _listPanel);
            Debug.Log(player.NickName);
            slot.GetComponent<PlayerSlotView>().Init(player.NickName);
        }
    }

    public void ExitRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(1);
    }
}
