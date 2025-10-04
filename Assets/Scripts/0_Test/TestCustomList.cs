using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// カスタムデータクラス
[System.Serializable]
public class TestCustom
{
    public int intValue;
    public float floatValue;
    public bool boolValue;
    public Texture2D textureValue;
    public int rotate = 0;
}

// 2次元配列管理クラス
[System.Serializable]
public class TestCustomList : MonoBehaviour
{
    public List<List<TestCustom>> data = new List<List<TestCustom>>();
    public Vector2Int size = new Vector2Int(1, 1);
    [Range(10, 100)] public int buttonSize = 30;
}

#if UNITY_EDITOR
[CustomEditor(typeof(TestCustomList))]
public class TestCustomListEditor : Editor
{
    private TestCustomList targetList;
    private int selectedRow = -1;
    private int selectedCol = -1;
    private Vector2 scrollPosition;
    private Dictionary<Vector2Int, int> buttonRotations = new Dictionary<Vector2Int, int>();

    private int selectedTab = 0;
    private readonly string[] tabTitles = { "Default", "Custom" };
    private Editor testCustomEditor;

    private void OnEnable()
    {
        targetList = (TestCustomList)target;

        // 初期回転角度を設定
        for (int i = 0; i < targetList.size.y; i++)
        {
            for (int j = 0; j < targetList.size.x; j++)
            {
                var key = new Vector2Int(i, j);
                if (!buttonRotations.ContainsKey(key))
                {
                    buttonRotations[key] = 0;
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 基本設定
        EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        Vector2Int newSize = EditorGUILayout.Vector2IntField("Size", targetList.size);
        int newButtonSize = EditorGUILayout.IntSlider("Button Size", targetList.buttonSize, 10, 100);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(targetList, "Change Grid Size");
            targetList.size = new Vector2Int(Mathf.Max(1, newSize.x), Mathf.Max(1, newSize.y));
            targetList.buttonSize = newButtonSize;
            ResizeList();
        }

        // グリッド表示
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Button Grid", EditorStyles.boldLabel);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // サイズチェック
        if (targetList.data.Count != targetList.size.y ||
            (targetList.data.Count > 0 && targetList.data[0].Count != targetList.size.x))
        {
            ResizeList();
        }

        for (int i = 0; i < targetList.size.y; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < targetList.size.x; j++)
            {
                var data = targetList.data[i][j];
                var key = new Vector2Int(i, j);

                // ボタン領域を確保
                Rect rect = GUILayoutUtility.GetRect(targetList.buttonSize, targetList.buttonSize,
                    GUILayout.ExpandWidth(false));

                // 現在のマトリックスを保存
                Matrix4x4 matrixBackup = GUI.matrix;

                // 回転の中心点を計算
                Vector2 pivot = new Vector2(rect.x + rect.width * 0.5f, rect.y + rect.height * 0.5f);
                GUIUtility.RotateAroundPivot(data.rotate, pivot);

                // ボタンスタイルを設定
                bool isSelected = (i == selectedRow && j == selectedCol);
                GUIStyle style = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleCenter,
                    imagePosition = ImagePosition.ImageAbove,
                    normal = { textColor = isSelected ? Color.yellow : Color.white }
                };

                // ボタン描画
                GUIContent content = data.textureValue != null ?
                    new GUIContent(data.textureValue) :
                    new GUIContent($"{i},{j}");

                if (GUI.Button(rect, content, style))
                {
                    selectedRow = i;
                    selectedCol = j;
                    UpdateCustomEditor();
                }

                // マトリックスを復元
                GUI.matrix = matrixBackup;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        // 選択されたボタンの編集
        if (selectedRow >= 0 && selectedCol >= 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Selected Cell ({selectedRow},{selectedCol})", EditorStyles.boldLabel);

            // タブ表示
            selectedTab = GUILayout.Toolbar(selectedTab, tabTitles);
            EditorGUILayout.Space();

            var selectedData = targetList.data[selectedRow][selectedCol];

            if (selectedTab == 0) // Defaultタブ
            {
                // ScriptableObjectラッパーを作成
                var wrapper = ScriptableObject.CreateInstance<SerializedObjectWrapper>();
                wrapper.target = selectedData;

                SerializedObject serializedWrapper = new SerializedObject(wrapper);
                SerializedProperty iterator = serializedWrapper.GetIterator();

                bool enterChildren = true;
                while (iterator.NextVisible(enterChildren))
                {
                    enterChildren = false;
                    if (iterator.name == "target")
                    {
                        EditorGUILayout.PropertyField(iterator, true);
                    }
                }

                if (serializedWrapper.ApplyModifiedProperties())
                {
                    EditorUtility.SetDirty(targetList);
                }

                ScriptableObject.DestroyImmediate(wrapper);
            }
            else if (selectedTab == 1) // Customタブ
            {
                if (testCustomEditor == null)
                {
                    UpdateCustomEditor();
                }

                if (testCustomEditor != null)
                {
                    testCustomEditor.OnInspectorGUI();
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    // Editor クラス内の修正部分
    private void UpdateCustomEditor()
    {
        if (selectedRow >= 0 && selectedCol >= 0)
        {
            // 既存のエディタを破棄
            if (testCustomEditor != null)
            {
                DestroyImmediate(testCustomEditor);
            }

            // 新しいラッパーを作成
            var wrapper = ScriptableObject.CreateInstance<TestCustomWrapper>();
            wrapper.data = targetList.data[selectedRow][selectedCol];

            // エディタを作成
            testCustomEditor = Editor.CreateEditor(wrapper);

            // 一時オブジェクトとしてマーク（シーンに保存されないように）
            wrapper.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    private void ResizeList()
    {
        // 行のリサイズ
        while (targetList.data.Count < targetList.size.y)
        {
            var newRow = new List<TestCustom>();
            for (int j = 0; j < targetList.size.x; j++)
            {
                newRow.Add(new TestCustom());
            }
            targetList.data.Add(newRow);
        }

        while (targetList.data.Count > targetList.size.y)
        {
            targetList.data.RemoveAt(targetList.data.Count - 1);
        }

        // 列のリサイズ
        for (int i = 0; i < targetList.data.Count; i++)
        {
            while (targetList.data[i].Count < targetList.size.x)
            {
                targetList.data[i].Add(new TestCustom());
            }

            while (targetList.data[i].Count > targetList.size.x)
            {
                targetList.data[i].RemoveAt(targetList.data[i].Count - 1);
            }
        }

        EditorUtility.SetDirty(targetList);
    }
}

// シリアライズ可能なラッパークラス
public class SerializedObjectWrapper : ScriptableObject
{
    public TestCustom target;

    private void OnEnable()
    {
        hideFlags = HideFlags.HideAndDontSave;
    }
}

// TestCustom を保持する ScriptableObject ラッパー
public class TestCustomWrapper : ScriptableObject
{
    public TestCustom data;
}
#endif