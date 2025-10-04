using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Rooms.Auto;
using UnityEngine.AddressableAssets;
using Rooms.PanelSystem;
using UnityEngine.ProBuilder.Shapes;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Rooms.DoorSystem
{
    public enum DoorLockType
    {
        Key = 0,
        Item = 1,
        Color = 2,
        Event = 3,
    }

    public enum DoorLockColor
    {
        None = -1,
        Red = 0,
        Blue = 1,
        Yellow = 2,
        Green = 3,
        Orange = 4,
        Purple = 5,
        Black = 6,
    }

    static class DoorLockEnum
    {
        public static Vector3Int ToCMY(this DoorLockColor color)
        {
            switch (color)
            {
                case DoorLockColor.Red:
                    return new Vector3Int(0, 1, 0);
                case DoorLockColor.Blue:
                    return new Vector3Int(1, 0, 0);
                case DoorLockColor.Yellow:
                    return new Vector3Int(0, 0, 1);
                case DoorLockColor.Green:
                    return new Vector3Int(1, 0, 1);
                case DoorLockColor.Orange:
                    return new Vector3Int(0, 1, 1);
                case DoorLockColor.Purple:
                    return new Vector3Int(1, 1, 0);
                case DoorLockColor.Black:
                    return new Vector3Int(1, 1, 1);
                default:
                    return new Vector3Int(0, 0, 0);
            }
        }

        public static Color ToMaterialColor(this DoorLockColor color)
        {
            switch (color)
            {
                case DoorLockColor.Blue:
                    return new Color(0.1f, 0.2f, 0.7f);
                case DoorLockColor.Red:
                    return new Color(0.7f, 0.1f, 0.2f);
                case DoorLockColor.Yellow:
                    return new Color(0.9f, 0.8f, 0.1f);
                case DoorLockColor.Purple:
                    return new Color(0.4f, 0.1f, 0.5f);
                case DoorLockColor.Green:
                    return new Color(0.1f, 0.7f, 0.1f);
                case DoorLockColor.Orange:
                    return new Color(0.9f, 0.6f, 0.1f);
                case DoorLockColor.Black:
                    return new Color(0, 0, 0);
                default: return Color.white;
            }
        }

        public static DoorLockColor ToDoorLockColor(this Vector3Int cmy)
        {
            Vector3Int color = new Vector3Int(Mathf.Clamp(cmy.x, 0, 1), Mathf.Clamp(cmy.y, 0, 1), Mathf.Clamp(cmy.z, 0, 1));
            if (color == new Vector3Int(0, 1, 0)) return DoorLockColor.Red;
            if (color == new Vector3Int(1, 0, 0)) return DoorLockColor.Blue;
            if (color == new Vector3Int(0, 0, 1)) return DoorLockColor.Yellow;
            if (color == new Vector3Int(1, 0, 1)) return DoorLockColor.Green;
            if (color == new Vector3Int(0, 1, 1)) return DoorLockColor.Orange;
            if (color == new Vector3Int(1, 1, 0)) return DoorLockColor.Purple;
            if (color == new Vector3Int(1, 1, 1)) return DoorLockColor.Black;
            return DoorLockColor.None;
        }

        public static Material ToMaterial(this DoorLockColor color)
        {
            PanelDoorMaterial doorMaterial = RoomsAssetsManager.PanelAssets.doorMaterial;
            switch (color)
            {
                case DoorLockColor.Red:
                    return doorMaterial.Red;
                case DoorLockColor.Blue:
                    return doorMaterial.Blue;
                case DoorLockColor.Yellow:
                    return doorMaterial.Yellow;
                case DoorLockColor.Green:
                    return doorMaterial.Green;
                case DoorLockColor.Orange:
                    return doorMaterial.Orange;
                case DoorLockColor.Purple:
                    return doorMaterial.Purple;
                case DoorLockColor.Black:
                    return doorMaterial.Black;
                default:
                    return null;
            }
        }

    }



    [Serializable]
    public class EventData
    {
        [SerializeField] private string _eventName;
        public string EventName
        {
            get => _eventName;
            set => _eventName = value;
        }

        [SerializeField] private EventObjectsHolder _eventObject;
        public EventObjectsHolder EventObject
        {
            get => _eventObject;
            set => _eventObject = value;
        }
    }


    [Serializable]
    public class KeyData
    {
        [SerializeField] private string _keyName;
        public string KeyName
        {
            get => _keyName;
            set => _keyName = value;
        }

        [SerializeField] private Color _keyHoleColor = Color.white;
        public Color KeyHoleColor
        {
            get => _keyHoleColor;
            set => _keyHoleColor = value;
        }
    }




    [Serializable]
    public class DoorLockData
    {
        [SerializeField] private DoorLockType _lockType;
        public DoorLockType LockType
        {
            get => _lockType;
            set => _lockType = value;
        }

        [SerializeField] private bool _isLocked = true;
        public bool IsLocked
        {
            get => _isLocked;
            set => _isLocked = value;
        }

        [SerializeField] private bool _isActive = true;
        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }


        [SerializeField] private string _itemData;
        [SerializeField] private DoorLockColor _colorData;
        [SerializeField] private EventData _eventData;
        [SerializeField] private KeyData _keyData;


        public object Data
        {
            get
            {
                switch (_lockType)
                {
                    case DoorLockType.Key:
                        return _keyData;
                    case DoorLockType.Item:
                        return _itemData;
                    case DoorLockType.Color:
                        return _colorData;
                    case DoorLockType.Event:
                        return _eventData;
                    default:
                        return null;
                }
            }
        }

        public object LockVal
        {
            get
            {
                switch (_lockType)
                {
                    case DoorLockType.Key:
                        return _keyData.KeyName;
                    case DoorLockType.Item:
                        return _itemData;
                    case DoorLockType.Color:
                        return _colorData;
                    case DoorLockType.Event:
                        return _eventData.EventName;
                    default:
                        return null;
                }
            }
        }

        [SerializeField] private AudioClip _lockSound;
        public AudioClip LockSound
        {
            get => _lockSound;
            set => _lockSound = value;
        }

        [SerializeField] private AudioClip _unlockSound;
        public AudioClip UnlockSound
        {
            get => _unlockSound;
            set => _unlockSound = value;
        }


        public DoorLockData(DoorLockType lockType, object data, bool isLocked = true, bool isActive = true)
        {
            _lockType = lockType;
            _isLocked = isLocked;
            _isActive = isActive;

            switch (lockType)
            {
                case DoorLockType.Key:
                    _keyData = data as KeyData;
                    break;
                case DoorLockType.Item:
                    _itemData = data as string;
                    break;
                case DoorLockType.Color:
                    _colorData = (DoorLockColor)data;
                    break;
                case DoorLockType.Event:
                    _eventData = data as EventData;
                    break;
                default:
                    break;
            }
        }


        public Material ToMaterial()
        {
            Material material;

            switch (LockType)
            {
                case DoorLockType.Item:
                    material = RoomsAssetsManager.PanelAssets.doorMaterial.Lock;
                    break;

                case DoorLockType.Key:
                    material = RoomsAssetsManager.PanelAssets.doorMaterial.Key;
                    if (Application.isPlaying)
                    {
                        material.enableInstancing = true;
                        material = new Material(material);
                        material.SetColor("_Color", _keyData.KeyHoleColor);
                        material.SetColor("_EmissionOfColor", _keyData.KeyHoleColor);
                    }
                    break;

                case DoorLockType.Color:
                    material = _colorData.ToMaterial();
                    break;

                case DoorLockType.Event:
                    if (_eventData.EventName == "ConfineTrap")
                    {
                        material = RoomsAssetsManager.PanelAssets.doorMaterial.Danger;
                    }
                    else
                    {
                        material = RoomsAssetsManager.PanelAssets.doorMaterial.Lock;
                    }
                    break;

                default:
                    material = RoomsAssetsManager.PanelAssets.doorMaterial.Lock;
                    break;
            }

            return material;
        }
    }


#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DoorLockData))]
    public class DoorLockDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position.height = EditorGUIUtility.singleLineHeight;

            property.isExpanded = EditorGUI.Foldout(
                position, property.isExpanded, label, true, EditorStyles.foldout
            );

            if (property.isExpanded)
            {
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(
                    position, property.FindPropertyRelative("_isLocked"), new GUIContent("初期状態")
                );

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(
                    position, property.FindPropertyRelative("_isActive"), new GUIContent("有効")
                );

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                EditorGUI.PropertyField(
                    position, property.FindPropertyRelative("_lockType"), new GUIContent("ロック条件")
                );

                switch (property.FindPropertyRelative("_lockType").enumValueIndex)
                {
                    case (int)DoorLockType.Key:
                        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                        EditorGUI.PropertyField(
                            position, property.FindPropertyRelative("_keyData").FindPropertyRelative("_keyName"), new GUIContent("鍵の名前")
                        );

                        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                        EditorGUI.PropertyField(
                            position, property.FindPropertyRelative("_keyData").FindPropertyRelative("_keyHoleColor"), new GUIContent("鍵の名前")
                        );
                        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        break;

                    case (int)DoorLockType.Item:
                        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                        EditorGUI.PropertyField(
                            position, property.FindPropertyRelative("_itemData"), new GUIContent("アイテム名")
                        );

                        position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_itemData")) + EditorGUIUtility.standardVerticalSpacing;
                        break;

                    case (int)DoorLockType.Color:
                        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                        EditorGUI.PropertyField(
                            position, property.FindPropertyRelative("_colorData"), new GUIContent("色")
                        );

                        position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_colorData")) + EditorGUIUtility.standardVerticalSpacing;
                        break;

                    case (int)DoorLockType.Event:
                        SerializedProperty eventObjectProperty = property.FindPropertyRelative("_eventData").FindPropertyRelative("_eventObject");

                        if (eventObjectProperty.objectReferenceValue != null)
                        {
                            EventObjectsHolder doorHolder = eventObjectProperty.objectReferenceValue as EventObjectsHolder;

                            if (doorHolder != null)
                            {
                                if (doorHolder.EventName != "")property.FindPropertyRelative("_eventData").FindPropertyRelative("_eventName").stringValue = doorHolder.EventName;

                                if (doorHolder.EventLockSound != null)property.FindPropertyRelative("_lockSound").objectReferenceValue = doorHolder.EventLockSound;
                                if (doorHolder.EventUnlockSound != null)property.FindPropertyRelative("_unlockSound").objectReferenceValue = doorHolder.EventUnlockSound;

                                EditorGUI.BeginDisabledGroup(true);
                            }
                        }

                        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                        EditorGUI.PropertyField(
                            position, property.FindPropertyRelative("_eventData").FindPropertyRelative("_eventName"), new GUIContent("イベント名")
                        );

                        EditorGUI.EndDisabledGroup();

                        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(
                            position, property.FindPropertyRelative("_eventData").FindPropertyRelative("_eventObject"), new GUIContent("イベントオブジェクト")
                        );
                        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                        break;

                    default:
                        break;
                }



                //position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(
                    position, property.FindPropertyRelative("_lockSound"), new GUIContent("ロック音")
                );

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(
                    position, property.FindPropertyRelative("_unlockSound"), new GUIContent("アンロック音")
                );
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded && property.FindPropertyRelative("_lockType") != null)
            {
                int step = 6;
                switch (property.FindPropertyRelative("_lockType").enumValueIndex)
                {
                    case (int)DoorLockType.Key:
                        step += 2; // Key type has 5 fields
                        break;
                    case (int)DoorLockType.Item:
                        step += 1; // Item type has 4 fields
                        break;
                    case (int)DoorLockType.Color:
                        step += 1; // Color type has 4 fields
                        break;
                    case (int)DoorLockType.Event:
                        step += 2; // Event type has 5 fields
                        break;
                    default:
                        step += 0; // Default case has 3 fields
                        break;
                }
                return EditorGUIUtility.singleLineHeight * step + EditorGUIUtility.standardVerticalSpacing * (step - 1);
            }
            return EditorGUIUtility.singleLineHeight;
        }
    }
#endif



    [Serializable]
    public class DoorLockDataList
    {
        [SerializeField]
        private List<DoorLockData> _locks = new List<DoorLockData>();


        public DoorLockData this[int index]
        {
            get => _locks[index];
            set
            {
                _locks[index] = value;
                UpdateLock();
            }
        }

        public IEnumerator<DoorLockData> GetEnumerator()
        {
            foreach (var lockData in _locks)
            {
                yield return lockData;
            }
        }


        public static DoorLockDataList operator +(DoorLockDataList list1, DoorLockDataList list2)
        {
            List<DoorLockData> combinedList = new List<DoorLockData>(list1._locks);
            combinedList.AddRange(list2._locks);
            return new DoorLockDataList(combinedList);
        }



        public int Count => _locks.Count;

        public void Add(DoorLockData data)
        {
            _locks.Add(data);
        }

        public void Clear()
        {
            _locks.Clear();
            IsLocked = false;
        }

        public event Action LockedEvent;
        public void OnLockedEvent() => LockedEvent?.Invoke();

        public event Action UnLockedEvent;
        public void OnUnLockedEvent() => UnLockedEvent?.Invoke();

        public event Action<DoorLockData> LockStateChangedEvent;
        public void OnLockStateChangedEvent(DoorLockData data) => LockStateChangedEvent?.Invoke(data);



        private bool _isLocked = false;
        public bool IsLocked
        {
            get => _isLocked;
            set => _isLocked = value;
        }

        private void UpdateLock()
        {
            bool flag = false;

            for (int i = 0; i < Count; i++)
            {
                if (this[i].IsLocked)
                {
                    flag = true;
                    break;
                }
            }

            if (!IsLocked && flag) OnLockedEvent();
            else if (IsLocked && !flag) OnUnLockedEvent();

            IsLocked = flag;
        }



        public void ResetLock()
        {
            for (int i = 0; i < Count; i++)
            {
                this[i].IsActive = true;

                if (this[i].LockType == DoorLockType.Color)
                {
                    this[i].IsLocked = RoomsManager.CurrentDoorLockColor == (DoorLockColor)this[i].LockVal;
                }
            }
            UpdateLock();
        }


        public bool SetActive(DoorLockType type, object key, bool state = true)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].LockType == type && (this[i].LockType == DoorLockType.Color ? (DoorLockColor)this[i].LockVal == (DoorLockColor)key:(string)this[i].LockVal == (string)key) && this[i].IsActive != state)
                {
                    this[i].IsActive = state;
                    return true;
                }
            }
            return false;
        }

        public bool CanUnLock(DoorLockType type, object key)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].LockType == type && (this[i].LockType == DoorLockType.Color ? (DoorLockColor)this[i].LockVal == (DoorLockColor)key:(string)this[i].LockVal == (string)key))
                    return this[i].IsLocked;
            }
            return false;
        }

        public bool SetLock(DoorLockType type, object key, bool state = false)
        {
            bool flag = false;
            for (int i = 0; i < Count; i++)
            {
                if (this[i].LockType == type && (this[i].LockType == DoorLockType.Color ? (DoorLockColor)this[i].LockVal == (DoorLockColor)key:(string)this[i].LockVal == (string)key))
                {
                    this[i].IsLocked = state;
                    flag = true;
                    OnLockStateChangedEvent(this[i]);
                    UpdateLock();
                }
            }
            return flag;
        }

        public bool IsExist(DoorLockType type, object key)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].LockType == type && (this[i].LockType == DoorLockType.Color ? (DoorLockColor)this[i].LockVal == (DoorLockColor)key:(string)this[i].LockVal == (string)key)) return true;
            }
            return false;
        }

        public bool IsExist(DoorLockType type)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].LockType == type) return true;
            }
            return false;
        }

        public DoorLockData[] GetLockDatas(DoorLockType type)
        {
            return _locks.Where(lockData => lockData.LockType == type).ToArray();
        }

        public DoorLockColor GetLockColor()
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].LockType == DoorLockType.Color) return (DoorLockColor)this[i].LockVal;
            }
            return DoorLockColor.None;
        }

        public Color GetKeyColor(int index = 0)
        {
            int c = 0;
            for (int i = 0; i < Count; i++)
            {
                if (this[i].LockType == DoorLockType.Key)
                {
                    if (index == c)
                    {
                        return ((KeyData)this[i].LockVal).KeyHoleColor;
                    }
                    else
                    {
                        c++;
                    }
                }
            }
            return Color.white;
        }

        public bool IsKeyLock(int index = 0)
        {
            int c = 0;
            for (int i = 0; i < Count; i++)
            {
                if (this[i].LockType == DoorLockType.Key)
                {
                    if (index == c)
                    {
                        return true;
                    }
                    else
                    {
                        c++;
                    }
                }
            }
            return false;
        }


        public Material ToMaterial()
        {
            if (Count == 0) return RoomsAssetsManager.PanelAssets.doorMaterial.Open;

            else return this[0].ToMaterial();
        }


        public DoorLockDataList()
        {

        }

        public DoorLockDataList(List<DoorLockData> list)
        {
            Clear();
            Vector3Int currentColor = Vector3Int.zero;
            foreach (DoorLockData data in list)
            {
                switch (data.LockType)
                {
                    case DoorLockType.Color:
                        currentColor += ((DoorLockColor)data.LockVal).ToCMY();
                        break;
                    default:
                        Add(data);
                        break;
                }
            }
            if (currentColor.ToDoorLockColor() != DoorLockColor.None)
            {
                Add(new DoorLockData(
                    DoorLockType.Color,
                    currentColor.ToDoorLockColor(),
                    RoomsManager.CurrentDoorLockColor != currentColor.ToDoorLockColor(),
                    true
                ));
            }

            for (int i = 0; i < Count; i++)
            {
                if (this[i].LockType == DoorLockType.Item) _isLocked = true;
            }
            UpdateLock();
        }
    }


#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DoorLockDataList))]
    public class DoorLockDataListDrawer : PropertyDrawer
    {
        private Dictionary<string, ReorderableList> reorderableLists = new Dictionary<string, ReorderableList>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // 配列プロパティを取得
            SerializedProperty arrayProperty = property.FindPropertyRelative("_locks");
            if (arrayProperty == null || !arrayProperty.isArray) {
                EditorGUI.HelpBox(position, arrayProperty == null ? "Array is not found! 0" : "Array is not found! 1", MessageType.Error);
                return;
            }

            // ReorderableListの初期化
            string key = property.propertyPath;
            if (!reorderableLists.ContainsKey(key)) {
                reorderableLists[key] = CreateReorderableList(arrayProperty, label);
            }

            position.x += EditorGUI.indentLevel * 15; // ラベルの幅を考慮して位置を調整
            position.width -= EditorGUI.indentLevel * 15 - 1; // ラベルの幅を考慮して幅を調整

            // リストを描画
            reorderableLists[key].DoList(position);
        }

        private ReorderableList CreateReorderableList(SerializedProperty arrayProperty, GUIContent label) {
            var list = new ReorderableList(
                arrayProperty.serializedObject,
                arrayProperty,
                draggable: true,
                displayHeader: true,
                displayAddButton: true,
                displayRemoveButton: true
            );

            list.drawHeaderCallback = (Rect rect) => {
                rect.x -= EditorGUI.indentLevel * 15; // ラベルの幅を考慮して位置を調整
                //rect.width += EditorGUI.indentLevel * 15; // ラベルの幅を考慮して幅を調整

                EditorGUI.LabelField(rect, "ロックデータ", EditorStyles.boldLabel);
            };

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                SerializedProperty element = arrayProperty.GetArrayElementAtIndex(index);
                rect.y += 2; // Adjust the y position to avoid overlap with the header
                rect.height = EditorGUI.GetPropertyHeight(element, true);
                rect.x += 15; // Add some padding to the left
                rect.width -= 15; // Adjust width to fit the label and field

                EditorGUI.PropertyField(rect, element, new GUIContent("データ"+index), true);
            };

            list.onAddCallback = (ReorderableList l) => {
                arrayProperty.arraySize++;

                arrayProperty.serializedObject.ApplyModifiedProperties(); // 先に反映してからアクセス
                SerializedProperty newElement = arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize-1);
                if (newElement != null) {
                    newElement.FindPropertyRelative("_lockType").enumValueIndex = 0; // デフォルトのロックタイプを設定
                    newElement.FindPropertyRelative("_isLocked").boolValue = true; // デフォルトでロック状態にする
                    newElement.FindPropertyRelative("_isActive").boolValue = true; // デフォルトで有効状態にする
                    arrayProperty.serializedObject.ApplyModifiedProperties(); // 先に反映してからアクセス
                }
            };


            list.elementHeightCallback = (int index) => {
                return EditorGUI.GetPropertyHeight(arrayProperty.GetArrayElementAtIndex(index)) + 4;
            };

            return list;
        }



        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if (reorderableLists.TryGetValue(property.propertyPath, out var list)) {
                return list.GetHeight();
            }
            return EditorGUIUtility.singleLineHeight * 2; // エラー表示用の余裕
        }

    }
#endif

}
