using System;
using Rooms.Auto;
using Rooms.RoomSystem;
using UnityEngine;


public class ColorRoomSet : MonoBehaviour
{
    [Button("SetUp")]
    public bool button;

    public Material material;

    public void SetUp()
    {
        FbxParentController fbxParent = GetComponent<FbxParentController>();

        foreach (DoorWallDirection wallDirection in Enum.GetValues(typeof(DoorWallDirection)))
        {
            FbxParentController.WallData wallData = fbxParent[wallDirection];
            if (wallData != null)
            {
                if (wallData.wall != null)
                {
                    wallData.wall.sharedMaterial = material;
                }
                if (wallData.wallRail != null)
                {
                    wallData.wallRail.sharedMaterial = material;
                }
                foreach (DoorHorizontalPosition doorPosition in Enum.GetValues(typeof(DoorHorizontalPosition)))
                {
                    GameObject doorFrame = wallData[doorPosition];
                    if (doorFrame != null)
                    {
                        Renderer renderer = doorFrame.GetComponent<Renderer>();
                        renderer.sharedMaterial = material;
                    }
                }
            }
        }
    }


}
