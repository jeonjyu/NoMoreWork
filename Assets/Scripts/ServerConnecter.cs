using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ServerConnecter : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1.0";

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void ConnectToSevcer()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
        PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    // 마스터 서버 연결
    public override void OnConnectedToMaster()
    {
        Debug.Log("[ServerConnecter] 마스터 서버 연결");
        PhotonNetwork.JoinLobby();
        //SceneManager.LoadScene(1);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("[ServerConnecter] 로비 입장");
    }
}
