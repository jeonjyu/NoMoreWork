using UnityEditor;
using UnityEngine;

public class PlayTestEditor: EditorWindow
{
    [MenuItem("Tools/Play Test Manager/Play Test")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<PlayTestEditor>("Play Test");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Play Test");
        EditorGUILayout.Space(10);

        if(GUILayout.Button("스테이지 클리어"))
        {
            for(int i = 0; i < StageManager.Instance.puzzleButtons.Count; i++)
            {
                StageManager.Instance.UpdateClearedPuzzle(i+1);
            }
        }
    }

}
