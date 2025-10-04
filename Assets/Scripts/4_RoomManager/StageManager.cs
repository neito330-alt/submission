using Rooms.PanelSystem;
using Rooms.RoomSystem;
using UnityEngine;

namespace Rooms
{
    public class StageManager : MonoBehaviour
    {
        public StageDataController stageDataController;
        public RoomManager roomManager;
        public PanelManager panelManager;
        public MapCameraController mapCameraController;
        public Vector3 respawnPosition;


        void Awake()
        {
            GameManager.stageManager = this;
        }

        void OnEnable()
        {
        }

        void OnDisable()
        {
        }

        void Start()
        {
            respawnPosition = new Vector3(
                (stageDataController.StartPosition.x+1) * 8,
                0,
                -(stageDataController.StartPosition.y+1) * 8
            );

            if (stageDataController.IsShuffle)
            {
                stageDataController.PanelShuffle(stageDataController.Size.x * stageDataController.Size.y * 4);
            }

            // 部屋を生成
            BuildRoom();
        }

        public void RespawnPlayer()
        {
            GameManager.playerManager.SetPosition(respawnPosition + new Vector3(0, 1f, 0));
        }

        public void BuildRoom()
        {
            foreach(RoomDataController roomController in roomManager.GetComponentsInChildren<RoomDataController>())
            {
                Destroy(roomController.gameObject);
            }

            for (int y = 0; y < stageDataController.Size.y; y++)
            {
                for (int x = 0; x < stageDataController.Size.x; x++)
                {
                    SlotData slotData = stageDataController.Data[y][x];
                    if (slotData.IsNotEmpty)
                    {
                        GameObject room = Instantiate(
                            slotData.RoomSetData.RoomSetController.roomDataController.gameObject,
                            Vector3.zero,
                            Quaternion.identity,
                            roomManager.transform);

                        room.SetActive(true);
                        room.transform.localPosition = new Vector3();
                        room.transform.localScale = Vector3.one;
                        room.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        room.transform.localEulerAngles = new Vector3(0, slotData.RoomSetData.Rotation, 0);

                        room.GetComponent<RoomDataController>().RoomDistanceState = RoomDistanceState.Far;

                        slotData.RoomSetData.RoomDataController = room.GetComponent<RoomDataController>();
                    }
                }
            }

            PlayerStatus.Instance.ClearInventory();
            roomManager.ResetStage();

            RoomsManager.OnRoomsUpdated();

            RespawnPlayer();
        }
    }
}
