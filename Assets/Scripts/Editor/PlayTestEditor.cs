using UnityEditor;
using Photon.Pun;
using UnityEngine;

public class PlayTestEditor : UnityEditor.EditorWindow
{
    LobbyManager lobbyManager;
    bool wasPlaying;
    bool isLogined;

    [MenuItem("Tools/Login Manager/Auto Login")]
    public static void ShowWindow()
    {
        GetWindow<PlayTestEditor>("Auto Login").Focus();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Runtime Auto Login", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        if(Application.isPlaying == false)
        {
            if(GUILayout.Button("플레이"))
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/TitleScene.unity");
                EditorApplication.EnterPlaymode();
            }

            return;
        }

        if(PlayTestManager.Instance == null)
        {
            EditorGUILayout.HelpBox("플레이 테스트 매니저를 찾을 수 없습니다", MessageType.Error);
            return;
        }

        if (!PhotonNetwork.InRoom)
        {
            if (GUILayout.Button("로그인"))
            {
                EditorGUILayout.LabelField("로그인 후 로비로 이동", EditorStyles.boldLabel);
                PlayTestManager.Instance.ExecuteLogin();
            }
        }

        EditorGUILayout.Space(5);

        if (PhotonNetwork.InLobby)
        {
            EditorGUILayout.LabelField("테스트용 방 생성 후 입장", EditorStyles.boldLabel);
            if(GUILayout.Button("테스트용 방 생성"))
            {
                PlayTestManager.Instance.CreateTestRoom();
            }
        }

        EditorGUILayout.Space(10);

        if (GUILayout.Button("로그인 + 방생성"))
        {
            PlayTestManager.Instance.ExecuteLogin();
            PlayTestManager.Instance.CreateTestRoom();
        }
    }



    private void Update()
    {
        if (Application.isPlaying != wasPlaying)
        {
            // 에디터 창을 새로 그리는 메서드
            // 플레이 여부가 변경되었을 때 다시 그려준다
            Repaint();
        }
    }
}
