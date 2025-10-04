using UnityEngine;


public class FieldCameraController : MonoBehaviour
{
    [SerializeField, CustomLabel("移動速度")]
    private float speedFactor = 1f;
    public float SpeedFactor
    {
        get => speedFactor;
        set => speedFactor = value;
    }

    [SerializeField, CustomLabel("マウス感度")]
    public static float mouseSensitivity = 70f;
    public float MouseSensitivity
    {
        get => mouseSensitivity;
        set => mouseSensitivity = value;
    }
    [SerializeField, CustomLabel("マウス感度係数")]
    private const float mouseCoefficient = 2f;

    [SerializeField, CustomLabel("パッド感度")]
    public static float padSensitivity = 70f;
    public float PadSensitivity
    {
        get => padSensitivity;
        set => padSensitivity = value;
    }
    [SerializeField, CustomLabel("パッド感度係数")]
    private const float padCoefficient = 15f;


    [SerializeField, CustomLabel("メインカメラ")]
    private FieldUIController fieldUIController;


    [SerializeField, CustomLabel("カメラ")]
    private Camera MainCamera;

    [SerializeField, CustomLabel("キャラクターコントローラー")]
    private CharacterController characterController;

    [SerializeField, CustomLabel("ポインター")]
    private GameObject _pointer;

    public LayerMask layerMask;

    private float _gravity = 0;

    static bool _isPointerEnabled = true;


    void Start()
    {

    }

    void OnEnable()
    {
        MainCamera.gameObject.SetActive(true);
        fieldUIController.gameObject.SetActive(true);
        GameManager.uiManager.fieldUI.Pointer.SetActive(true && _isPointerEnabled);
        //_pointer.SetActive(true);
    }

    void OnDisable()
    {
        MainCamera.gameObject.SetActive(false);
        fieldUIController.gameObject.SetActive(false);
        if (GameManager.uiManager != null)GameManager.uiManager.fieldUI.Pointer.SetActive(false);
        //_pointer.SetActive(false);
    }

    private void Update()
    {
        Vector2 angleInput = Vector2.zero;
        angleInput.x = Mathf.Abs(Input.GetAxis("Mouse X")) > Mathf.Abs(Input.GetAxis("Pad X")) ?
            Input.GetAxis("Mouse X") * mouseSensitivity * mouseCoefficient :
            Input.GetAxis("Pad X") * padSensitivity * padCoefficient;

        angleInput.y = Mathf.Abs(Input.GetAxis("Mouse Y")) > Mathf.Abs(Input.GetAxis("Pad Y")) ?
            Input.GetAxis("Mouse Y") * mouseSensitivity * mouseCoefficient :
            Input.GetAxis("Pad Y") * padSensitivity * padCoefficient;

        angleInput *= Time.deltaTime;

        Vector3 newAngle = transform.localEulerAngles;
        newAngle.y += angleInput.x;
        transform.localEulerAngles = newAngle;

        newAngle = MainCamera.transform.localEulerAngles;
        newAngle.x -= angleInput.y;
        if ((newAngle.x + 360) % 360 > 85 && (newAngle.x + 360) % 360 < 275)
            newAngle.x = (newAngle.x + 360) % 360 < 180 ? 85 : 275;

        MainCamera.transform.localEulerAngles = newAngle;


        Vector2 move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal");
        move.y = Input.GetAxis("Vertical");

        Vector3 delta = Vector3.zero;
        float rad = transform.localEulerAngles.y * Mathf.Deg2Rad;
        Vector2 normal = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        delta.x = move.x * normal.x + move.y * normal.y;
        delta.z = move.x * -normal.y + move.y * normal.x;
        delta *= Input.GetButton("Dash") ? 2f : 1f;
        delta *= speedFactor;

        bool flag = false;
        foreach (Collider col in Physics.OverlapSphere(transform.position, 0.5f, LayerMask.GetMask("Gimmick")))
        {
            if (col.transform.tag == "Ladder" && move.y > 0)
            {
                if (col.transform.GetComponent<GimmickLadder>())
                {
                    GimmickLadder ladder = col.transform.GetComponent<GimmickLadder>();
                    if (transform.position.y >= ladder.bottom && transform.position.y < ladder.top)
                    {
                        delta.y = 2f;
                        _gravity = 0;
                        flag = true;
                        break;
                    }
                }
            }
        }

        if (!flag)
        {
            if (Input.GetKey(KeyCode.M) || Input.GetKey(KeyCode.Joystick1Button5))
            {
                delta.y += - Physics.gravity.y * 20 * Time.deltaTime;
                _gravity = 0;
            }
            else
            {
                if (characterController.isGrounded)
                {
                    _gravity = 0;
                }
                else
                {
                    _gravity += Physics.gravity.y * 3* Time.deltaTime;
                }
                delta.y += _gravity;
            }
            if (Input.GetKey(KeyCode.LeftShift)) move.y -= 1f;
        }

        characterController.Move(delta * Time.deltaTime);

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        RaycastHit hit;

        if (_isPointerEnabled)fieldUIController.PointerColor = new Color(1f, 1f, 1f);
        if (Physics.Raycast(ray, out hit, 2f, layerMask))
        {
            if (hit.transform.GetComponent<InteractiveObject>())
            {
                if (hit.transform.GetComponent<InteractiveObject>().IsInteractable)
                {
                    if (Input.GetButtonDown("Action"))
                    {
                        hit.transform.GetComponent<InteractiveObject>().OnInteract();
                    }
                    if (_isPointerEnabled)fieldUIController.PointerColor = new Color(1f, 0.5f, 0f);
                }
            }
        }

        if (Input.GetButtonDown("Option"))
        {
            GameManager.playerManager.Mode = PlayerMode.Menu;
        }
        else if(Input.GetButtonDown("Map"))
        {
            GameManager.playerManager.Mode = PlayerMode.Map;
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            _isPointerEnabled = !_isPointerEnabled;
            if (GameManager.uiManager != null) GameManager.uiManager.fieldUI.Pointer.SetActive(_isPointerEnabled);
            //_pointer.SetActive(_isPointerEnabled);
        }

    }

}
