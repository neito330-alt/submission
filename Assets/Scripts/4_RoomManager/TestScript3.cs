using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine.InputSystem.EnhancedTouch;
using Unity.VisualScripting;
using Rooms.Auto;
using System.IO;
using System.Text.RegularExpressions;
using Rooms.PanelSystem;
using Rooms;


// [Serializable]
// public class StageDataController0
// {
//     private string _nextScene;
//     public bool _isShuffle = false;
//     public List<SlotDataList> _data = new List<SlotDataList>();
//     public Vector2Int _size = new Vector2Int(1, 1);
//     public int _buttonSize = 30;
//     public Vector2Int _start = new Vector2Int(0, 0);
//     public SlotData _currentSlot = new SlotData();


//     /// <summary>
//     /// ステージのパネルをシャッフルする
//     /// </summary>
//     /// <param name="count">シャッフル回数</param>
//     /// <remarks>
//     /// シャッフル回数を指定しない場合は50回シャッフルする
//     /// </remarks>
//     public void PanelShuffle(int count = 50)
//     {
//         List<Vector2Int> moveList = new List<Vector2Int>();
//         List<Vector2Int> rotateList = new List<Vector2Int>();

//         for (int y = 0; y < _size.y; y++)
//         {
//             for (int x = 0; x < _size.x; x++)
//             {
//                 if (_data[y][x].isSlot)
//                 {
//                     if (_data[y][x].IsMoveable)
//                     {
//                         moveList.Add(new Vector2Int(x,y));
//                     }
//                     if (_data[y][x].IsRotatable)
//                     {
//                         rotateList.Add(new Vector2Int(x,y));
//                     }
//                 }
//             }
//         }

//         for (int i = 0; i < count; i++)
//         {
//             Vector2Int pos0 = moveList[UnityEngine.Random.Range(0, moveList.Count)];

//             Vector2Int pos1 = moveList[UnityEngine.Random.Range(0, moveList.Count)];

//             RoomSetData tmp = _data[pos0.y][pos0.x].RoomSetData;

//             _data[pos0.y][pos0.x].RoomSetData = _data[pos1.y][pos1.x].RoomSetData;
//             _data[pos1.y][pos1.x].RoomSetData = tmp;
//         }

//         foreach (Vector2Int pos in rotateList)
//         {
//             _data[pos.y][pos.x].RoomSetData.Rotation = UnityEngine.Random.Range(0, 4) * 90;
//         }
//     }

//     public SlotData GetPanelSlotData(Vector2Int position)
//     {
//         if (position.x < 0 || position.y < 0)
//         {
//             return new SlotData(new Vector2Int(-1, -1));
//         }
//         if (position.y >= _data.Count || position.x >= _data[Math.Min(position.y, _data.Count - 1)].Count)
//         {
//             return new SlotData(new Vector2Int(-1, -1));
//         }
//         return _data[position.y][position.x];
//     }
// }

// #if UNITY_EDITOR
// [CustomPropertyDrawer(typeof(StageDataController))]
// public class SlotDataListEditor : PropertyDrawer
// {
//     private class SlotDragData : ScriptableObject
//     {
//         public RoomSetData roomSetData;
//         public Vector2Int position;

//         public SlotDragData(Vector2Int position, RoomSetData roomSetData)
//         {
//             this.roomSetData = roomSetData;
//             this.position = position;
//         }
//     }


//     private SerializedProperty _dataProperty;

//     private Vector2Int currentPosition = new Vector2Int(-1, -1);

//     private Vector2 scrollPosition;


//     SerializedProperty GetSlotProperty(Vector2Int position)
//     {
//         if (position.x < 0 || position.y < 0)
//         {
//             return null;
//         }
//         if (position.y >= _dataProperty.arraySize || position.x >= _dataProperty.GetArrayElementAtIndex(position.y).FindPropertyRelative("rowData").arraySize)
//         {
//             return null;
//         }
//         _dataProperty.GetArrayElementAtIndex(position.y).FindPropertyRelative("rowData").GetArrayElementAtIndex(position.x).
//         return _dataProperty.GetArrayElementAtIndex(position.y).FindPropertyRelative("rowData").GetArrayElementAtIndex(position.x);
//     }

    

//     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//     {
//         position.height = EditorGUIUtility.singleLineHeight;

//         EditorGUI.BeginProperty(position, label, property);

//         SerializedProperty nextSceneProperty = property.FindPropertyRelative("_nextScene");
//         SerializedProperty isShuffleProperty = property.FindPropertyRelative("_isShuffle");
//         SerializedProperty startProperty = property.FindPropertyRelative("_start");
//         SerializedProperty sizeProperty = property.FindPropertyRelative("_size");
//         SerializedProperty buttonSizeProperty = property.FindPropertyRelative("_buttonSize");
//         _dataProperty = property.FindPropertyRelative("_data");



//         EditorGUI.LabelField(position, "Grid Settings", EditorStyles.boldLabel);

//         position.height = EditorGUI.GetPropertyHeight(nextSceneProperty);
//         EditorGUI.PropertyField(position, nextSceneProperty, new GUIContent("Next Scene"));
//         position.y += position.height + EditorGUIUtility.standardVerticalSpacing;


//         position.height = EditorGUI.GetPropertyHeight(isShuffleProperty);
//         EditorGUI.PropertyField(position, isShuffleProperty, new GUIContent("Shuffle"));
//         position.y += position.height + EditorGUIUtility.standardVerticalSpacing;


//         position.height = EditorGUI.GetPropertyHeight(startProperty);
//         EditorGUI.PropertyField(position, startProperty, new GUIContent("Start Position"));
//         position.y += position.height + EditorGUIUtility.standardVerticalSpacing;


//         position.height = EditorGUI.GetPropertyHeight(sizeProperty);
//         EditorGUI.PropertyField(position, sizeProperty, new GUIContent("Grid Size"));
//         position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

//         position.height = EditorGUI.GetPropertyHeight(buttonSizeProperty);
//         EditorGUI.IntSlider(position, buttonSizeProperty, 10, 100, new GUIContent("Button Size"));
//         position.y += position.height + EditorGUIUtility.standardVerticalSpacing;








//         EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);

//         // 基本設定
//         EditorGUI.BeginChangeCheck();

        


//         string newNextScene = EditorGUILayout.TextField("nextScene", targetList.nextScene);

//         bool newIsShuffle = EditorGUILayout.Toggle("Shuffle", targetList.isShuffle);

//         Vector2Int newStart = EditorGUILayout.Vector2IntField("Start", targetList.start);
//         Vector2Int newGoal = EditorGUILayout.Vector2IntField("Goal", targetList.goal);


//         // グリッド表示
//         EditorGUILayout.Space();

//         EditorGUILayout.LabelField("パネルエディター", EditorStyles.boldLabel);
//         EditorGUI.indentLevel++;

//         Vector2Int newSize = EditorGUILayout.Vector2IntField("グリッドサイズ", targetList.size);
//         int newButtonSize = EditorGUILayout.IntSlider("ボタンの大きさ", targetList.buttonSize, 10, 100);

//         DrawButtons();

//         EditorGUILayout.Space();

//         bool isRange = currentPosition.x != Mathf.Clamp(currentPosition.x, 0, targetList.size.x - 1) ||
//                         currentPosition.y != Mathf.Clamp(currentPosition.y, 0, targetList.size.y - 1);

//         EditorGUI.BeginDisabledGroup(isRange);

//         EditorGUILayout.PropertyField(
//             serializedObject.FindProperty("currentSlot"),new GUIContent("Current Slot Data"),true
//         );

//         EditorGUI.EndDisabledGroup();

//         EditorGUI.indentLevel--;


//         if (EditorGUI.EndChangeCheck())
//         {
//             Undo.RecordObject(targetList, "Change Grid Size");
//             targetList.isShuffle = newIsShuffle;
//             targetList.nextScene = newNextScene;
//             targetList.size = new Vector2Int(Mathf.Max(1, newSize.x), Mathf.Max(1, newSize.y));
//             targetList.buttonSize = newButtonSize;
//             targetList.start = newStart;
//             targetList.goal = newGoal;
//             ResizeList();

//             if (!isRange)
//             {
//                 targetList.data[currentPosition.y][currentPosition.x] = targetList.currentSlot;

//             }
//         }

//         serializedObject.ApplyModifiedProperties();
//     }


//     void DrawButtons(Vector2Int size, int buttonSize)
//     {
//         // サイズチェック
//         if (
//             _dataProperty.arraySize != size.y ||
//             _dataProperty.arraySize > 0 && 
//             _dataProperty.GetArrayElementAtIndex(0).FindPropertyRelative("rowData").arraySize != size.x
//         )
//         {
//             ResizeList();
//         }


//         for (int y = 0; y < size.y; y++)
//         {
//             for (int x = 0; x < size.x; x++)
//             {
//                 Vector2Int position = new Vector2Int(x, y);
//                 string controlName = $"{(char)((int)'A' + x)}{y}";
//                 bool isSelected = position == currentPosition;
//                 var data = GetSlotProperty(position);

//                 Rect rect = GUILayoutUtility.GetRect(
//                     buttonSize,
//                     buttonSize,
//                     GUILayout.ExpandWidth(false)
//                 );
//                 rect.x += EditorGUI.indentLevel * 15; // インデントを適用


//                 GUI.color = isSelected && data.IsNotEmpty ? new Color(2,2,2) : Color.white; // 選択時の色を設定
//                 GUI.backgroundColor = data.isSlot ? data.IsNotEmpty ?
//                     data.RoomSetData.RoomSetController.roomMode.ToColor() :
//                     Color.white :
//                     Color.gray;
                
//                 GUIStyle style = new GUIStyle(GUI.skin.button)
//                 {
//                     alignment = TextAnchor.MiddleCenter,
//                     imagePosition = data.isSlot && data.IsNotEmpty ? ImagePosition.ImageOnly : ImagePosition.ImageAbove,
//                     normal = { textColor = isSelected ? Color.yellow : Color.white },
//                 };



//             }
            
//         }
//         GUI.color = Color.white; // 色をリセット
//         GUI.contentColor = Color.white; // テキスト色をリセット
//         GUI.backgroundColor = Color.white; // 色をリセット

//     }



//     void DrawButtons0()
//     {
//         scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

//         // サイズチェック
//         if (targetList.data.Count != targetList.size.y ||
//             (targetList.data.Count > 0 && targetList.data[0].rowData.Count != targetList.size.x))
//         {
//             ResizeList();
//         }

//         for (int i = 0; i < targetList.size.y; i++)
//         {
//             EditorGUILayout.BeginHorizontal();
//             for (int j = 0; j < targetList.size.x; j++)
//             {
//                 Vector2Int position = new Vector2Int(j, i);
//                 string controlName = $"{(char)((int)'A' + j)}{i}";
//                 bool isSelected = position == currentPosition;
//                 var data = targetList.data[i][j];

//                 // ボタン領域を確保
//                 Rect rect = GUILayoutUtility.GetRect(
//                     targetList.buttonSize,
//                     targetList.buttonSize,
//                     GUILayout.ExpandWidth(false)
//                 );

//                 rect.x += EditorGUI.indentLevel * 15; // インデントを適用

                

//                 GUI.color = isSelected && data.IsNotEmpty ? new Color(2,2,2) : Color.white; // 選択時の色を設定
//                 GUI.backgroundColor = data.isSlot ? data.IsNotEmpty ?
//                     data.RoomSetData.RoomSetController.roomMode.ToColor() :
//                     Color.white :
//                     Color.gray;


//                 GUIStyle style = new GUIStyle(GUI.skin.button)
//                 {
//                     alignment = TextAnchor.MiddleCenter,
//                     imagePosition = data.isSlot && data.IsNotEmpty ? ImagePosition.ImageOnly : ImagePosition.ImageAbove,
//                     normal = { textColor = isSelected ? Color.yellow : Color.white },
                    
//                 };

//                 GUIContent content;
//                 if (data.IsNotEmpty && false)
//                 {
//                     content = new GUIContent(data.RoomSetData.RoomSetController.roomThumb);
//                 }
//                 else
//                 {
//                     content = new GUIContent(controlName);
//                 }

//                 //GUI.Box(rect, "");

//                 DragStart(rect, new Vector2Int(j, i));
//                 HandleDragAndDrop(rect, new Vector2Int(j, i));

                
                

//                 Event evt = Event.current;
                
//                 if (rect.Contains(evt.mousePosition))
//                 {
//                     if (evt.type == EventType.MouseDown)
//                     {
//                         if (Event.current.button == 0)
//                         {
//                             if (Event.current.clickCount == 2)
//                             {
//                                 if (data.isSlot && data.IsNotEmpty)
//                                 {
//                                     PrefabStageUtility.OpenPrefab(
//                                         AssetDatabase.GetAssetPath(data.RoomSetData.RoomSetController.gameObject)
//                                     );
//                                 }
//                             }
//                             else
//                             {
//                                 currentPosition = position;
//                                 targetList.currentSlot = data;
//                             }
//                         }
//                         else if (Event.current.button == 1)
//                         {
//                             data.isSlot = !data.isSlot;
//                         }
//                     }
                    
//                 }
//                 if (isSelected && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
//                 {
//                     if (data.IsNotEmpty)
//                     {
//                         data.RoomSetData.RoomSetController = null;
//                     }
//                 }
//                 GUI.SetNextControlName(controlName);
//                 GUI.Button(rect, content, style);

                
                

//                 int padding = 5;
//                 Rect thumbRect = new Rect(
//                     (int)rect.x + padding,
//                     (int)rect.y + padding,
//                     (int)rect.width - 2 * padding,
//                     (int)rect.height - 2 * padding
//                 );
                
//                 // 現在のマトリックスを保存
//                 Matrix4x4 matrixBackup = GUI.matrix;
//                 if (data.IsNotEmpty)
//                 {
//                     // 回転の中心点を計算
//                     Vector2 pivot = new Vector2(thumbRect.x + thumbRect.width * 0.5f, thumbRect.y + thumbRect.height * 0.5f);
//                     GUIUtility.RotateAroundPivot(data.RoomSetData.Rotation, pivot);
//                 }

                
//                 if ((data?.RoomSetData?.RoomSetController?.roomThumb != null) && (data?.isSlot ?? false))GUI.DrawTexture(thumbRect, data.RoomSetData.RoomSetController.roomThumb, ScaleMode.ScaleAndCrop);

//                 // マトリックスを復元
//                 GUI.matrix = matrixBackup;
//             }
//             EditorGUILayout.EndHorizontal();
//         }

//         GUI.color = Color.white; // 色をリセット
//         GUI.contentColor = Color.white; // テキスト色をリセット
//         GUI.backgroundColor = Color.white; // 色をリセット
//         EditorGUILayout.EndScrollView();
//     }

//     private void DragStart(Rect dragArea, Vector2Int position)
//     {
//         Event evt = Event.current;

//         SlotDragData slotDragData = ScriptableObject.CreateInstance<SlotDragData>();
//         slotDragData.position = position;
//         slotDragData.roomSetData = targetList.data[position.y][position.x].RoomSetData;

//         if (evt.type == EventType.MouseDrag && dragArea.Contains(evt.mousePosition))
//         {
//             DragAndDrop.PrepareStartDrag();
//             DragAndDrop.objectReferences = new UnityEngine.Object[] { slotDragData };
//             DragAndDrop.StartDrag("ドラッグ中");
//             Event.current.Use();
//         }
//     }

//     private void HandleDragAndDrop(Rect dropArea, Vector2Int position)
//     {
//         Event evt = Event.current;

//         switch (evt.type)
//         {
//             case EventType.DragUpdated:
//             case EventType.DragPerform:
//                 if (!dropArea.Contains(evt.mousePosition))
//                     return;

//                 // ドラッグ対象を有効に見せる
//                 DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

//                 if (evt.type == EventType.DragPerform)
//                 {
//                     DragAndDrop.AcceptDrag();

//                     foreach (var draggedObject in DragAndDrop.objectReferences)
//                     {
//                         string path = AssetDatabase.GetAssetPath(draggedObject);

//                         if (draggedObject == null) continue;
//                         if (draggedObject is SlotDragData slotDragData)
//                         {
//                             targetList.data[slotDragData.position.y][slotDragData.position.x].RoomSetData = targetList.data[position.y][position.x].RoomSetData;
//                             targetList.data[position.y][position.x].RoomSetData = slotDragData.roomSetData;

//                             currentPosition = position;
//                             targetList.currentSlot = targetList.data[position.y][position.x];

//                             EditorUtility.SetDirty(targetList);
//                             continue;
//                         }
//                         else if (draggedObject is GameObject && draggedObject.GetComponent<RoomSetController>() != null)
//                         {
//                             // ドラッグされたオブジェクトがRoomSetControllerを持つ場合
//                             RoomSetController roomSetController = draggedObject.GetComponent<RoomSetController>();

//                             // 対象のスロットに新しいRoomSetDataを設定
//                             if (targetList.data[position.y][position.x].isSlot)
//                             {
//                                 targetList.data[position.y][position.x].RoomSetData.RoomSetController = roomSetController;

//                                 currentPosition = position;
//                                 targetList.currentSlot = targetList.data[position.y][position.x];

//                                 EditorUtility.SetDirty(targetList);
//                             }
//                         }
//                         else if (AssetDatabase.IsValidFolder(path))
//                         {
//                             // フォルダ内の全ファイル（再帰的に）を取得（例：.prefab）
//                             string[] guids = AssetDatabase.FindAssets("", new[] { path });

//                             foreach (string guid in guids)
//                             {
//                                 string assetPath = AssetDatabase.GUIDToAssetPath(guid);
//                                 string fileName = Path.GetFileName(assetPath);
//                                 Match match = Regex.Match(fileName, @"_([A-Z]+)([0-9]+)\.prefab$");
//                                 if (match.Success)
//                                 {
//                                     int col = (int)match.Groups[1].Value[0] - (int)'A';
//                                     int idx = int.Parse(match.Groups[2].Value);

//                                     UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);

//                                     if (asset is GameObject && asset.GetComponent<RoomSetController>() != null)
//                                     {
//                                         RoomSetController roomSetController = asset.GetComponent<RoomSetController>();

//                                         // 対象のスロットに新しいRoomSetDataを設定
//                                         if (idx < targetList.data.Count && col < targetList.data[idx].Count && targetList.data[idx][col].isSlot)
//                                         {
//                                             targetList.data[idx][col].RoomSetData.RoomSetController = roomSetController;

//                                             currentPosition = new Vector2Int(col, idx);
//                                             targetList.currentSlot = targetList.data[idx][col];

//                                             EditorUtility.SetDirty(targetList);
//                                         }
//                                     }
//                                 }
//                             }
//                         }
//                     }
//                     GUI.FocusControl(null);
//                     GUIUtility.hotControl = 0;
//                 }
                

//                 evt.Use();
//                 break;
//         }
//     }

//     private void ResizeList()
//     {
//         // 行のリサイズ
//         while (targetList.data.Count < targetList.size.y)
//         {
//             var newRow = new SlotDataList();
//             for (int j = 0; j < targetList.size.x; j++)
//             {
//                 newRow.rowData.Add(new SlotData(new Vector2Int(j, targetList.data.Count)));
//             }
//             targetList.data.Add(newRow);
//         }

//         while (targetList.data.Count > targetList.size.y)
//         {
//             targetList.data.RemoveAt(targetList.data.Count - 1);
//         }

//         // 列のリサイズ
//         for (int i = 0; i < targetList.data.Count; i++)
//         {
//             while (targetList.data[i].Count < targetList.size.x)
//             {
//                 targetList.data[i].rowData.Add(new SlotData(new Vector2Int(targetList.data[i].Count, i)));
//             }

//             while (targetList.data[i].Count > targetList.size.x)
//             {
//                 targetList.data[i].rowData.RemoveAt(targetList.data[i].Count - 1);
//             }
//         }

//         EditorUtility.SetDirty(targetList);
//     }

// }
// #endif


public class TestScript3 : MonoBehaviour
{

    
}
