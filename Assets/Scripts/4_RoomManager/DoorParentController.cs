using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rooms.Auto;
using UnityEngine.UIElements.Experimental;
using UnityEditor;



namespace Rooms.DoorSystem
{
    public class DoorParentController : MonoBehaviour
    {
        public List<DoorController> doors = new List<DoorController>();



        public void ResetRoom()
        {
            StartCoroutine(ResetCoroutine());
        }

        private IEnumerator ResetCoroutine()
        {
            foreach (var door in doors)
            {
                if (door == null) continue; // Skip if the door is null
                door.ResetDoor();
            }

            for (int i = 0; i < 1; i++)
            {
                yield return null;
            }

            foreach (var door in doors)
            {
                if (door == null) continue; // Skip if the door is null
                door.CollisionCheck();
            }
        }



        public void RefreshRoom()
        {
            StartCoroutine(RefreshCoroutine());
        }

        private IEnumerator RefreshCoroutine()
        {
            foreach (var door in doors)
            {
                if (door == null) continue; // Skip if the door is null
                door.RefreshDoor();
            }

            for (int i = 0; i < 1; i++)
            {
                yield return null;
            }

            foreach (var door in doors)
            {
                if (door == null) continue; // Skip if the door is null
                door.CollisionCheck();
            }
        }




        #if UNITY_EDITOR
        public void SetUp(RoomSetController roomSetController)
        {
            List<GameObject> doorObjects = new List<GameObject>();
            foreach (Transform child in transform)
            {
                doorObjects.Add(child.gameObject);
            }

            for (int i = 0; i < doorObjects.Count; i++)
            {
                if (doorObjects[i] != null)
                {
                    DestroyImmediate(doorObjects[i]);
                }
            }

            doors = new List<DoorController>();

            foreach (KeyValuePair<DoorPositionData, DoorData> doorData in roomSetController.doorDataList)
            {
                if (roomSetController.doorDataList[doorData.Key].isDoor)
                {
                    DoorController doorController = (DoorController)PrefabUtility.InstantiatePrefab(roomSetController.roomBuildAssets.doorPrefab, transform);

                    InteractiveDoor door = doorData.Key.verticalPosition != DoorVerticalPosition.Middle ? roomSetController.roomBuildAssets.defaultDoorPrefab_Low : null;
                    if ((doorData.Key.verticalPosition == DoorVerticalPosition.Middle ?
                        roomSetController.doorConfigData.defaultDoorPrefab :
                        roomSetController.doorConfigData.defaultDoorPrefab_Low) != null)
                    {
                        door = doorData.Key.verticalPosition == DoorVerticalPosition.Middle ?
                        roomSetController.doorConfigData.defaultDoorPrefab :
                        roomSetController.doorConfigData.defaultDoorPrefab_Low;
                    }

                    if (doorData.Value.doorData.door != null)
                    {
                        door = doorData.Value.doorData.door;
                    }

                    if (door != null)
                    {
                        DestroyImmediate(doorController.Door.gameObject);
                        doorController.Door = (InteractiveDoor)PrefabUtility.InstantiatePrefab(door, doorController.transform);
                    }

                    doorController.transform.localPosition = doorData.Key.ToVector3();
                    doorController.transform.localRotation = Quaternion.Euler(0f, doorData.Key.direction.ToDoorRotate(), 0f);


                    Material material = (doorData.Value.doorData.wallMaterial != null) ?
                        doorData.Value.doorData.wallMaterial : roomSetController.doorConfigData.defaultWallMaterial;
                    if (material != null)
                    {
                        if (doorController.Door.PliminaryWall != null)
                        {
                            doorController.Door.PliminaryWall.GetComponent<Renderer>().sharedMaterial = material;
                        }
                    }

                    material = null;
                    material = (doorData.Value.doorData.doorBodyMaterial != null) ?
                        doorData.Value.doorData.doorBodyMaterial : roomSetController.doorConfigData.defaultDoorMaterial;
                    if (material != null)
                    {
                        foreach (InteractiveDoor.DoorBodyData bodyData in doorController.Door.DoorBody)
                        {
                            Material[] materials = bodyData.DoorBody.GetComponent<Renderer>().sharedMaterials;
                            materials[bodyData.materialIndex] = material;
                            bodyData.DoorBody.GetComponent<Renderer>().sharedMaterials = materials;
                        }
                    }

                    doorController.LockList = doorData.Value.doorData.LockData;


                    if (doorData.Key.verticalPosition == DoorVerticalPosition.Middle)
                    {
                        doorController.WallMesh = roomSetController.roomDataController.fbxParentController[doorData.Key.direction].wall;
                        doorController.ShapeKey = doorData.Key.horizontalPosition.ToShapeKeyInt();
                    }

                    doors.Add(doorController);
                }
            }
        }
        #endif
    }
}
