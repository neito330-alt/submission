using System.Net.Sockets;
using Rooms.Auto;
using Rooms.PanelSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Rooms
{
    public enum RoomMode
    {
        Normal = 0,
        Static = 1,
        Rotate = 2
    }

    public static class RoomModeEnum
    {
        public static Color ToColor(this RoomMode roomMode)
        {
            return roomMode switch
            {
                RoomMode.Normal => Color.white,
                RoomMode.Static => Color.red,
                RoomMode.Rotate => Color.green,
                _ => Color.gray,
            };
        }

        public static bool CanMove(this RoomMode roomMode)
        {
            switch (roomMode)
            {
                case RoomMode.Normal:
                    return true;
                case RoomMode.Static:
                    return false;
                case RoomMode.Rotate:
                    return false;
                default:
                    return false;
            }
        }

        public static bool CanHold(this RoomMode roomMode)
        {
            switch (roomMode)
            {
                case RoomMode.Normal:
                    return true;
                case RoomMode.Static:
                    return false;
                case RoomMode.Rotate:
                    return false;
                default:
                    return false;
            }
        }

        public static bool CanRotate(this RoomMode roomMode)
        {
            switch (roomMode)
            {
                case RoomMode.Normal:
                    return true;
                case RoomMode.Static:
                    return false;
                case RoomMode.Rotate:
                    return true;
                default:
                    return false;
            }
        }

        public static Material ToMaterial(this RoomMode roomMode)
        {
            PanelCornerMaterial assets = RoomsAssetsManager.PanelAssets.cornerMaterial;

            return roomMode switch
            {
                RoomMode.Normal => assets.Normal,
                RoomMode.Static => assets.Static,
                RoomMode.Rotate => assets.Rotate,
                _ => assets.Normal,
            };
        }
    }
}
