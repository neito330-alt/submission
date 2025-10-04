using UnityEngine;
using System;
using System.Collections.Generic;
using Rooms.DoorSystem;
using Rooms.Auto;
using UnityEngine.ProBuilder.Shapes;
using System.Linq;
using Rooms.PanelSystem;
using System.Net.WebSockets;
using System.Diagnostics;
using Rooms.RoomSystem;
using UnityEngine.UI;
using Unity.Properties;
using System.Reflection;










#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Rooms.Auto
{
    public enum DoorHorizontalPosition
    {
        Left,
        Middle,
        Right,
    }

    public enum DoorVerticalPosition
    {
        Top,
        Middle,
        Bottom,
    }

    public enum DoorWallDirection
    {
        North,
        East,
        South,
        West,
    }

    public static class EnumExtensions
    {
        public static string ToHeadName(this DoorHorizontalPosition position)
        {
            switch (position)
            {
                case DoorHorizontalPosition.Left:
                    return "L";
                case DoorHorizontalPosition.Middle:
                    return "M";
                case DoorHorizontalPosition.Right:
                    return "R";
                default:
                    throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }
        }

        public static string ToHeadName(this DoorWallDirection direction)
        {
            switch (direction)
            {
                case DoorWallDirection.North:
                    return "N";
                case DoorWallDirection.East:
                    return "E";
                case DoorWallDirection.South:
                    return "S";
                case DoorWallDirection.West:
                    return "W";
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }


        public static int ToShapeKeyInt(this DoorHorizontalPosition position)
        {
            switch (position)
            {
                case DoorHorizontalPosition.Right:
                    return 0;
                case DoorHorizontalPosition.Middle:
                    return 1;
                case DoorHorizontalPosition.Left:
                    return 2;
                default:
                    throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }
        }

        public static int ToShapeKeyInt(this DoorWallDirection direction)
        {
            switch (direction)
            {
                case DoorWallDirection.North:
                    return 0;
                case DoorWallDirection.West:
                    return 3;
                case DoorWallDirection.South:
                    return 6;
                case DoorWallDirection.East:
                    return 9;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public static int ToMaterialIndex(this DoorHorizontalPosition position)
        {
            switch (position)
            {
                case DoorHorizontalPosition.Right:
                    return 1;
                case DoorHorizontalPosition.Middle:
                    return 0;
                case DoorHorizontalPosition.Left:
                    return 2;
                default:
                    throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }
        }

        public static int ToMaterialIndex(this DoorWallDirection direction)
        {
            switch (direction)
            {
                case DoorWallDirection.North:
                    return 12;
                case DoorWallDirection.West:
                    return 9;
                case DoorWallDirection.South:
                    return 6;
                case DoorWallDirection.East:
                    return 3;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public static int ToMeshValue(this DoorHorizontalPosition position)
        {
            switch (position)
            {
                case DoorHorizontalPosition.Right:
                    return 1;
                case DoorHorizontalPosition.Middle:
                    return 2;
                case DoorHorizontalPosition.Left:
                    return 4;
                default:
                    throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }
        }

        public static float ToDoorRotate(this DoorWallDirection direction)
        {
            switch (direction)
            {
                case DoorWallDirection.North:
                    return 90f;
                case DoorWallDirection.East:
                    return 180f;
                case DoorWallDirection.South:
                    return 270f;
                case DoorWallDirection.West:
                    return 0f;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public static Vector3 ToDoorPosition(this DoorVerticalPosition position)
        {
            switch (position)
            {
                case DoorVerticalPosition.Top:
                    return new Vector3(0, 4.24f, 0);
                case DoorVerticalPosition.Middle:
                    return new Vector3(0, 0, 0);
                case DoorVerticalPosition.Bottom:
                    return new Vector3(0, -2.4f, 0);
                default:
                    throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }
        }

        public static Vector3 ToDoorPosition1(this DoorWallDirection direction)
        {
            switch (direction)
            {
                case DoorWallDirection.North:
                    return new Vector3(0, 0, -4f);
                case DoorWallDirection.East:
                    return new Vector3(-4f, 0, 0);
                case DoorWallDirection.South:
                    return new Vector3(0, 0, 4f);
                case DoorWallDirection.West:
                    return new Vector3(4f, 0, 0);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public static Vector3 ToDoorPosition2(this DoorWallDirection direction)
        {
            switch (direction)
            {
                case DoorWallDirection.North:
                    return new Vector3(-1f, 0, 0);
                case DoorWallDirection.East:
                    return new Vector3(0, 0, 1f);
                case DoorWallDirection.South:
                    return new Vector3(1f, 0, 0);
                case DoorWallDirection.West:
                    return new Vector3(0, 0, -1f);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public static float ToDoorPosition(this DoorHorizontalPosition position)
        {
            switch (position)
            {
                case DoorHorizontalPosition.Left:
                    return -2.5f;
                case DoorHorizontalPosition.Middle:
                    return 0f;
                case DoorHorizontalPosition.Right:
                    return 2.5f;
                default:
                    throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }
        }

        }


    [Serializable]
    public class DoorPositionData
    {
        public DoorWallDirection direction = DoorWallDirection.North;
        public DoorVerticalPosition verticalPosition = DoorVerticalPosition.Middle;
        public DoorHorizontalPosition horizontalPosition = DoorHorizontalPosition.Middle;

        public Vector3 ToVector3()
        {
            return direction.ToDoorPosition1() + verticalPosition.ToDoorPosition() + direction.ToDoorPosition2() * horizontalPosition.ToDoorPosition();
        }

        public int ToMaterialIndex()
        {
            return direction.ToMaterialIndex() + horizontalPosition.ToMaterialIndex();
        }
    }


    [Serializable]
    public class DoorData
    {
        [Serializable]
        public class DoorParameter
        {
            public InteractiveDoor door;
            [SerializeField, Range(0, 20)]
            public int level = 5;
            public Material doorBodyMaterial;
            public Material wallMaterial;
            [SerializeField]
            public DoorLockDataList doorLockList;
            public DoorLockDataList LockData
            {
                get
                {
                    return doorLockList;
                }
            }
        }

        #if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(DoorParameter))]
        public class DoorParameterDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                GUIContent content = EditorGUI.BeginProperty(position, label, property);

                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(position, content, EditorStyles.boldLabel);

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("door"), new GUIContent("Door Prefab"));

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("level"), new GUIContent("Level"));

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("doorBodyMaterial"), new GUIContent("ドアマテリアル"));

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("wallMaterial"), new GUIContent("壁マテリアル"));

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                if (property.FindPropertyRelative("doorLockList") != null)
                {
                    position.height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("doorLockList"));
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("doorLockList"), new GUIContent("ロックデータ"));
                }

                EditorGUI.EndProperty();
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                if (property == null || property.FindPropertyRelative("doorLockList") == null)
                {
                    return EditorGUIUtility.singleLineHeight * 5 + EditorGUIUtility.standardVerticalSpacing * 5;
                }
                return EditorGUIUtility.singleLineHeight * 5 + EditorGUIUtility.standardVerticalSpacing * 5 + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("doorLockList"));
            }
        }
        #endif

        [SerializeField]
        public bool isDoor = false;

        [SerializeField]
        public DoorParameter doorData;
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DoorData))]
    public class DoorDataDrawer : PropertyDrawer
    {
        bool isFoldout = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            property.isExpanded = EditorGUI.Foldout(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                property.isExpanded, label, true, EditorStyles.foldoutHeader);

            if (!isFoldout)
            {
                property.isExpanded = true;
                isFoldout = true;
            }

            if (property.isExpanded)
            {
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.FindPropertyRelative("isDoor"), new GUIContent("ドア"));

                EditorGUI.BeginDisabledGroup(!property.FindPropertyRelative("isDoor").boolValue);

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                SerializedProperty doorDataProperty = property.FindPropertyRelative("doorData");

                EditorGUI.PropertyField(
                    new Rect(position.x, position.y, position.width, EditorGUI.GetPropertyHeight(doorDataProperty)),
                    doorDataProperty,
                    new GUIContent("ドアデータ"),
                    true);

                EditorGUI.EndDisabledGroup();
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded || property.FindPropertyRelative("doorData") == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            return EditorGUIUtility.singleLineHeight*2 + EditorGUIUtility.standardVerticalSpacing*2 + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("doorData"), label, true);
        }
    }
    #endif


    [Serializable]
    public class DoorDataList
    {
        [Serializable]
        public class DoorDataVertical
        {
            [Serializable]
            public class DoorDataHorizontal
            {
                public DoorData left;
                public DoorData middle;
                public DoorData right;

                public DoorData this[DoorHorizontalPosition position]
                {
                    get
                    {
                        switch (position)
                        {
                            case DoorHorizontalPosition.Left:
                                return left;
                            case DoorHorizontalPosition.Middle:
                                return middle;
                            case DoorHorizontalPosition.Right:
                                return right;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(position), position, null);
                        }
                    }
                    set
                    {
                        switch (position)
                        {
                            case DoorHorizontalPosition.Left:
                                left = value;
                                break;
                            case DoorHorizontalPosition.Middle:
                                middle = value;
                                break;
                            case DoorHorizontalPosition.Right:
                                right = value;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(position), position, null);
                        }
                    }
                }
            }

            public DoorDataHorizontal top;
            public DoorDataHorizontal middle;
            public DoorDataHorizontal bottom;

            public DoorDataHorizontal this[DoorVerticalPosition position]
            {
                get
                {
                    switch (position)
                    {
                        case DoorVerticalPosition.Top:
                            return top;
                        case DoorVerticalPosition.Middle:
                            return middle;
                        case DoorVerticalPosition.Bottom:
                            return bottom;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(position), position, null);
                    }
                }
            }
        }

        public DoorDataVertical north;
        public DoorDataVertical east;
        public DoorDataVertical south;
        public DoorDataVertical west;

        public DoorDataVertical this[DoorWallDirection direction]
        {
            get
            {
                switch (direction)
                {
                    case DoorWallDirection.North:
                        return north;
                    case DoorWallDirection.East:
                        return east;
                    case DoorWallDirection.South:
                        return south;
                    case DoorWallDirection.West:
                        return west;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                }
            }
        }


        public DoorData this[DoorPositionData position]
        {
            get
            {
                DoorDataVertical wallDoorData;
                switch (position.direction)
                {
                    case DoorWallDirection.North:
                        wallDoorData = north;
                        break;
                    case DoorWallDirection.West:
                        wallDoorData = west;
                        break;
                    case DoorWallDirection.South:
                        wallDoorData = south;
                        break;
                    case DoorWallDirection.East:
                        wallDoorData = east;
                        break;
                    default:
                        wallDoorData = new DoorDataVertical();
                        break;
                }

                DoorDataVertical.DoorDataHorizontal doorDataHorizontal;
                doorDataHorizontal = wallDoorData[position.verticalPosition];
                return doorDataHorizontal[position.horizontalPosition];
            }

            set
            {
                DoorDataVertical wallDoorData;
                switch (position.direction)
                {
                    case DoorWallDirection.North:
                        wallDoorData = north;
                        break;
                    case DoorWallDirection.West:
                        wallDoorData = west;
                        break;
                    case DoorWallDirection.South:
                        wallDoorData = south;
                        break;
                    case DoorWallDirection.East:
                        wallDoorData = east;
                        break;
                    default:
                        wallDoorData = new DoorDataVertical();
                        break;
                }

                DoorDataVertical.DoorDataHorizontal doorDataHorizontal;
                doorDataHorizontal = wallDoorData[position.verticalPosition];
                doorDataHorizontal[position.horizontalPosition] = value;
            }
        }

        public IEnumerator<KeyValuePair<DoorPositionData, DoorData>> GetEnumerator()
        {
            foreach (DoorWallDirection direction in Enum.GetValues(typeof(DoorWallDirection)))
            {
                foreach (DoorVerticalPosition verticalPosition in Enum.GetValues(typeof(DoorVerticalPosition)))
                {
                    foreach (DoorHorizontalPosition horizontalPosition in Enum.GetValues(typeof(DoorHorizontalPosition)))
                    {
                        DoorPositionData key = new DoorPositionData
                        {
                            direction = direction,
                            verticalPosition = verticalPosition,
                            horizontalPosition = horizontalPosition
                        };
                        yield return new KeyValuePair<DoorPositionData, DoorData>(key, this[key]);
                    }
                }
            }

        }

        public void DoorDataListReset()
        {
            north = new DoorDataVertical();
            east = new DoorDataVertical();
            south = new DoorDataVertical();
            west = new DoorDataVertical();
        }
    }


    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DoorDataList))]
    public class DoorDataListDrawer : PropertyDrawer
    {
        private DoorPositionData currentPosition = new DoorPositionData
        {
            direction = DoorWallDirection.North,
            verticalPosition = DoorVerticalPosition.Top,
            horizontalPosition = DoorHorizontalPosition.Right
        };


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            property.isExpanded = EditorGUI.Foldout(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                property.isExpanded,
                label);

            if (property.isExpanded)
            {
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                EditorGUI.indentLevel++;

                if (GUI.Button(new Rect(position.x + EditorGUI.indentLevel * 15, position.y, position.width - EditorGUI.indentLevel * 15, EditorGUIUtility.singleLineHeight), "リセット"))
                {
                    MethodInfo method = property.managedReferenceValue.GetType().GetMethod("DoorDataListReset",BindingFlags.Public | BindingFlags.Instance);
                    method.Invoke(property.managedReferenceValue, null);
                }

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                position.y += BuildButtons(position, property);

                position.y += EditorGUIUtility.standardVerticalSpacing;


                SerializedProperty currentDoorProperty = property
                    ?.FindPropertyRelative(currentPosition.direction.ToString().ToLower())
                    ?.FindPropertyRelative(currentPosition.verticalPosition.ToString().ToLower())
                    ?.FindPropertyRelative(currentPosition.horizontalPosition.ToString().ToLower());
                
                if (currentPosition.verticalPosition != DoorVerticalPosition.Middle && currentPosition.horizontalPosition != DoorHorizontalPosition.Middle)
                {
                    EditorGUI.BeginDisabledGroup(true);
                }

                EditorGUI.PropertyField(
                    new Rect(position.x, position.y, position.width, EditorGUI.GetPropertyHeight(currentDoorProperty)),
                    currentDoorProperty,
                    new GUIContent("Selected Door"),
                    true);
                
                EditorGUI.EndDisabledGroup();

                position.y +=  EditorGUI.GetPropertyHeight(currentDoorProperty) + EditorGUIUtility.standardVerticalSpacing;

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public float BuildButtons(Rect position, SerializedProperty property)
        {
            float buttonSize = 20f;
            float buttonSpacing = 2f;

            var gridRect = new Rect(EditorGUI.indentLevel * 15 + position.x, position.y, 11*buttonSize + 12*buttonSpacing, 11*buttonSize + 12*buttonSpacing);

            GUI.Box(gridRect, "");

            for (int y = 0; y < 11; y++)
            {
                for (int x = 0; x < 11; x++)
                {
                    DoorPositionData doorPositionData = GetDoorPositionData(x, y);

                    if (doorPositionData != null)
                    {
                        SerializedProperty doorProperty = property
                            .FindPropertyRelative(doorPositionData.direction.ToString().ToLower())
                            .FindPropertyRelative(doorPositionData.verticalPosition.ToString().ToLower())
                            .FindPropertyRelative(doorPositionData.horizontalPosition.ToString().ToLower());

                        Rect rect = new Rect(
                            position.x + x*buttonSize+ (x+1)*buttonSpacing + EditorGUI.indentLevel * 15,
                            position.y + y*buttonSize+ (y+1)*buttonSpacing,
                            buttonSize, buttonSize);

                        GUIStyle style = EditorStyles.miniButton;

                        GUIContent content = new GUIContent("");

                        bool isSelected = (
                            doorPositionData.horizontalPosition == currentPosition.horizontalPosition
                            && doorPositionData.verticalPosition == currentPosition.verticalPosition
                            && doorPositionData.direction == currentPosition.direction);

                        if (isSelected)
                        {
                            GUI.color = new Color(0.5f, 0f, 0f, 1f).ToBackgroundIgnoreColor();
                        }
                        else
                        {
                            if (doorProperty.FindPropertyRelative("isDoor").boolValue)
                            {
                                GUI.color = new Color(0.5f, 0.25f, 0f, 1f).ToBackgroundIgnoreColor();
                            }
                            else
                            {
                                GUI.color = Color.white;
                            }
                        }

                        if (GUI.Button(rect, content, style))
                        {
                            if (Event.current.button == 0)
                            {
                                currentPosition = doorPositionData;
                            }
                            else if (Event.current.button == 1)
                            {
                                // Toggle door state
                                bool isDoor = doorProperty.FindPropertyRelative("isDoor").boolValue;
                                doorProperty.FindPropertyRelative("isDoor").boolValue = !isDoor;
                            }
                        }
                    }
                }
            }

            GUI.color = Color.white;

            return 11*buttonSize + 12*buttonSpacing;
        }

        private DoorPositionData GetDoorPositionData(int x, int y)
        {
            if      (x == 0 && y == 5) return new DoorPositionData{ direction = DoorWallDirection.West, verticalPosition = DoorVerticalPosition.Top, horizontalPosition = DoorHorizontalPosition.Middle };
            else if (x == 1 && y == 3) return new DoorPositionData{ direction = DoorWallDirection.West, verticalPosition = DoorVerticalPosition.Middle, horizontalPosition = DoorHorizontalPosition.Right };
            else if (x == 1 && y == 5) return new DoorPositionData{ direction = DoorWallDirection.West, verticalPosition = DoorVerticalPosition.Middle, horizontalPosition = DoorHorizontalPosition.Middle };
            else if (x == 1 && y == 7) return new DoorPositionData{ direction = DoorWallDirection.West, verticalPosition = DoorVerticalPosition.Middle, horizontalPosition = DoorHorizontalPosition.Left };
            else if (x == 2 && y == 5) return new DoorPositionData{ direction = DoorWallDirection.West, verticalPosition = DoorVerticalPosition.Bottom, horizontalPosition = DoorHorizontalPosition.Middle };

            else if (x == 5 && y == 0) return new DoorPositionData{ direction = DoorWallDirection.North, verticalPosition = DoorVerticalPosition.Top, horizontalPosition = DoorHorizontalPosition.Middle };
            else if (x == 7 && y == 1) return new DoorPositionData{ direction = DoorWallDirection.North, verticalPosition = DoorVerticalPosition.Middle, horizontalPosition = DoorHorizontalPosition.Right };
            else if (x == 5 && y == 1) return new DoorPositionData{ direction = DoorWallDirection.North, verticalPosition = DoorVerticalPosition.Middle, horizontalPosition = DoorHorizontalPosition.Middle };
            else if (x == 3 && y == 1) return new DoorPositionData{ direction = DoorWallDirection.North, verticalPosition = DoorVerticalPosition.Middle, horizontalPosition = DoorHorizontalPosition.Left };
            else if (x == 5 && y == 2) return new DoorPositionData{ direction = DoorWallDirection.North, verticalPosition = DoorVerticalPosition.Bottom, horizontalPosition = DoorHorizontalPosition.Middle };

            else if (x == 10 && y == 5) return new DoorPositionData{ direction = DoorWallDirection.East, verticalPosition = DoorVerticalPosition.Top, horizontalPosition = DoorHorizontalPosition.Middle };
            else if (x == 9 && y == 7) return new DoorPositionData{ direction = DoorWallDirection.East, verticalPosition = DoorVerticalPosition.Middle, horizontalPosition = DoorHorizontalPosition.Right };
            else if (x == 9 && y == 5) return new DoorPositionData{ direction = DoorWallDirection.East, verticalPosition = DoorVerticalPosition.Middle, horizontalPosition = DoorHorizontalPosition.Middle };
            else if (x == 9 && y == 3) return new DoorPositionData{ direction = DoorWallDirection.East, verticalPosition = DoorVerticalPosition.Middle, horizontalPosition = DoorHorizontalPosition.Left };
            else if (x == 8 && y == 5) return new DoorPositionData{ direction = DoorWallDirection.East, verticalPosition = DoorVerticalPosition.Bottom, horizontalPosition = DoorHorizontalPosition.Middle };

            else if (x == 5 && y == 10) return new DoorPositionData{ direction = DoorWallDirection.South, verticalPosition = DoorVerticalPosition.Top, horizontalPosition = DoorHorizontalPosition.Middle };
            else if (x == 3 && y == 9) return new DoorPositionData{ direction = DoorWallDirection.South, verticalPosition = DoorVerticalPosition.Middle, horizontalPosition = DoorHorizontalPosition.Right };
            else if (x == 5 && y == 9) return new DoorPositionData{ direction = DoorWallDirection.South, verticalPosition = DoorVerticalPosition.Middle, horizontalPosition = DoorHorizontalPosition.Middle };
            else if (x == 7 && y == 9) return new DoorPositionData{ direction = DoorWallDirection.South, verticalPosition = DoorVerticalPosition.Middle, horizontalPosition = DoorHorizontalPosition.Left };
            else if (x == 5 && y == 8) return new DoorPositionData{ direction = DoorWallDirection.South, verticalPosition = DoorVerticalPosition.Bottom, horizontalPosition = DoorHorizontalPosition.Middle };

            else return null;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.managedReferenceValue == null || !property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            SerializedProperty currentDoorProperty = property
                        .FindPropertyRelative(currentPosition.direction.ToString().ToLower())
                        .FindPropertyRelative(currentPosition.verticalPosition.ToString().ToLower())
                        .FindPropertyRelative(currentPosition.horizontalPosition.ToString().ToLower());

            return 244 +  EditorGUIUtility.singleLineHeight*2 + EditorGUIUtility.standardVerticalSpacing*3 + EditorGUI.GetPropertyHeight(currentDoorProperty);
        }
    }
    #endif


    [Serializable]
    public class DoorConfigData
    {
        public InteractiveDoor defaultDoorPrefab;
        public InteractiveDoor defaultDoorPrefab_Low;
        public Material defaultDoorMaterial;
        public Material defaultWallMaterial;
    }




    [Serializable]
    public class RoomSetController : MonoBehaviour
    {
        [SerializeField]
        public Texture2D roomThumb;

        [SerializeField]
        public Material roomThumbMaterial;

        [SerializeField]
        public RoomMode roomMode = RoomMode.Normal;

        [SerializeField]
        public bool isBasement = false;


        [SerializeReference]
        public DoorDataList doorDataList;

        [CustomList("デカールデータ", "デカール")]
        public PropertyList<DecalData> _decalDataList;

        [CustomList("入力デカールデータ", "デカール"),Readonly]
        public PropertyList<DecalData> _decalDataListInput;
        public List<DecalData> DecalDataList
        {
            get
            {
                List<DecalData> data = _decalDataList.list.Concat(_decalDataListInput.list).ToList();
                foreach (var decalData in data)
                {
                    decalData.canDelete = false;
                }
                return data;
            }
        }


        [SerializeField]
        public RoomBuildAssets roomBuildAssets;

        [SerializeField]
        public PanelAssets panelAssets;

        [SerializeField]
        public DoorConfigData doorConfigData;

        [SerializeField]
        public RoomDataController roomDataController;
        [SerializeField]
        public PanelDataController panelDataController;


    #if UNITY_EDITOR
        public void SetUp()
        {
            _decalDataListInput.list.Clear();
            roomDataController.SetUp(this);
            panelDataController.SetUp(this);

            EditorUtility.SetDirty(this);
        }
    #endif
    }


    #if UNITY_EDITOR
    [CustomEditor(typeof(RoomSetController))]
    public class RoomSetControllerDrawer : Editor
    {
        private RoomSetController roomSetAuto;

        private bool _isOpenAssets = true;
        private bool _isOpenComponent = true;
        private bool _isOpenRoomSetting = true;

        private void OnEnable()
        {
            roomSetAuto = (RoomSetController)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();


            if (GUI.Button(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()), "ドアの位置を自動設定"))
            {
                roomSetAuto.SetUp();
            }


            _isOpenAssets = EditorGUILayout.Foldout(_isOpenAssets, "アセット設定", true, EditorStyles.foldoutHeader);
            if (_isOpenAssets)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("roomBuildAssets"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("panelAssets"));

                EditorGUI.indentLevel--;
            }



            _isOpenComponent = EditorGUILayout.Foldout(_isOpenComponent, "コンポーネント設定", true, EditorStyles.foldoutHeader);
            if (_isOpenComponent)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("roomDataController"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("panelDataController"));
                EditorGUI.indentLevel--;
            }

            _isOpenRoomSetting = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), _isOpenRoomSetting, "部屋設定", true, EditorStyles.foldoutHeader);
            if (_isOpenRoomSetting)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("roomThumb"), new GUIContent("部屋サムネイル"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("roomThumbMaterial"), new GUIContent("ディスプレイマテリアル"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("roomMode"), new GUIContent("部屋タイプ"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("isBasement"), new GUIContent("地下フロア"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("doorDataList"), new GUIContent("ドア設定"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("doorConfigData"), new GUIContent("ドアデフォルトアセット"));


                EditorGUILayout.PropertyField(serializedObject.FindProperty("_decalDataList"), new GUIContent("デカール設定"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_decalDataListInput"), new GUIContent("入力デカール設定"));

                EditorGUI.indentLevel--;
            }

            if (EditorGUI.EndChangeCheck())
            {

            }

            serializedObject.ApplyModifiedProperties();
        }


    }
    #endif
}
