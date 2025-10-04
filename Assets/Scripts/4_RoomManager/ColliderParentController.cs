using UnityEngine;
using System;
using Rooms.Auto;
using UnityEngine.InputSystem;
using System.Collections.Generic;


namespace Rooms.RoomSystem
{
    public class ColliderParentController : MonoBehaviour
    {
        [Serializable]
        public class VerticalColliderData
        {
            public MeshCollider top;
            public MeshCollider middle;
            public MeshCollider bottom;

            public MeshCollider this[DoorVerticalPosition position]
            {
                get
                {
                    return position switch
                    {
                        DoorVerticalPosition.Top => top,
                        DoorVerticalPosition.Middle => middle,
                        DoorVerticalPosition.Bottom => bottom,
                        _ => throw new ArgumentOutOfRangeException(nameof(position), position, null)
                    };
                }
            }
        }


        public VerticalColliderData north;
        public VerticalColliderData east;
        public VerticalColliderData south;
        public VerticalColliderData west;

        public MeshCollider floorMesh;

        public VerticalColliderData this[DoorWallDirection position]
        {
            get
            {
                return position switch
                {
                    DoorWallDirection.North => north,
                    DoorWallDirection.East => east,
                    DoorWallDirection.South => south,
                    DoorWallDirection.West => west,
                    _ => throw new ArgumentOutOfRangeException(nameof(position), position, null)
                };
            }
        }




        public void ResetRoom()
        {

        }

        public void RefreshRoom()
        {
        }


        #if UNITY_EDITOR
        public void SetUp(RoomSetController roomSetController)
        {
            foreach (DoorWallDirection direction in Enum.GetValues(typeof(DoorWallDirection)))
            {
                this[direction].bottom.gameObject.SetActive(roomSetController.isBasement);

                foreach (DoorVerticalPosition verticalPosition in Enum.GetValues(typeof(DoorVerticalPosition)))
                {
                    int value = 0;

                    foreach (DoorHorizontalPosition horizontalPosition in Enum.GetValues(typeof(DoorHorizontalPosition)))
                    {
                        DoorPositionData positionData = new DoorPositionData
                        {
                            direction = direction,
                            verticalPosition = verticalPosition,
                            horizontalPosition = horizontalPosition
                        };
                        if (roomSetController.doorDataList[positionData].isDoor)
                        {
                            value += horizontalPosition.ToMeshValue();
                        }
                    }

                    this[direction][verticalPosition].sharedMesh = roomSetController.roomBuildAssets.wallMesh[verticalPosition].GetMesh(value);
                }
            }

            floorMesh.transform.localPosition = new Vector3(0, roomSetController.isBasement ? -2.45f : -0.05f, 0);
        }
        #endif
    }
}
