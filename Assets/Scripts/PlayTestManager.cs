using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loginUser
{
    public string id;
    public string pw;

    public loginUser(string id, string pw)
    {
        this.id = id;
        this.pw = pw;
    }
}

// 클릭하면 자동으로 로그인
public class PlayTestManager : Singleton<PlayTestManager>
{
    loginUser tstUser = new loginUser("tst", "tst@nomorework.com");

    // FirebaseAuthManager가 생성될 때까지 대기
    IEnumerator Start()
    {
        yield return new WaitUntil(() => FirebaseAuthManager.Instance);
    }

    public void ExecuteLogin()
    {
        if (FirebaseAuthManager.Instance != null)
        {
            StartCoroutine(FirebaseAuthManager.Instance.LoginCoroutine(tstUser.id, tstUser.pw));
        }
    }

    public void CreateTestRoom()
    {
        StartCoroutine(CreateTestRoomCor());
    }

    IEnumerator CreateTestRoomCor()
    {
        yield return new WaitUntil(() => PhotonNetwork.InLobby);

        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 4, PublishUserId = true };
        PhotonNetwork.CreateRoom("안녕하세요", roomOptions);
    }
}
