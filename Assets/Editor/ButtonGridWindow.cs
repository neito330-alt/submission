using UnityEditor;
using UnityEngine;

public class ButtonGridWindow : EditorWindow
{
    private int rows = 3;
    private int columns = 3;
    
    [MenuItem("Window/Button Grid Window")]
    public static void ShowWindow()
    {
        GetWindow<ButtonGridWindow>("Button Grid");
    }
    
    private void OnGUI()
    {
        // 行と列の数を設定可能にする
        rows = EditorGUILayout.IntField("行数", rows);
        columns = EditorGUILayout.IntField("列数", columns);
        
        // 最小値を1に制限
        rows = Mathf.Max(1, rows);
        columns = Mathf.Max(1, columns);
        
        EditorGUILayout.Space();
        
        // 表形式でボタンを配置
        for (int i = 0; i < rows; i++)
        {
            EditorGUILayout.BeginHorizontal();
            
            for (int j = 0; j < columns; j++)
            {
                if (GUILayout.Button($"Button {i},{j}", GUILayout.MinWidth(50), GUILayout.MaxWidth(50), GUILayout.MinHeight(50), GUILayout.MaxHeight(50)))
                {
                    Debug.Log($"Button {i},{j} clicked");
                    // ボタンがクリックされた時の処理
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
}