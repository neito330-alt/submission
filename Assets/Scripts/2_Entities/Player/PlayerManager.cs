using System.Collections.Generic;
using Rooms;
using Rooms.PanelSystem;
using Rooms.RoomSystem;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;


public enum PlayerMode
{
    None = -1,
    Field = 0,
    Panel = 1,
    Map = 2,
    Menu = 3,
}


public class PlayerManager: MonoBehaviour
{
    [SerializeField]
    private PlayerMode _mode = PlayerMode.None;
    public PlayerMode Mode
    {
        get => _mode;
        set
        {
            if (value == _mode) return;
            // new value
            switch (value)
            {
                case PlayerMode.Field:
                    FieldCameraController.enabled = true;
                    // Activate field mode
                    break;

                case PlayerMode.Panel:
                    PanelCameraController.enabled = true;
                    // Activate panel mode
                    break;

                case PlayerMode.Map:
                    MapCameraController.enabled = true;
                    // Activate map mode
                    break;

                case PlayerMode.Menu:
                    MenuCameraController.enabled = true;
                    // Activate menu mode
                    break;
                default:
                    break;
            }

            // before value
            switch (_mode)
            {
                case PlayerMode.Field:
                    FieldCameraController.enabled = false;
                    // Deactivate field mode
                    break;

                case PlayerMode.Panel:
                    PanelCameraController.enabled = false;
                    // Deactivate panel mode
                    break;

                case PlayerMode.Map:
                    MapCameraController.enabled = false;
                    // Deactivate map mode
                    break;

                case PlayerMode.Menu:
                    // Deactivate menu mode
                    MenuCameraController.enabled = false;
                    break;
                default:
                    break;
            }
            _mode = value;
        }


    }

    [SerializeField]
    private List<RoomDataController> _roomDataControllers;

    [SerializeField]
    private FieldCameraController _fieldCameraController;
    public FieldCameraController FieldCameraController
    {
        get => _fieldCameraController;
        set => _fieldCameraController = value;
    }

    public static PanelCameraController _panelCameraController;
    public PanelCameraController PanelCameraController
    {
        set => _panelCameraController = value;
        get => _panelCameraController;
    }

    public  MenuCameraController MenuCameraController
    {
        get => GameManager.UIObject;
    }

    public MapCameraController MapCameraController
    {
        get => GameManager.stageManager.mapCameraController;
    }


    private Rooms.RoomSystem.RoomManager _roomManager;

    [SerializeField]
    private Vector2Int _position;
    public Vector2Int Position
    {
        get => _position;
        set
        {
            if (_position == value) return;
            _position = value;
            LightUpdate();
        }
    }

    public void LightUpdate(bool isFast = false)
    {
        foreach (RoomDataController room in _roomDataControllers)
        {
            if (room == null) continue;
            room.RoomDistanceState = RoomDistanceState.Far;
        }
        _roomDataControllers.Clear();

        for (int i = 0; i<5 ; i++)
        {
            Vector2Int pos;
            RoomDistanceState state = RoomDistanceState.Near;
            switch (i)
            {
                case 0:
                    pos = new Vector2Int(Position.x, Position.y);
                    state = RoomDistanceState.Current;
                    break;
                case 1:
                    pos = new Vector2Int(Position.x - 1, Position.y);
                    break;
                case 2:
                    pos = new Vector2Int(Position.x + 1, Position.y);
                    break;
                case 3:
                    pos = new Vector2Int(Position.x, Position.y - 1);
                    break;
                case 4:
                    pos = new Vector2Int(Position.x, Position.y + 1);
                    break;
                default:
                    pos = Vector2Int.zero;
                    break;
            }
            SlotData slotData = GameManager.stageManager.stageDataController.GetPanelSlotData(pos);
            if (slotData != null)
            {
                _roomDataControllers.Add(slotData.RoomSetData?.RoomDataController);
                if (slotData.RoomSetData?.RoomDataController?.RoomDistanceState == null) continue;
                slotData.RoomSetData.RoomDataController.RoomDistanceState = state;
                if (isFast && slotData.RoomSetData.RoomDataController.lightParentController.GetComponent<Animator>())slotData.RoomSetData.RoomDataController.lightParentController.GetComponent<Animator>().SetTrigger("FastEvent");
            }
        }
    }

    void Awake()
    {
        GameManager.playerManager = this;
    }

    void Start()
    {
        // Set the initial mode
        _roomManager = GameManager.stageManager.roomManager;
        Mode = PlayerMode.Field;
    }

    void Update()
    {
        Vector3 pos = transform.position;
        pos.x += 4;
        pos.z -= 4;
        Position = new Vector2Int(Mathf.FloorToInt(pos.x / 8)-1, Mathf.FloorToInt(-pos.z / 8)-1);
    }







    void OnEnable()
    {
        if (GameManager.IsExist())
        {
            GameManager.playerManager = this;
        }
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetPosition(Vector3 position)
    {
        CharacterController characterController = GetComponent<CharacterController>();
        characterController.enabled = false;
        transform.position = position;
        characterController.enabled = true;
        LightUpdate(true);
    }




}
