using System.Threading.Tasks;
using Rooms.Auto;
using Rooms.PanelSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Rooms
{
    public class RoomsAssetsManager
    {
        private static RoomBuildAssets _roomBuildAssets;
        public static RoomBuildAssets RoomBuildAssets
        {
            set
            {
                _roomBuildAssets = value;
            }
            get
            {
                // if (_roomBuildAssets == null)
                // {
                //     Debug.LogError("RoomBuildAssets not initialized. Call InitializeAsync first.");
                // }
                return _roomBuildAssets;
            }
        }
        
        private static PanelAssets _panelAssets;
        public static PanelAssets PanelAssets
        {
            set
            {
                _panelAssets = value;
            }
            get
            {
                // if (_panelAssets == null)
                // {
                //     Debug.LogError("PanelAssets not initialized. Call InitializeAsync first.");
                // }
                return _panelAssets;
            }
        }

        
        public static async Task InitializeAsync()
        {
            if (_roomBuildAssets == null)
            {
                _roomBuildAssets = await Addressables.LoadAssetAsync<RoomBuildAssets>(
                    "Assets/Scripts/4_RoomManager/RoomBuildAssets.asset"
                ).Task;
            }
            if (_panelAssets == null)
            {
                _panelAssets = await Addressables.LoadAssetAsync<PanelAssets>(
                    "Assets/Scripts/4_RoomManager/PanelAssets.asset"
                ).Task;
            }
        }

    }

    
}