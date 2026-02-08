using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.SceneManagement;

/// <summary>
/// 플레이어 생성, 방에서 나가는 거 관리
/// 스테이지 관리, 저장
/// </summary>
public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    public GameObject ClearCanvas;
    [SerializeField] List<GameObject> playerPrefab = new List<GameObject>();

    private void Awake()
    {
        
    }

    private void Start()
    {
        instance = this;
        //플레이어 정보 없으면 코루틴 실행
        if (PlayerManager.PlayerInstance == null) StartCoroutine(SpawnPlayer());
    }

    IEnumerator SpawnPlayer()
    {
        // 방에 들어갈 때까지 대기
        yield return new WaitUntil(() => PhotonNetwork.InRoom);

        // 네트워크 상에서 생성하기
        PlayerManager.PlayerInstance = PhotonNetwork.Instantiate(playerPrefab[Random.Range(0, playerPrefab.Count)].name, new Vector3(-6.5f, 1, -8f), Quaternion.identity, 0);
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(1);
    }

    public void LeaveRoom()
    {
        Debug.Log($"[GameManager] 방을 나가기 위해 데이터 저장");

        StartCoroutine(SaveDataAndLeaveRoom());
    }

    public IEnumerator SaveDataAndLeaveRoom()
    {
        if (!PhotonNetwork.InRoom) yield break;

        // 게임 저장 
        PlayData playData = GetPlayData();
        yield return new WaitUntil(() => playData != null);
        Debug.Log($"[GameManager] 플레이 데이터 생성 완료");


        StartCoroutine(FirebaseDBManager.Instance.SavePlayData(playData));
        yield return new WaitUntil(() => FirebaseDBManager.Instance.isSaved == true);
        Debug.Log($"[GameManager] 플레이 데이터 저장 완료");

        if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Leaving) yield break;
        PhotonNetwork.LeaveRoom();
    }

    private PlayData GetPlayData()
    {
        Debug.Log($"[GameManager] 플레이 데이터 생성");

        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("FirebaseName", out object name);
        string userName = name.ToString();
        // 이름, 마지막 층 수, 날짜
        PlayData playData = new PlayData(userName, StageManager.Instance.CurrentStage);
        Debug.Log($"[GameManager] {playData.Name} {playData.HighScore} {playData.Date}");

        return playData;
    }


    public void ClearGame()
    {
        Debug.Log("[GameManager] 게임 클리어");

        // 클리어 팝업 활성화

        // 팝업에 LeaveRoom 이벤트 연결
        
    }
}
