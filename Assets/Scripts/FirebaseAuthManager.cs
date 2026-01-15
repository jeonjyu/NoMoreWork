using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum LoginErrMsg
{
    [Description("로그인 정보를 다시 확인해주세요")] InvaildEmail = 11,
    [Description("로그인 정보를 다시 확인해주세요")] WrongPassword = 12,
    [Description("존재하지 않는 아이디입니다")] UserNotFound = 14,
    [Description("이메일을 입력해주세요")] MissingEmail = 37,
    [Description("비밀번호를 입력해주세요")] MissingPassword = 38,

}

public enum RegisterErrMsg
{
    [Description("이메일을 확인해주세요")] MissingEmail = 37,
    [Description("패스워드를 입력해주세요")] MissingPassword = 38,
    [Description("이미 등록된 이메일입니다")] EmailAlreadyInUse = 8
}

/// <summary>
/// 로비에서 로그인, 회원가입 처리
/// </summary>
public class FirebaseAuthManager : MonoBehaviour
{
    [SerializeField] Button _loginBtn;
    private FirebaseApp _app;
    public FirebaseAuth _auth;
    public static FirebaseUser _user;
    public static DatabaseReference _dbRef;

    [SerializeField] TMP_InputField _emailField;
    [SerializeField] TMP_InputField _passwordField;
    [SerializeField] TMP_InputField _usernameField;

    [SerializeField] TMP_Text _errText;

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                _app = FirebaseApp.DefaultInstance;
                _auth = FirebaseAuth.DefaultInstance;
                _dbRef = FirebaseDatabase.DefaultInstance.RootReference;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                Debug.LogError("[" + this.GetType().Name + "] Firebase 의존성 설정 실패 : " + dependencyStatus);
                // Firebase Unity SDK is not safe to use here.
                //_app = null;
                //_auth = null;
            }
        });
    }

    public void Login()
    {
        StartCoroutine(LoginCoroutine(_emailField.text, _passwordField.text));
    }

    IEnumerator LoginCoroutine(string id, string pw)
    {
        // 로그인 결과 콜백으로 받아와 저장
        Task<AuthResult> loginTask = _auth.SignInWithEmailAndPasswordAsync(id, pw);

        // 로그인 정보 저장이 완료될 때까지 대기
        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

        // 로그인 정보 받아오면
        if (loginTask.Exception != null)
        {
            Debug.LogError("[FirebaseAuthManager] 로그인 실패: " + loginTask.Exception);

            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException; 
            AuthError error = (AuthError) firebaseException.ErrorCode;

            PrintErrMsg((LoginErrMsg)error);
        }
        else
        {
            _user = loginTask.Result.User;
            Debug.Log($"[FirebaseAuthManager] 로그인 성공 : {_user.DisplayName}");
            SceneManager.LoadScene(1);
        }
    }

    public void Register()
    {
        StartCoroutine(RegisterCoroutine(_emailField.text, _passwordField.text, _usernameField.text));
    }

    IEnumerator RegisterCoroutine(string id, string ps, string username)
    {
        Task<AuthResult> registerResult = _auth.CreateUserWithEmailAndPasswordAsync(id, ps);

        yield return new WaitUntil(predicate: () => registerResult.IsCompleted);

        if (registerResult.Exception != null)
        {
            Debug.LogError("[FirebaseAuthManager] 회원가입 실패: " + registerResult.Exception);

            FirebaseException firebaseException = registerResult.Exception.GetBaseException() as FirebaseException;
            AuthError error = (AuthError)firebaseException.ErrorCode;
            Debug.LogError("[FirebaseAuthManager] 회원가입 실패: " + error);

            PrintErrMsg((RegisterErrMsg)error);
        }
        else
        {
            _user = registerResult.Result.User;
            // user가 null이 아니면 프로필 설정
            if (_user != null) 
            {
                // 로컬 프로필
                UserProfile profile = new UserProfile { DisplayName = username };

                // 파이어베이스에 콜백으로 프로필 업데이트
                Task profileTask = _user.UpdateUserProfileAsync(profile);
                yield return new WaitUntil(predicate: () => profileTask.IsCompleted);

                // 프로필 설정 에러 처리
                if (profileTask.Exception != null)
                {
                    Debug.LogError("[FirebaseAuthManager] 프로필 설정 실패");
                    FirebaseException firebaseException = profileTask.Exception.GetBaseException() as FirebaseException;
                    AuthError error = (AuthError)firebaseException.ErrorCode;
                    _errText.text = "프로필 설정에 실패했습니다";
                }
                else
                {
                    _errText.text = "";
                    Debug.Log("[FirebaseAuthManager] 프로필 설정 성공 " + _user.DisplayName);
                }
            }
        }
    }

    // Enum에 정해둔 에러 Desciption을 출력 텍스트로 넣어준다
    public void PrintErrMsg(Enum errCode)
    {        
        FieldInfo fieldInfo = errCode.GetType().GetField(errCode.ToString());
        DescriptionAttribute description = fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute), false) as DescriptionAttribute;
        Debug.LogError("[" + this.GetType().Name + "] " + errCode.GetType().Name + " 에러코드 : " + description);

        _errText.text = description.Description;
    }
}
