using System;
using System.Collections.Generic;
using System.Threading;
using Rooms.PanelSystem;
using UnityEngine;

namespace Rooms.RoomSystem
{
    public class RoomManager : MonoBehaviour
    {
        void Start()
        {
            RoomsManager.RoomsUpdated += ResetStage;
        }

        void OnDestroy()
        {
            RoomsManager.RoomsUpdated -= ResetStage;
        }


        public void ResetStage()
        {
            Debug.Log("ResetStage called");

            foreach (RoomDataController roomDataController in transform.GetComponentsInChildren<RoomDataController>())
            {
                roomDataController.transform.localPosition = roomDataController.roomSetData.ToWorldPosition() * 8;
                roomDataController.transform.localRotation = Quaternion.Euler(0, roomDataController.roomSetData.Rotation + 180, 0);
            }

            foreach (RoomDataController roomDataController in transform.GetComponentsInChildren<RoomDataController>())
            {
                roomDataController.ResetRoom();
            }

            GameManager.playerManager.LightUpdate(true);
        }

    }
}
