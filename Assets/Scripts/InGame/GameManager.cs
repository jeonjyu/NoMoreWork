using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.SceneManagement;

/// <summary>
/// 플레이어 생성, 방에서 나가는 거 관리
/// </summary>
public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    [SerializeField] GameObject playerPrefab;

    private void Start()
    {
        instance = this;
        //플레이어 정보 없으면 코루틴 실행
        if (PlayerManager.LocalPlayerInstance == null) StartCoroutine(SpawnPlayer());
    }

    IEnumerator SpawnPlayer()
    {
        // 방에 들어갈 때까지 대기
        yield return new WaitUntil(() => PhotonNetwork.InRoom);

        // 네트워크 상에서 생성하기
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(1);
    }

    public void LeaveRoom()
    {
        if (!PhotonNetwork.InRoom) return;
        if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Leaving) return;
        PhotonNetwork.LeaveRoom();
    }
}
