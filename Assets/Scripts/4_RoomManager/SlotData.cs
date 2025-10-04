using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Rooms.Auto;
using Rooms.RoomSystem;

namespace Rooms.PanelSystem
{
    /// <summary>
    /// 部屋とパネルのデータを管理するクラス
    /// </summary>
    [Serializable] public class RoomSetData
    {
        [SerializeField] private RoomSetController _roomSetController;
        public RoomSetController RoomSetController
        {
            get => _roomSetController;
            set
            {
                _roomSetController = value;
            }
        }

        [SerializeField] private PanelDataController _panelDataController;
        public PanelDataController PanelDataController
        {
            get => _panelDataController;
            set
            {
                _panelDataController = value;
                _panelDataController.roomSetData = this;
            }
        }

        [SerializeField] private RoomDataController _roomDataController;
        public RoomDataController RoomDataController
        {
            get => _roomDataController;
            set
            {
                _roomDataController = value;
                _roomDataController.roomSetData = this;
            }
        }


        [SerializeField] private int _rotate = 0;
        /// <summary>
        /// 部屋の回転角度を90度単位で設定するプロパティ。端数は自動で切り捨てられ、0から360の範囲に正規化される。
        /// 回転方向は時計回りで、0が北、90が東、180が南、270が西を表す。
        /// </summary>
        public int Rotation
        {
            get => _rotate*90;
            set
            {
                _rotate = ((value/90)+4)%4;
            }
        }


        private Vector2Int _position;
        /// <summary>
        /// 部屋の位置を示す2D座標、原則自動で設定される。
        /// </summary>
        public Vector2Int Position
        {
            get => _position;
            set => _position = value;
        }




        /// <summary>
        /// 部屋の回転角度を取得するプロパティ。値は0から360の範囲に正規化される。
        /// </summary>
        /// <param name="yOffset">部屋のY座標オフセット。デフォルトは0。</param>
        /// <returns>部屋の回転角度を表すQuaternion。0が北、90が東、180が南、270が西を表す。</returns>
        public Vector3 ToWorldPosition(float yOffset = 0)
        {
            return new Vector3(Position.x+1, yOffset, -(Position.y+1));
        }
    }



    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(RoomSetData))]
    public class RoomSetDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty roomSetControllerProperty = property.FindPropertyRelative("_roomSetController");

            SerializedProperty panelDataControllerProperty = property.FindPropertyRelative("_panelDataController");
            SerializedProperty roomDataControllerProperty = property.FindPropertyRelative("_roomDataController");

            SerializedProperty rotateProperty = property.FindPropertyRelative("_rotate");



            EditorGUI.BeginProperty(position, label, property);


            EditorGUI.PropertyField(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                roomSetControllerProperty, new GUIContent("部屋セット"), true);

            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            
            if (Application.isPlaying)
            {
                EditorGUI.PropertyField(
                    new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                    panelDataControllerProperty, new GUIContent("パネル"), true);
                
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;


                EditorGUI.PropertyField(
                    new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                    roomDataControllerProperty, new GUIContent("部屋"), true);
                
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }


            rotateProperty.intValue = EditorGUI.IntSlider(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("回転"), rotateProperty.intValue*90,0,270)/90;


            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (Application.isPlaying)
            {
                // プレイ中の場合、パネルと部屋のデータも表示する。
                return EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 3;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 1;
            }
        }

    }
    #endif



    /// <summary>
    /// スロットのデータを管理するクラス.
    /// </summary>
    [Serializable] public class SlotData
    {
        /// <summary>
        /// スロットが空かどうかを示すフラグ
        /// </summary>
        public bool isSlot = true;

        public RoomSetData _roomSetData;
        public RoomSetData RoomSetData
        {
            get => _roomSetData;
            set
            {
                _roomSetData = value;
                if (_roomSetData != null)
                {
                    _roomSetData.Position = Position;
                }
            }
        }

        [SerializeField]
        public Vector2Int _position;
        /// <summary>
        /// スロットの位置を示す2D座標,原則自動で設定される.
        /// </summary>
        public Vector2Int Position
        {
            get => _position;
            set
            {
                _position = value;
                if (RoomSetData != null)
                {
                    RoomSetData.Position = value;
                }
            }
        }

        /// <summary>
        /// スロットが空かどうかを示すプロパティ.
        /// IsEmptyは、スロットであり、かつroomSetControllerがnullである場合にtrueを返す.
        /// </summary>
        public bool IsEmpty => isSlot && RoomSetData?.RoomSetController == null;
        /// <summary>
        /// スロットが空でないかどうかを示すプロパティ.
        /// IsNotEmptyは、スロットであり、かつroomSetControllerがnullでない場合にtrueを返す.
        /// </summary>
        public bool IsNotEmpty => isSlot && RoomSetData?.RoomSetController != null;

        public bool IsMoveable => IsNotEmpty ? RoomSetData.RoomSetController.roomMode.CanMove() : isSlot;
        public bool IsHoldable => IsNotEmpty && RoomSetData.RoomSetController.roomMode.CanHold();
        public bool IsRotatable => IsNotEmpty && RoomSetData.RoomSetController.roomMode.CanRotate();

        public SlotData(Vector2Int position)
        {
            Position = position;
            this.RoomSetData = new RoomSetData
            {
                Position = position,
                RoomSetController = null // 初期状態ではnull
            };
        }

        public SlotData()
        {
            isSlot = false;
        }

        public Color ToCursorColor()
        {
            if (IsEmpty) return Color.gray;
            return RoomSetData.RoomSetController.roomMode.ToColor();
        }

        public Vector3 ToWorldPosition(float yOffset = 0)
        {
            return new Vector3(Position.x, yOffset, -Position.y);
        }
    }


    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SlotData))]
    public class SlotDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty isSlotProperty = property.FindPropertyRelative("isSlot");
            SerializedProperty roomSetDataProperty = property.FindPropertyRelative("_roomSetData");
            SerializedProperty positionProperty = property.FindPropertyRelative("_position");

            EditorGUI.LabelField(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                label);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                isSlotProperty, new GUIContent("Is Slot"), true);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.BeginDisabledGroup(!isSlotProperty.boolValue);

            EditorGUI.PropertyField(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                roomSetDataProperty, new GUIContent("Room Set Data"), true);
            position.y += EditorGUI.GetPropertyHeight(roomSetDataProperty) + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.BeginDisabledGroup(true);

            EditorGUI.PropertyField(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                positionProperty, new GUIContent("Position"), true);

            EditorGUI.EndDisabledGroup();

            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 3
                + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_roomSetData"))
                + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_position")) ;
        }

    }
    #endif



    [Serializable] public class SlotDataList
    {
        public SlotData this[int index]
        {
            get => rowData[index];
            set => rowData[index] = value;
        }
        
        public List<SlotData> rowData = new List<SlotData>();

        public int Count => rowData.Count;
    }
}
