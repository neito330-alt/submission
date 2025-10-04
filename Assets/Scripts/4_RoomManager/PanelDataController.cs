using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Rooms.Auto;
using Rooms.DoorSystem;
using Rooms.RoomSystem;
using SerializedArray;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering.HighDefinition;

namespace Rooms.PanelSystem
{
    public enum PanelDirection
    {
        Up,
        Right,
        Down,
        Left
    }

    public static class DirectionEnum
    {
        public static PanelDirection ToRight(this PanelDirection direction)
        {
            switch (direction)
            {
                case PanelDirection.Up:
                    return PanelDirection.Right;
                case PanelDirection.Right:
                    return PanelDirection.Down;
                case PanelDirection.Down:
                    return PanelDirection.Left;
                case PanelDirection.Left:
                    return PanelDirection.Up;
            }
            return PanelDirection.Left;
        }

        public static PanelDirection ToLeft(this PanelDirection direction)
        {
            switch (direction)
            {
                case PanelDirection.Up:
                    return PanelDirection.Left;
                case PanelDirection.Right:
                    return PanelDirection.Up;
                case PanelDirection.Down:
                    return PanelDirection.Right;
                case PanelDirection.Left:
                    return PanelDirection.Down;
            }
            return PanelDirection.Left;
        }

        public static Vector2Int ToVector(this PanelDirection direction)
        {
            switch (direction)
            {
                case PanelDirection.Up:
                    return Vector2Int.up;
                case PanelDirection.Right:
                    return Vector2Int.right;
                case PanelDirection.Down:
                    return Vector2Int.down;
                case PanelDirection.Left:
                    return Vector2Int.left;
            }
            return Vector2Int.up;
        }
    }

    public class PanelDataController : MonoBehaviour
    {
        public RoomSetData roomSetData;
        public SlotData slotData;

        public SkinnedMeshRenderer panelRenderer;
        public DecalManager decalManager;

        public int _rotation;
        public int Rotation
        {
            get => _rotation;
            set
            {
                // if (_rotation < value)
                // {
                //     while (_rotation < value)_rotation -= 90;
                // }
                // else
                // {
                //     while (_rotation > value)_rotation += 90;
                // }

                _rotation = (value + 360) % 360;
                transform.localEulerAngles = new Vector3(0, _rotation + 180, 0);
                decalManager.Rotation = _rotation + 180;
            }
        }



        void Start()
        {
            SetPanelMaterial();

            //GetComponent<SkinnedMeshRenderer>().enabled = false;

            int rot = Rotation;
            _rotation = 0;
            Rotation = rot;
        }



        public void AddDecal(DecalItem decalItem)
        {
            float rot = -(Rotation+180) * Mathf.Deg2Rad;

            Debug.Log("Adding decal at position: " + decalItem.position + " with rotation: " + (Vector3)(Quaternion.Euler(0, 0, decalManager.Rotation) * decalItem.position));

            decalManager.AddDecal(
                new DecalItem()
                {
                    position = Quaternion.Euler(0, 0, decalManager.Rotation) * decalItem.position,
                    scale = decalItem.scale,
                    material = decalItem.material,
                    projector = decalItem.projector,
                    canDelete = decalItem.canDelete
                }
            );
        }

        public void AddDecal(DecalData decalData)
        {
            float rot = -((Rotation+180)-decalManager.Rotation) * Mathf.Deg2Rad;
            decalData.position = Quaternion.Euler(0, 0, decalManager.Rotation) * decalData.position;
            decalManager.AddDecal(decalData);
        }



        public void RemoveDecal(Vector2 position)
        {
            float rot = -((Rotation+180)-decalManager.Rotation) * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(
                position.x * Mathf.Cos(rot) - position.y * Mathf.Sin(rot),
                position.x * Mathf.Sin(rot) + position.y * Mathf.Cos(rot)
            );
            decalManager.RemoveDecal(Quaternion.Euler(0, 0, decalManager.Rotation) * position);
        }



        public void SetMaterial(Material material)
        {
            if (material == null)
            {
                GetComponent<SkinnedMeshRenderer>().enabled = false;
                return;
            }
            GetComponent<SkinnedMeshRenderer>().enabled = true;
            GetComponent<SkinnedMeshRenderer>().material = material;
        }

        public void SetPanelMaterial()
        {
            Material[] panelMaterials;

            panelMaterials = panelRenderer.materials;

            panelMaterials[0] = roomSetData.RoomSetController.roomMode.ToMaterial();

            foreach (KeyValuePair<DoorPositionData, DoorData> doorData in roomSetData.RoomSetController.doorDataList)
            {
                int index = doorData.Key.direction.ToMaterialIndex() + doorData.Key.horizontalPosition.ToMaterialIndex();
                if (doorData.Value.isDoor)
                {
                    if (doorData.Value.doorData.LockData.Count > 0)
                    {
                        panelMaterials[index] = doorData.Value.doorData.LockData[0].ToMaterial();
                    }
                    else
                    {
                        panelMaterials[index] = roomSetData.RoomSetController.panelAssets.doorMaterial.Open;
                    }
                }
            }
            panelRenderer.materials = panelMaterials;
        }



        private void OnDestroy()
        {
            var renderer = panelRenderer;
            int length   = renderer.materials.Length;
            for(int i = 0; i < length; ++i)
            {
                Destroy( renderer.materials[i] );
            }
        }



        #if UNITY_EDITOR
        public void SetUp(RoomSetController roomSetController)
        {
            SkinnedMeshRenderer panelParentMesh = GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer panelMesh = panelRenderer;

            Material[] panelMaterials = panelMesh.sharedMaterials;

            foreach (DoorWallDirection direction in Enum.GetValues(typeof(DoorWallDirection)))
            {
                foreach (DoorHorizontalPosition horizontalPosition in Enum.GetValues(typeof(DoorHorizontalPosition)))
                {
                    DoorPositionData doorPositionData = new DoorPositionData()
                    {
                        direction = direction,
                        horizontalPosition = horizontalPosition
                    };

                    DoorData doorData = roomSetController.doorDataList[doorPositionData];
                    if (doorData.isDoor)
                    {
                        panelParentMesh.SetBlendShapeWeight(direction.ToShapeKeyInt() + horizontalPosition.ToShapeKeyInt(), 100f);
                        panelMesh.SetBlendShapeWeight(direction.ToShapeKeyInt() + horizontalPosition.ToShapeKeyInt(), 100f);

                        Material doorMaterial = roomSetController.panelAssets.doorMaterial.Open;

                        if (doorData.doorData.LockData.Count > 0)
                        {
                            switch (doorData.doorData.LockData[0].LockType)
                            {
                                case DoorLockType.Item:
                                    doorMaterial = roomSetController.panelAssets.doorMaterial.Lock;
                                    break;
                                case DoorLockType.Key:
                                    doorMaterial = roomSetController.panelAssets.doorMaterial.Key;
                                    break;
                                case DoorLockType.Color:
                                    switch ((DoorLockColor)doorData.doorData.LockData[0].LockVal)
                                    {
                                        case DoorLockColor.Red:
                                            doorMaterial = roomSetController.panelAssets.doorMaterial.Red;
                                            break;
                                        case DoorLockColor.Green:
                                            doorMaterial = roomSetController.panelAssets.doorMaterial.Green;
                                            break;
                                        case DoorLockColor.Blue:
                                            doorMaterial = roomSetController.panelAssets.doorMaterial.Blue;
                                            break;
                                        case DoorLockColor.Yellow:
                                            doorMaterial = roomSetController.panelAssets.doorMaterial.Yellow;
                                            break;
                                        case DoorLockColor.Purple:
                                            doorMaterial = roomSetController.panelAssets.doorMaterial.Purple;
                                            break;
                                        case DoorLockColor.Orange:
                                            doorMaterial = roomSetController.panelAssets.doorMaterial.Orange;
                                            break;
                                        case DoorLockColor.Black:
                                            doorMaterial = roomSetController.panelAssets.doorMaterial.Black;
                                            break;
                                    }
                                    break;
                                case DoorLockType.Event:
                                    if ((string)doorData.doorData.LockData[0].LockVal == "ConfineTrap")
                                    {
                                        doorMaterial = roomSetController.panelAssets.doorMaterial.Danger;
                                    }
                                    else
                                    {
                                        doorMaterial = roomSetController.panelAssets.doorMaterial.Lock;
                                    }
                                    break;
                                default:
                                    doorMaterial = roomSetController.panelAssets.doorMaterial.Lock;
                                    break;
                            }

                        }
                        else
                        {
                            doorMaterial = roomSetController.panelAssets.doorMaterial.Open;
                        }
                        panelMaterials[direction.ToMaterialIndex() + horizontalPosition.ToMaterialIndex()] = doorMaterial;
                    }
                    else
                    {
                        panelParentMesh.SetBlendShapeWeight(direction.ToShapeKeyInt() + horizontalPosition.ToShapeKeyInt(), 0f);
                        panelMesh.SetBlendShapeWeight(direction.ToShapeKeyInt() + horizontalPosition.ToShapeKeyInt(), 0f);
                        panelMaterials[direction.ToMaterialIndex() + horizontalPosition.ToMaterialIndex()] = roomSetController.roomBuildAssets.defaultMaterial.GetMaterial(direction, horizontalPosition);
                    }
                }
            }
            panelMaterials[0] = roomSetController.roomMode switch
            {
                RoomMode.Normal => roomSetController.panelAssets.cornerMaterial.Normal,
                RoomMode.Static => roomSetController.panelAssets.cornerMaterial.Static,
                RoomMode.Rotate => roomSetController.panelAssets.cornerMaterial.Rotate,
                _               => roomSetController.panelAssets.cornerMaterial.Normal
            };
            panelMaterials[2] = roomSetController.roomThumbMaterial;

            panelMesh.sharedMaterials = panelMaterials;


        }
        #endif
    }
}
