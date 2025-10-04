using Rooms;
using Rooms.PanelSystem;
using Rooms.RoomSystem;
using UnityEngine;


public class PanelCameraController : MonoBehaviour
{
    [Header("パネルカメラ")]
    [SerializeField] private Camera _camera;
    [SerializeField] private PanelManager _panelManager;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private LightParentController _lightParentController;
    public LightParentController LightParentController
    {
        get => _lightParentController;
        set => _lightParentController = value;
    }


    [Space(10)]
    [Header("パネル関連")]

    [SerializeField,CustomLabel("カーソルオブジェクト")]
    private LineRenderer _cursorRenderer;

    [SerializeField, CustomLabel("ボリューム")]
    private GameObject _volumeController;

    [SerializeField, CustomLabel("パネルビューアー")]
    private GameObject _panelViewerController;

    [SerializeField, CustomLabel("パネルカメラ")]
    private GameObject _panelViewCamera;



    [Space(10)]
    [Header("カーソル")]

    [SerializeField]
    private Vector2Int _cursorPosition = new Vector2Int(0,0);
    /// <summary>
    /// カーソルのスロット座標
    /// </summary>
    public Vector2Int cursorPosition
    {
        get => _cursorPosition;
        set
        {
            if(_cursorPosition != value){
                _cursorPosition = value;
                CursorUpdate();
            }
        }
    }

    private PanelDataController _cursorPanel;
    /// <summary>
    /// カーソルのパネルデータ
    /// </summary>
    public PanelDataController CursorPanel
    {
        get => _cursorPanel;
        set
        {
            if (_cursorPanel != value)
            {
                if (_cursorPanel != null)
                {
                    _cursorPanel.panelRenderer.gameObject.layer = LayerMask.NameToLayer("Default");
                }
                _cursorPanel = value;
                if (_cursorPanel != null)
                {
                    _cursorPanel.panelRenderer.gameObject.layer = LayerMask.NameToLayer("highlight");
                }
            }
        }
    }

    private Material _cursorMaterial;


    [Space(10)]
    [Header("ホールド中のパネル")]

    /// <summary>
    /// ホールド時のカーソルのスロット座標
    /// </summary>
    private Vector2Int _holdPanelPosition = new Vector2Int(0,0);

    /// <summary>
    /// ホールドされているパネルの本体
    /// </summary>
    private PanelDataController _holdPanel;

    /// <summary>
    /// ホログラム中の部屋パネル
    /// </summary>
    private PanelDataController _holdingPanel;

    public bool IsHolding => (_holdPanel != null) && (_holdingPanel != null);

    private Material _holdGlowMaterial;
    private Material _holdHologramMaterial;



    [Space(10)]
    [Header("カーソルの状態")]

    /// <summary>
    /// 設置可能か
    /// </summary>
    private bool _canPlace;

    /// <summary>
    /// 入れ替え可能か
    /// </summary>
    private bool _canSwap;


    [Space(10)]
    [Header("カーソルのフォーカス")]


    private static bool _isCursorFocus = false;

    [SerializeField, CustomLabel("カーソルフォーカスフィルター")]
    private GameObject _focusFilter;


    [Space(10)]
    [Header("入力関連")]

    private Vector2Int _last = Vector2Int.zero;

    private bool _isEdit = false;

    void Awake()
    {
        PanelAssets panelAssets = RoomsAssetsManager.PanelAssets;

        _cursorMaterial = new Material(panelAssets.systemMaterials.cursorMaterial);
        _holdGlowMaterial = new Material(panelAssets.systemMaterials.glowMaterial);
        _holdHologramMaterial = new Material(panelAssets.systemMaterials.hologramMaterial);
    }



    void OnEnable()
    {
        _camera.gameObject.SetActive(true);

        _panelViewerController.SetActive(true);
        _panelViewCamera.gameObject.SetActive(true);

        _lightParentController.gameObject.SetActive(false);
        _volumeController.SetActive(true);

        _cursorRenderer.enabled = true;
        _focusFilter.SetActive(_isCursorFocus);
        _audioSource.PlayOneShot(RoomsAssetsManager.PanelAssets.panelSound.enterSound);

        _isEdit = false;

        CursorUpdate();
    }

    void OnDisable()
    {
        _camera.gameObject.SetActive(false);
        _panelViewCamera.gameObject.SetActive(false);

        _panelViewerController.SetActive(false);

        _volumeController.SetActive(false);

        _cursorRenderer.enabled = false;
        _focusFilter.SetActive(false);

        _lightParentController.gameObject.SetActive(true);

        if (_isEdit)
        {
            _audioSource.PlayOneShot(RoomsAssetsManager.PanelAssets.panelSound.exitSound);

            _panelManager.PanelApply();
            RoomsManager.OnRoomsUpdated();
        }
        else
        {
            Vector2Int pos = GameManager.playerManager.Position;
            SlotData data = GameManager.stageManager.stageDataController.GetPanelSlotData(pos);

            if (data.RoomSetData != null)
            {
                _lightParentController.SetDistance(RoomDistanceState.Current);
            }
        }
    }

    void Start()
    {
        // カーソルの太さを設定
        float width = new Vector2(_cursorRenderer.transform.lossyScale.x, _cursorRenderer.transform.lossyScale.z).magnitude * 0.02f;
        _cursorRenderer.material = RoomsAssetsManager.PanelAssets.systemMaterials.cursorMaterial;
        _cursorRenderer.startWidth = width;
        _cursorRenderer.endWidth = width;

        _cursorRenderer.sharedMaterial = _cursorMaterial;

        cursorPosition = GameManager.stageManager.stageDataController.StartPosition;
    }

    void Update()
    {
        Vector2Int input = Vector2Int.zero;
        Vector2Int delta = Vector2Int.zero;
        if (0.1f < Input.GetAxis("Horizontal"))
        {
            input.x = +1;
            delta.x = _last.x == 1 ? 0 : 1;
        }
        if (-0.1f > Input.GetAxis("Horizontal"))
        {
            input.x = -1;
            delta.x = _last.x == -1 ? 0 : -1;
        }
        if (0.1f < Input.GetAxis("Vertical"))
        {
            input.y = -1;
            delta.y = _last.y == -1 ? 0 : -1;
        }
        if (-0.1f > Input.GetAxis("Vertical"))
        {
            input.y = +1;
            delta.y = _last.y == 1 ? 0 : 1;
        }

        if (_panelManager.GetPanelSlotData(cursorPosition + delta).isSlot && delta != Vector2Int.zero)
        {
            _audioSource.PlayOneShot(RoomsAssetsManager.PanelAssets.panelSound.moveSound);
            cursorPosition += delta;
        }
        else if (input != _last && input != Vector2Int.zero)
        {
            _audioSource.PlayOneShot(RoomsAssetsManager.PanelAssets.panelSound.stopSound);
        }
        _last = input;



        if (Input.GetButtonDown("toRight"))
        {
            if (IsHolding)
            {
                _audioSource.PlayOneShot(RoomsAssetsManager.PanelAssets.panelSound.rotateSound);
                _holdingPanel.Rotation += 90;
                _isEdit = true;
                CursorUpdate();
            }
            else if (_panelManager.GetPanelSlotData(cursorPosition).IsRotatable)
            {
                _audioSource.PlayOneShot(RoomsAssetsManager.PanelAssets.panelSound.rotateSound);
                _isEdit |= _panelManager.PanelRotate(cursorPosition, _panelManager.GetPanelSlotData(cursorPosition).panelDataController.Rotation+90);
            }
        }
        else if (Input.GetButtonDown("toLeft"))
        {
            if (IsHolding)
            {
                _audioSource.PlayOneShot(RoomsAssetsManager.PanelAssets.panelSound.rotateSound);
                _holdingPanel.Rotation -= 90;
                _isEdit = true;
                CursorUpdate();
            }
            else if (_panelManager.GetPanelSlotData(cursorPosition).IsRotatable)
            {
                _audioSource.PlayOneShot(RoomsAssetsManager.PanelAssets.panelSound.rotateSound);
                _isEdit |= _panelManager.PanelRotate(cursorPosition, _panelManager.GetPanelSlotData(cursorPosition).panelDataController.Rotation-90);
            }
        }

        if (Input.GetButtonDown("Action"))
        {
            PanelManager.SlotDataHolder slotData = _panelManager.GetPanelSlotData(cursorPosition);
            if (!IsHolding)
            {
                if (slotData.IsNotEmpty && slotData.IsHoldable)
                {
                    _audioSource.PlayOneShot(RoomsAssetsManager.PanelAssets.panelSound.pickSound);
                    CursorPick();
                    CursorUpdate();
                }
            }
            else
            {
                if (_canPlace)
                {
                    _audioSource.PlayOneShot(RoomsAssetsManager.PanelAssets.panelSound.placeSound);
                    CursorDrop();
                }
                else
                {
                    _audioSource.PlayOneShot(RoomsAssetsManager.PanelAssets.panelSound.errorSound);
                }
            }
        }
        if(Input.GetButtonDown("Cancel"))
        {
            if(! IsHolding){
                GameManager.playerManager.Mode = PlayerMode.Field;
            }
            else{
                CursorUndo();
                CursorUpdate();
            }
        }

        if (Input.GetButtonDown("Func2"))
        {
            _isCursorFocus = !_isCursorFocus;
            _focusFilter.SetActive(_isCursorFocus);
        }
    }

    void OnDestroy()
    {
        Destroy(_cursorMaterial);
        Destroy(_holdGlowMaterial);
        Destroy(_holdHologramMaterial);
    }


    void CursorUpdate()
        {
            CursorPanel = _panelManager.GetPanelSlotData(_cursorPosition)?.panelDataController;

            _cursorRenderer.transform.localPosition = _panelManager.GetWorldPosition(cursorPosition,1)+_panelManager._offset;

            _cursorRenderer.enabled = !IsHolding;

            if (IsHolding)
            {
                PanelManager.SlotDataHolder cursorSlotData = _panelManager.GetPanelSlotData(cursorPosition);

                _canPlace = cursorSlotData.IsNotEmpty ? cursorSlotData.IsMoveable : cursorSlotData.isSlot;
                _canSwap = _canPlace && cursorSlotData != null && cursorSlotData.IsNotEmpty;

                _holdHologramMaterial.SetColor("_Color", (
                    _canSwap ?
                    new Color(1, 0.5f, 0, 0.25f) :
                    _canPlace ?
                    new Color(0, 1, 1, 0.25f) :
                    new Color(1, 0, 0, 0.25f)).SetEmission(3f));

                _panelViewCamera.transform.localPosition = _panelManager.GetWorldPosition(_holdPanelPosition)+_panelManager._offset;
            }
            else
            {
                _cursorRenderer.positionCount = 4;
                _cursorRenderer.SetPosition(0, new Vector3(0.5f, 0, 0.5f));
                _cursorRenderer.SetPosition(1, new Vector3(-0.5f, 0, 0.5f));
                _cursorRenderer.SetPosition(2, new Vector3(-0.5f, 0, -0.5f));
                _cursorRenderer.SetPosition(3, new Vector3(0.5f, 0, -0.5f));

                _panelViewCamera.transform.localPosition = _panelManager.GetWorldPosition(cursorPosition)+_panelManager._offset;
            }

            Color color = _panelManager.GetPanelData(_cursorPosition)?.slotData?.ToCursorColor() ?? Color.gray;

            _cursorMaterial.SetColor("_Color",color);
        }

    void CursorPick()
    {
        if (!IsHolding && _panelManager.GetPanelSlotData(_cursorPosition).panelDataController != null)
        {
            _holdPanelPosition = _cursorPosition;

            _holdPanel = _panelManager.GetPanelSlotData(_cursorPosition).panelDataController;

            _holdingPanel = Instantiate(_holdPanel.gameObject, _cursorRenderer.transform).GetComponent<PanelDataController>();

            _holdingPanel.transform.localPosition = Vector3.zero;
            _holdingPanel.transform.localScale = Vector3.one;
            _holdingPanel.transform.localEulerAngles = Vector3.zero;

            _holdingPanel.Rotation = _holdPanel.Rotation;

            _holdingPanel.panelRenderer.gameObject.SetActive(false);

            _holdingPanel.SetMaterial(_holdHologramMaterial);

            _holdPanel.SetMaterial(_holdGlowMaterial);

            CursorUpdate();
        }
    }

    void CursorDrop()
    {
        if (IsHolding)
        {
            _isEdit |= _panelManager.PanelRotate(_holdPanelPosition, _holdingPanel.Rotation);
            _isEdit |= _panelManager.PanelMove(_holdPanelPosition, _cursorPosition);

            _holdPanel.SetMaterial(null);
            _holdPanel = null;

            Destroy(_holdingPanel.gameObject);
            _holdingPanel = null;

            CursorUpdate();
        }
    }

    void CursorUndo()
    {
        if (IsHolding)
        {
            _holdPanel.SetMaterial(null);
            _holdPanel = null;

            Destroy(_holdingPanel.gameObject);
            _holdingPanel = null;

            CursorUpdate();
        }
    }
}
