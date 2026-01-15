using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using System.Collections;

public class FirebaseDBManager : MonoBehaviour
{
    DatabaseReference _dbRef;
    FirebaseUser _user;

    private void Start()
    {
        _dbRef = FirebaseAuthManager._dbRef;
        _user = FirebaseAuthManager._user;
    }

    public void LoadDatabase()
    {
        StartCoroutine(LoadUserDataCor());
    }

    IEnumerator LoadUserDataCor()
    {
        var DBTask = _dbRef.Child("users").Child(_user.UserId).GetValueAsync();
        yield return new WaitUntil(() => DBTask.IsCompleted);

        // 에러 핸들링
        if (DBTask.Exception != null)
        {
            Debug.LogWarning("[FirebaseDBManager] 데이터 불러오기 실패 " + DBTask.Exception);
        }
        else
        {
            DataSnapshot dataSnapshot = DBTask.Result;
            
        }
    }
}
