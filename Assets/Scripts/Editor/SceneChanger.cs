using UnityEditor;
using UnityEngine;

public class SceneChanger : EditorWindow
{

    [MenuItem("Tools/Scene Manager/Scene Changer")]
    public static void ShowWindow()
    {
        GetWindow<SceneChanger>("Scene Changer").Focus();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Scene Changer", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        if(GUILayout.Button("로비 씬"))
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/LobbyScene.unity");
        }
        
        EditorGUILayout.Space(5);
        
        if(GUILayout.Button("룸 씬"))
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/RoomScene.unity");
        }
        
        EditorGUILayout.Space(5);

        if(GUILayout.Button("인게임 씬"))
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/InGameScene.unity");
        }
    }
}
