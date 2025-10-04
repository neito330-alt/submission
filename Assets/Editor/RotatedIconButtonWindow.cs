using UnityEditor;
using UnityEngine;

public class RotatedIconButtonWindow : EditorWindow
{
    private Texture2D icon;
    private float rotationAngle = 45f; // 回転角度
    
    [MenuItem("Window/Rotated Icon Button")]
    public static void ShowWindow()
    {
        GetWindow<RotatedIconButtonWindow>("Rotated Icon Button");
    }
    
    private void OnEnable()
    {
        // アイコンを読み込む (Assets/Editor Default Resources/以下に配置)
        icon = EditorGUIUtility.Load("icon.png") as Texture2D;
    }
    
    private void OnGUI()
    {
        rotationAngle = EditorGUILayout.Slider("回転角度", rotationAngle, 0f, 360f);
        
        // 回転したスタイルを作成
        GUIStyle rotatedButtonStyle = new GUIStyle(GUI.skin.button);
        rotatedButtonStyle.imagePosition = ImagePosition.ImageAbove;
        
        // ボタン描画領域を取得
        Rect buttonRect = GUILayoutUtility.GetRect(new GUIContent(icon), rotatedButtonStyle);
        
        // マトリックスを保存
        Matrix4x4 matrixBackup = GUI.matrix;
        
        // ボタンの中心を原点として回転
        Vector2 pivot = new Vector2(buttonRect.x + buttonRect.width * 0.5f, buttonRect.y + buttonRect.height * 0.5f);
        GUIUtility.RotateAroundPivot(rotationAngle, pivot);
        
        // ボタンを描画
        if (GUI.Button(buttonRect, new GUIContent(icon), rotatedButtonStyle))
        {
            Debug.Log("回転ボタンがクリックされました");
        }
        
        
        // マトリックスを復元
        GUI.matrix = matrixBackup;
    }
}