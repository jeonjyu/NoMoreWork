using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using System.Collections;
using Photon.Pun;
using HashTable = ExitGames.Client.Photon.Hashtable;


/// <summary>
/// user 정보 DB에 저장,로드
/// </summary>
public class FirebaseDBManager : MonoBehaviour
{
    DatabaseReference _dbRef;
    FirebaseUser _user;

    IEnumerator Start()
    {
        Debug.Log("[FirebaseDBManager] AuthManager 기다리는 중");
        yield return new WaitUntil(() => FirebaseAuthManager.isAwaken == true);

        Debug.Log("[FirebaseDBManager] Start");
        _dbRef = FirebaseAuthManager._dbRef;
        _user = FirebaseAuthManager._user;
        
        FirebaseAuthManager.OnLogin += LoadFromDatabase;
        FirebaseAuthManager.OnRegister += SaveToDatabase;
    }

    // UserInfo 받아와서 저장
    public void SaveToDatabase(UserInfo userInfo)
    {
        StartCoroutine(UpdateNickname(userInfo));
    }

    IEnumerator UpdateNickname(UserInfo userInfo)
    {
        if (_dbRef == null) Debug.Log("[FirebaseDBManager] db 없음");
        if (userInfo == null) Debug.Log("[FirebaseDBManager] user 없음");

        HashTable userProperty = new HashTable() { { "UserName", userInfo.UserName } };

        var DBTask = _dbRef.Child("users").Child(userInfo.UserId).Child("username").SetValueAsync(userInfo.UserName);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null) 
        {
            Debug.LogWarning("[FirebaseDBManager] 닉네임 업데이트 실패 : " + DBTask.Exception);
        }
        else
        {
            // 저장 완료 
            PhotonNetwork.LocalPlayer.SetCustomProperties(userProperty);
            Debug.Log("[FirebaseDBManager] 닉네임 업데이트 성공 " + userInfo.UserName);
        }
    }

    // 프로필 이미지 업데이트
    //IEnumerator UpdateProfileImage(Image image)
    //{
    //    var DBTask = _dbRef.Child("users").Child(_user./*이미지*/).SetValueAsync(image);

    //    yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

    //    if (DBTask.Exception != null)
    //    {
    //        Debug.LogWarning("[FirebaseDBManager] 프로필 이미지 업데이트 실패 : " + DBTask.Exception);
    //    }
    //    else
    //    {
    //        // 저장 완료 
    //        Debug.Log("[FirebaseDBManager] 프로필 이미지 업데이트 성공 ");
    //    }
    //}

    public void LoadFromDatabase(string id)
    {
        StartCoroutine(LoadUserDataCor(id));
    }

    IEnumerator LoadUserDataCor(string id)
    {
        var DBTask = _dbRef.Child("users").Child(id).GetValueAsync();
        yield return new WaitUntil(() => DBTask.IsCompleted);

        // 에러 핸들링
        if (DBTask.Exception != null)
        {
            Debug.LogWarning("[FirebaseDBManager] 데이터 불러오기 실패 " + DBTask.Exception);
        }
        else
        {
            DataSnapshot dataSnapshot = DBTask.Result;
            PhotonNetwork.NickName = dataSnapshot.Child("username").Exists ? dataSnapshot.Child("username").Value.ToString() : "player";
        }
    }
}
