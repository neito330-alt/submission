using System;
using System.Runtime.CompilerServices;
using Rooms.Auto;
using Rooms.DoorSystem;
using Rooms.PanelSystem;
using UnityEngine;

namespace Rooms
{
    public class RoomsManager : MonoBehaviour
    {
        [SerializeField]
        RoomBuildAssets _roomBuildAssets;

        [SerializeField]
        PanelAssets _panelAssets;

        public static DoorLockColor CurrentDoorLockColor = DoorLockColor.None;

        public void SetCurrentDoorLockColor(DoorLockColor color)
        {
            CurrentDoorLockColor = color;
        }

        public static event Action RoomsUpdated;
        public static void OnRoomsUpdated()
        {
            CurrentDoorLockColor = DoorLockColor.None; // Reset the color when rooms are updated
            RoomsUpdated?.Invoke();
        }

        public static event Action<Vector2Int, DecalItem> DecalAdded;
        public static void OnDecalAdded(Vector2Int position, DecalItem decalItem)
        {
            DecalAdded?.Invoke(position, decalItem);
        }

        public static event Action<Vector2Int, Vector2> DecalRemoved;
        public static void OnDecalRemoved(Vector2Int position, Vector2 decalPosition)
        {
            DecalRemoved?.Invoke(position, decalPosition);
        }

        void Awake()
        {
            RoomsAssetsManager.RoomBuildAssets = _roomBuildAssets;
            RoomsAssetsManager.PanelAssets = _panelAssets;
            Debug.Log("RoomsAssetsManager initialized with RoomBuildAssets and PanelAssets.");
            //await RoomsAssetsManager.InitializeAsync();


        }

        void Start()
        {
            InteractionEvents.Instance.ColorButtonInteracted += SetCurrentDoorLockColor;
        }

    }
}
