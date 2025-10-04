using UnityEngine;
using System.Collections.Generic;
using Rooms.DoorSystem;
using System;


namespace Rooms.Auto
{
    [Serializable]
    public class DefaultMaterial
    {
        public Material North_Right;
        public Material North_Middle;
        public Material North_Left;
        public Material West_Right;
        public Material West_Middle;
        public Material West_Left;
        public Material South_Right;
        public Material South_Middle;
        public Material South_Left;
        public Material East_Right;
        public Material East_Middle;
        public Material East_Left;

        public Material GetMaterial(DoorWallDirection direction, DoorHorizontalPosition position)
        {
            switch (direction)
            {
                case DoorWallDirection.North:
                    return position switch
                    {
                        DoorHorizontalPosition.Right => North_Right,
                        DoorHorizontalPosition.Middle => North_Middle,
                        DoorHorizontalPosition.Left => North_Left,
                        _ => throw new ArgumentOutOfRangeException(nameof(position), position, null),
                    };
                case DoorWallDirection.West:
                    return position switch
                    {
                        DoorHorizontalPosition.Right => West_Right,
                        DoorHorizontalPosition.Middle => West_Middle,
                        DoorHorizontalPosition.Left => West_Left,
                        _ => throw new ArgumentOutOfRangeException(nameof(position), position, null),
                    };
                case DoorWallDirection.South:
                    return position switch
                    {
                        DoorHorizontalPosition.Right => South_Right,
                        DoorHorizontalPosition.Middle => South_Middle,
                        DoorHorizontalPosition.Left => South_Left,
                        _ => throw new ArgumentOutOfRangeException(nameof(position), position, null),
                    };
                case DoorWallDirection.East:
                    return position switch
                    {
                        DoorHorizontalPosition.Right => East_Right,
                        DoorHorizontalPosition.Middle => East_Middle,
                        DoorHorizontalPosition.Left => East_Left,
                        _ => throw new ArgumentOutOfRangeException(nameof(position), position, null),
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
    }

    [Serializable]
    public class VerticalWallMesh
    {
        [Serializable]
        public class HorizontalWallMesh
        {
            public Mesh Wall_000;
            public Mesh Wall_001;
            public Mesh Wall_010;
            public Mesh Wall_011;
            public Mesh Wall_100;
            public Mesh Wall_101;
            public Mesh Wall_110;
            public Mesh Wall_111;

            public Mesh GetMesh(int meshValue)
            {
                return meshValue switch
                {
                    0 => Wall_000,
                    1 => Wall_001,
                    2 => Wall_010,
                    3 => Wall_011,
                    4 => Wall_100,
                    5 => Wall_101,
                    6 => Wall_110,
                    7 => Wall_111,
                    _ => null,
                };
            }
        }

        public HorizontalWallMesh Top;
        public HorizontalWallMesh Middle;
        public HorizontalWallMesh Bottom;

        public HorizontalWallMesh this[DoorVerticalPosition index]
        {
            get
            {
                return index switch
                {
                    DoorVerticalPosition.Top => Top,
                    DoorVerticalPosition.Middle => Middle,
                    DoorVerticalPosition.Bottom => Bottom,
                    _ => null,
                };
            }
        }
    }





    [CreateAssetMenu(fileName = "RoomBuildAssets", menuName = "ScriptableObjects/RoomBuildAssets", order = 1)]
    public class RoomBuildAssets : ScriptableObject
    {
        [SerializeField]
        public InteractiveDoor defaultDoorPrefab;
        [SerializeField]
        public InteractiveDoor defaultDoorPrefab_Low;

        [SerializeField]
        public DoorController doorPrefab;
        [SerializeField]
        public DefaultMaterial defaultMaterial;
        [SerializeField]
        public VerticalWallMesh wallMesh;
    }
}
