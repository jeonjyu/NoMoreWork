using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;


public class RoomManager : MonoBehaviourPunCallbacks
{
    Player[] players;
    [SerializeField] GameObject _playerSlotPrefab;
    [SerializeField] Transform _listPanel;
    [SerializeField] Button _readyBtn;
    [SerializeField] Button _startBtn;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => PhotonNetwork.InRoom);

        players = PhotonNetwork.PlayerList;

        foreach (var p in players) Debug.Log(p.UserId);

        if (PhotonNetwork.IsMasterClient == false)
        {
            // 방장이 아니면 할 일
            _readyBtn.gameObject?.SetActive(true);
            _startBtn.gameObject?.SetActive(false);
        }
        else 
        {
            _readyBtn.gameObject?.SetActive(false);
            _startBtn.gameObject?.SetActive(true);
        }

        UpdateUI();
        // 준비 완료 등 처리
    }

    public void StartGame()
    {
        if(PhotonNetwork.IsMasterClient == true)
        {
            Debug.Log("[RoomManger] 게임 시작");
            PhotonNetwork.LoadLevel(3);
        }
    }

    // 플레이어에 변동이 있으면 업데이트

    // 플레이어 목록 업데이트
    // 자기것만 presenter 주고 나머지는
    // ?? 나머지 업데이트 어떻게 하지?
    // 플레이어 이름 넣어주고 
    // 내것만 presenter 줌

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("[RoomManager]" + newPlayer.UserId + " 입장");

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
        Debug.Log("[RoomManager]" + otherPlayer.UserId + " 퇴장");
        // 플레이어 목록 업데이트
        UpdateUI();
    }

    // player에 있는 애들을 매번 Instantiate 안하고 불러오기할 수 없나?
    public void UpdateUI()
    {
        Debug.Log("[RoomManager] 리스트 갱신");

        if (_listPanel.childCount > 0)
        {
            PlayerSlotView[] slots = _listPanel.GetComponentsInChildren<PlayerSlotView>();
            foreach (PlayerSlotView slot in slots) Destroy(slot.gameObject);
        }

        //매번 Instantiate 하는 거랑 있는 presenter 만들어두고 그 애들을 불러오는 거 중에 뭐가 낫지
        foreach (Player player in PhotonNetwork.PlayerList) 
        {
            GameObject slot = Instantiate(_playerSlotPrefab, _listPanel);
            Debug.Log("[RoomManager] 리스트 " + player.UserId);
            slot.GetComponent<PlayerSlotView>().Init(player);
        }
    }

    public void ExitRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(1);
    }
}
