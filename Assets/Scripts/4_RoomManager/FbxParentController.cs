using UnityEngine;
using System;
using Rooms.Auto;


namespace Rooms.RoomSystem
{
    public class FbxParentController : MonoBehaviour
    {
        [Serializable]
        public class WallData
        {
            public SkinnedMeshRenderer wall;
            public SkinnedMeshRenderer wallRail;
            public GameObject doorFrame_left;
            public GameObject doorFrame_middle;
            public GameObject doorFrame_right;

            public GameObject this[DoorHorizontalPosition position]
            {
                get
                {
                    switch (position)
                    {
                        case DoorHorizontalPosition.Left:
                            return doorFrame_left;
                        case DoorHorizontalPosition.Middle:
                            return doorFrame_middle;
                        case DoorHorizontalPosition.Right:
                            return doorFrame_right;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(position), position, null);
                    }
                }
                set
                {
                    switch (position)
                    {
                        case DoorHorizontalPosition.Left:
                            doorFrame_left = value;
                            break;
                        case DoorHorizontalPosition.Middle:
                            doorFrame_middle = value;
                            break;
                        case DoorHorizontalPosition.Right:
                            doorFrame_right = value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(position), position, null);
                    }
                }
            }
        }

        public WallData north;
        public WallData east;
        public WallData south;
        public WallData west;

        public WallData this[DoorWallDirection position]
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
            set
            {
                switch (position)
                {
                    case DoorWallDirection.North:
                        north = value;
                        break;
                    case DoorWallDirection.East:
                        east = value;
                        break;
                    case DoorWallDirection.South:
                        south = value;
                        break;
                    case DoorWallDirection.West:
                        west = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(position), position, null);
                }
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
                Transform tmpObj;

                this[direction].wall = transform.Find("Wall_Base_" + direction.ToHeadName()).GetComponent<SkinnedMeshRenderer>();

                this[direction].wall.SetBlendShapeWeight(0, 0f);
                this[direction].wall.SetBlendShapeWeight(1, 0f);
                this[direction].wall.SetBlendShapeWeight(2, 0f);

                tmpObj = transform.Find("Wall_Rail_" + direction.ToHeadName());
                if (tmpObj != null)
                {
                    this[direction].wallRail = tmpObj.GetComponent<SkinnedMeshRenderer>();
                }
                else
                {
                    this[direction].wallRail = null;
                }

                foreach (DoorHorizontalPosition position in Enum.GetValues(typeof(DoorHorizontalPosition)))
                {
                    DoorPositionData positionData = new DoorPositionData
                    {
                        direction = direction,
                        horizontalPosition = position
                    };

                    this[direction].wall.SetBlendShapeWeight(
                        position.ToShapeKeyInt(),
                        roomSetController.doorDataList[positionData].isDoor ? 100f : 0f
                    );

                    if (this[direction].wallRail != null)
                    {
                        this[direction].wallRail.SetBlendShapeWeight(
                            position.ToShapeKeyInt(),
                            roomSetController.doorDataList[positionData].isDoor ? 100f : 0f
                        );
                    }

                    tmpObj = transform.Find("Wall_Frame_" + direction.ToHeadName() + "_" + position.ToHeadName());
                    if (tmpObj != null)
                    {
                        this[direction][position] = tmpObj.gameObject;

                        this[direction][position].SetActive(roomSetController.doorDataList[positionData].isDoor);
                    }
                    else
                    {
                        this[direction][position] = null;
                    }
                }
            }
        }
        #endif
    }

}
