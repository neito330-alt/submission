using Rooms.Auto;
using Rooms.RoomSystem;
using UnityEngine;

namespace Rooms.PanelSystem
{
    public class GimmickPazzle : GimmickController
    {
        [SerializeField] private InteractiveObject interactiveObject;
        [SerializeField] private PanelCameraController panelCameraController;

        void Start()
        {
            interactiveObject.Interact += Interact;
        }

        public void Interact()
        {
            PlayerManager._panelCameraController = panelCameraController;
            GameManager.playerManager.Mode = PlayerMode.Panel;
        }


#if UNITY_EDITOR
        public override void SetUp(RoomSetController roomSetController)
        {
            base.SetUp(roomSetController);

            panelCameraController.LightParentController = roomSetController.roomDataController.lightParentController;

        }
        #endif
    }
}
