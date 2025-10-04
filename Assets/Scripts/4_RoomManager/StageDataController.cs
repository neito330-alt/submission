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
using UnityEngine.UIElements;


namespace Rooms.PanelSystem
{
    public class StageDataController : MonoBehaviour
    {
        public string _nextScene = "Stage0_0";
        public string NextScene
        {
            get => _nextScene;
            set => _nextScene = value;
        }
        
        public bool _isShuffle = false;
        public bool IsShuffle
        {
            get => _isShuffle;
            set => _isShuffle = value;
        }


        public Vector2Int _startPosition = new Vector2Int(0, 0);
        public Vector2Int StartPosition
        {
            get => _startPosition;
            set => _startPosition = value;
        }


        public Vector2Int _size = new Vector2Int(1, 1);
        public Vector2Int Size
        {
            get => _size;
            set => _size = new Vector2Int(Mathf.Max(1, value.x), Mathf.Max(1, value.y));
        }


        public int buttonSize = 30;

        public List<SlotDataList> _data = new List<SlotDataList>();
        public List<SlotDataList> Data
        {
            get => _data;
            set => _data = value;
        }

        public SlotData currentSlot = new SlotData();


        void Awake()
        {
            for (int x=0; x<Size.x;x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    Data[y][x].Position = new Vector2Int(x, y);
                    if (Data[y][x].RoomSetData != null)
                    {
                        Data[y][x].RoomSetData.Position = new Vector2Int(x, y);
                    }
                }
            }
        }



        /// <summary>
        /// ステージのパネルをシャッフルする
        /// </summary>
        /// <param name="count">シャッフル回数</param>
        /// <remarks>
        /// シャッフル回数を指定しない場合は50回シャッフルする
        /// </remarks>
        public void PanelShuffle(int count = 50)
        {
            List<Vector2Int> moveList = new List<Vector2Int>();
            List<Vector2Int> rotateList = new List<Vector2Int>();

            for (int y = 0; y < Size.y; y++)
            {
                for (int x = 0; x < Size.x; x++)
                {
                    if (Data[y][x].isSlot)
                    {
                        if (Data[y][x].IsMoveable)
                        {
                            moveList.Add(new Vector2Int(x,y));
                        }
                        if (Data[y][x].IsRotatable)
                        {
                            rotateList.Add(new Vector2Int(x,y));
                        }
                    }
                }
            }

            for (int i = 0; i < count; i++)
            {
                Vector2Int pos0 = moveList[UnityEngine.Random.Range(0, moveList.Count)];

                Vector2Int pos1 = moveList[UnityEngine.Random.Range(0, moveList.Count)];

                RoomSetData tmp = Data[pos0.y][pos0.x].RoomSetData;

                Data[pos0.y][pos0.x].RoomSetData = Data[pos1.y][pos1.x].RoomSetData;
                Data[pos1.y][pos1.x].RoomSetData = tmp;
            }

            foreach (Vector2Int pos in rotateList)
            {
                Data[pos.y][pos.x].RoomSetData.Rotation = UnityEngine.Random.Range(0, 4) * 90;
            }
        }


        public SlotData GetPanelSlotData(Vector2Int position)
        {
            if (position.x < 0 || position.y < 0)
            {
                return new SlotData(new Vector2Int(-1, -1));
            }
            if (position.y >= Data.Count || position.x >= Data[Math.Min(position.y, Data.Count - 1)].Count)
            {
                return new SlotData(new Vector2Int(-1, -1));
            }
            return Data[position.y][position.x];
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(StageDataController))]
    public class SlotDataListEditor : Editor
    {
        private class SlotDragData : ScriptableObject
        {
            public RoomSetData roomSetData;
            public Vector2Int position;

            public SlotDragData(Vector2Int position, RoomSetData roomSetData)
            {
                this.roomSetData = roomSetData;
                this.position = position;
            }
        }

        private StageDataController targetList;

        private Vector2Int currentPosition = new Vector2Int(-1, -1);

        private Vector2 scrollPosition;


        private void OnEnable()
        {
            targetList = (StageDataController)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);

            // 基本設定
            EditorGUI.BeginChangeCheck();

            string newNextScene = EditorGUILayout.TextField("nextScene", targetList._nextScene);

            bool newIsShuffle = EditorGUILayout.Toggle("Shuffle", targetList._isShuffle);

            Vector2Int newStart = EditorGUILayout.Vector2IntField("Start", targetList._startPosition);


            // グリッド表示
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("パネルエディター", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            Vector2Int newSize = EditorGUILayout.Vector2IntField("グリッドサイズ", targetList.Size);
            int newButtonSize = EditorGUILayout.IntSlider("ボタンの大きさ", targetList.buttonSize, 10, 100);

            DrawButtons();

            EditorGUILayout.Space();

            bool isRange = currentPosition.x != Mathf.Clamp(currentPosition.x, 0, targetList.Size.x - 1) ||
                           currentPosition.y != Mathf.Clamp(currentPosition.y, 0, targetList.Size.y - 1);

            EditorGUI.BeginDisabledGroup(isRange);

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("currentSlot"),new GUIContent("Current Slot Data"),true
            );

            if (GUILayout.Button("スタート位置に設定",EditorStyles.miniButton))
            {

                targetList.StartPosition = currentPosition;
                newStart = currentPosition;
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.indentLevel--;


            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetList, "Change Grid Size");
                targetList._isShuffle = newIsShuffle;
                targetList._nextScene = newNextScene;
                targetList._size = new Vector2Int(Mathf.Max(1, newSize.x), Mathf.Max(1, newSize.y));
                targetList.buttonSize = newButtonSize;
                targetList._startPosition = newStart;
                ResizeList();

                if (!isRange)
                {
                    targetList._data[currentPosition.y][currentPosition.x] = targetList.currentSlot;

                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        void DrawButtons()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // サイズチェック
            if (targetList._data.Count != targetList.Size.y ||
                (targetList._data.Count > 0 && targetList._data[0].rowData.Count != targetList.Size.x))
            {
                ResizeList();
            }

            FolderArea();

            for (int i = 0; i < targetList.Size.y; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < targetList.Size.x; j++)
                {
                    Vector2Int position = new Vector2Int(j, i);
                    string controlName = $"{(char)((int)'A' + j)}{i}";
                    bool isSelected = position == currentPosition;
                    var data = targetList._data[i][j];

                    // ボタン領域を確保
                    Rect rect = GUILayoutUtility.GetRect(
                        targetList.buttonSize,
                        targetList.buttonSize,
                        GUILayout.ExpandWidth(false)
                    );

                    rect.x += EditorGUI.indentLevel * 15; // インデントを適用

                    

                    GUI.color = isSelected && data.IsNotEmpty ? new Color(2,2,2) : Color.white; // 選択時の色を設定
                    GUI.backgroundColor = data.isSlot ? data.IsNotEmpty ?
                        data.RoomSetData.RoomSetController.roomMode.ToColor() :
                        Color.white :
                        Color.gray;


                    GUIStyle style = new GUIStyle(GUI.skin.button)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        imagePosition = data.isSlot && data.IsNotEmpty ? ImagePosition.ImageOnly : ImagePosition.ImageAbove,
                        normal = { textColor = isSelected ? Color.yellow : Color.white },
                        
                    };

                    GUIContent content;
                    if (data.IsNotEmpty && false)
                    {
                        content = new GUIContent(data.RoomSetData.RoomSetController.roomThumb);
                    }
                    else
                    {
                        content = new GUIContent(controlName);
                    }

                    //GUI.Box(rect, "");

                    DragStart(rect, new Vector2Int(j, i));
                    HandleDragAndDrop(rect, new Vector2Int(j, i));

                    
                    

                    Event evt = Event.current;
                    
                    if (rect.Contains(evt.mousePosition))
                    {
                        if (evt.type == EventType.MouseDown)
                        {
                            if (Event.current.button == 0)
                            {
                                if (Event.current.clickCount == 2)
                                {
                                    if (data.isSlot && data.IsNotEmpty)
                                    {
                                        PrefabStageUtility.OpenPrefab(
                                            AssetDatabase.GetAssetPath(data.RoomSetData.RoomSetController.gameObject)
                                        );
                                    }
                                }
                                else
                                {
                                    currentPosition = position;
                                    targetList.currentSlot = data;
                                }
                            }
                            else if (Event.current.button == 1)
                            {
                                data.isSlot = !data.isSlot;
                            }
                        }
                        
                    }
                    if (isSelected && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
                    {
                        if (data.IsNotEmpty)
                        {
                            data.RoomSetData.RoomSetController = null;
                        }
                    }
                    GUI.SetNextControlName(controlName);
                    GUI.Button(rect, content, style);

                    
                    

                    int padding = 5;
                    Rect thumbRect = new Rect(
                        (int)rect.x + padding,
                        (int)rect.y + padding,
                        (int)rect.width - 2 * padding,
                        (int)rect.height - 2 * padding
                    );
                    
                    // 現在のマトリックスを保存
                    Matrix4x4 matrixBackup = GUI.matrix;
                    if (data.IsNotEmpty)
                    {
                        // 回転の中心点を計算
                        Vector2 pivot = new Vector2(thumbRect.x + thumbRect.width * 0.5f, thumbRect.y + thumbRect.height * 0.5f);
                        GUIUtility.RotateAroundPivot(data.RoomSetData.Rotation, pivot);
                    }

                    
                    if ((data?.RoomSetData?.RoomSetController?.roomThumb != null) && (data?.isSlot ?? false))GUI.DrawTexture(thumbRect, data.RoomSetData.RoomSetController.roomThumb, ScaleMode.ScaleAndCrop);

                    // マトリックスを復元
                    GUI.matrix = matrixBackup;
                }
                EditorGUILayout.EndHorizontal();
            }

            GUI.color = Color.white; // 色をリセット
            GUI.contentColor = Color.white; // テキスト色をリセット
            GUI.backgroundColor = Color.white; // 色をリセット
            EditorGUILayout.EndScrollView();
        }


        void FolderArea()
        {
            Rect rect = GUILayoutUtility.GetRect(
                        targetList.buttonSize * targetList.Size.x,
                        EditorGUIUtility.singleLineHeight,
                        GUILayout.ExpandWidth(false)
                    );
            rect.x += EditorGUI.indentLevel * 15; // インデントを適用
            
            GUI.Box(rect,"フォルダーをインポート");

            Event evt = Event.current;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!rect.Contains(evt.mousePosition))
                        return;

                    // ドラッグ対象を有効に見せる
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var draggedObject in DragAndDrop.objectReferences)
                        {
                            string path = AssetDatabase.GetAssetPath(draggedObject);

                            if (AssetDatabase.IsValidFolder(path))
                            {
                                // フォルダ内の全ファイル（再帰的に）を取得（例：.prefab）
                                string[] guids = AssetDatabase.FindAssets("", new[] { path });

                                foreach (string guid in guids)
                                {
                                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                                    string fileName = Path.GetFileName(assetPath);
                                    Match match = Regex.Match(fileName, @"_([A-Z]+)([0-9]+)\.prefab$");
                                    if (match.Success)
                                    {
                                        int col = (int)match.Groups[1].Value[0] - (int)'A';
                                        int idx = int.Parse(match.Groups[2].Value);

                                        UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);

                                        if (asset is GameObject && asset.GetComponent<RoomSetController>() != null)
                                        {
                                            RoomSetController roomSetController = asset.GetComponent<RoomSetController>();

                                            // 対象のスロットに新しいRoomSetDataを設定
                                            if (idx < targetList._data.Count && col < targetList._data[idx].Count && targetList._data[idx][col].isSlot)
                                            {
                                                targetList._data[idx][col].RoomSetData.RoomSetController = roomSetController;

                                                currentPosition = new Vector2Int(col, idx);
                                                targetList.currentSlot = targetList._data[idx][col];

                                                EditorUtility.SetDirty(targetList);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    evt.Use();
                    break;
            }
        }

        private void DragStart(Rect dragArea, Vector2Int position)
        {
            Event evt = Event.current;

            SlotDragData slotDragData = ScriptableObject.CreateInstance<SlotDragData>();
            slotDragData.position = position;
            slotDragData.roomSetData = targetList._data[position.y][position.x].RoomSetData;

            if (evt.type == EventType.MouseDrag && dragArea.Contains(evt.mousePosition))
            {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences = new UnityEngine.Object[] { slotDragData };
                DragAndDrop.StartDrag("ドラッグ中");
                Event.current.Use();
            }
        }

        private void HandleDragAndDrop(Rect dropArea, Vector2Int position)
        {
            Event evt = Event.current;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        return;

                    // ドラッグ対象を有効に見せる
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var draggedObject in DragAndDrop.objectReferences)
                        {
                            string path = AssetDatabase.GetAssetPath(draggedObject);

                            if (draggedObject == null) continue;
                            if (draggedObject is SlotDragData slotDragData)
                            {
                                targetList._data[slotDragData.position.y][slotDragData.position.x].RoomSetData = targetList._data[position.y][position.x].RoomSetData;
                                targetList._data[position.y][position.x].RoomSetData = slotDragData.roomSetData;

                                currentPosition = position;
                                targetList.currentSlot = targetList._data[position.y][position.x];

                                EditorUtility.SetDirty(targetList);
                                continue;
                            }
                            else if (draggedObject is GameObject && draggedObject.GetComponent<RoomSetController>() != null)
                            {
                                // ドラッグされたオブジェクトがRoomSetControllerを持つ場合
                                RoomSetController roomSetController = draggedObject.GetComponent<RoomSetController>();

                                // 対象のスロットに新しいRoomSetDataを設定
                                if (targetList._data[position.y][position.x].isSlot)
                                {
                                    targetList._data[position.y][position.x].RoomSetData.RoomSetController = roomSetController;

                                    currentPosition = position;
                                    targetList.currentSlot = targetList._data[position.y][position.x];

                                    EditorUtility.SetDirty(targetList);
                                }
                            }
                        }
                        GUI.FocusControl(null);
                        GUIUtility.hotControl = 0;
                    }
                    

                    evt.Use();
                    break;
            }
        }

        private void ResizeList()
        {
            // 行のリサイズ
            while (targetList._data.Count < targetList.Size.y)
            {
                var newRow = new SlotDataList();
                for (int j = 0; j < targetList.Size.x; j++)
                {
                    newRow.rowData.Add(new SlotData(new Vector2Int(j, targetList._data.Count)));
                }
                targetList._data.Add(newRow);
            }

            while (targetList._data.Count > targetList.Size.y)
            {
                targetList._data.RemoveAt(targetList._data.Count - 1);
            }

            // 列のリサイズ
            for (int i = 0; i < targetList._data.Count; i++)
            {
                while (targetList._data[i].Count < targetList.Size.x)
                {
                    targetList._data[i].rowData.Add(new SlotData(new Vector2Int(targetList._data[i].Count, i)));
                }

                while (targetList._data[i].Count > targetList.Size.x)
                {
                    targetList._data[i].rowData.RemoveAt(targetList._data[i].Count - 1);
                }
            }

            EditorUtility.SetDirty(targetList);
        }

    }
    #endif
}
