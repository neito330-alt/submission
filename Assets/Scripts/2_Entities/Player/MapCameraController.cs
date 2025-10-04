using UnityEngine;
using System.Collections.Generic;
using System;
using Rooms.PanelSystem;


[CreateAssetMenu(fileName = "MapAssets", menuName = "ScriptableObjects/MapAssets", order = 1)]
public class MapAssets : ScriptableObject
{
    [Serializable]
    public class MapDecalData
    {
        public Texture2D decalTexture;
        public Sprite decalSprite
        {
            get
            {
                if (decalTexture == null) return null;
                return Sprite.Create(
                    decalTexture,
                    new Rect(0, 0, decalTexture.width, decalTexture.height),
                    new Vector2(0f, 0f)
                );
            }
        }
        public float scale = 1f;
    }

    [Serializable]
    public class MapDecalColorData
    {
        public Color color;
        public float emission = 1f;
    }

    public Material decalMaterial;

    public List<MapDecalData> decals;
    [SerializeField]
    private int _decalIndex = 0;
    public int DecalIndex
    {
        get => _decalIndex;
        set
        {
            if (value < 0)
            {
                _decalIndex = decals.Count - 1;
            }
            else if (value >= decals.Count)
            {
                _decalIndex = 0;
            }
            else
            {
                _decalIndex = value;
            }
        }
    }
    public MapDecalData CurrentDecal
    {
        get => decals[_decalIndex];
    }


    public List<MapDecalColorData> decalColors;
    [SerializeField]
    private int _decalColorIndex = 0;
    public int DecalColorIndex
    {
        get => _decalColorIndex;
        set
        {
            if (value < 0)
            {
                _decalColorIndex = decalColors.Count - 1;
            }
            else if (value >= decalColors.Count)
            {
                _decalColorIndex = 0;
            }
            else
            {
                _decalColorIndex = value;
            }
        }
    }
    public MapDecalColorData CurrentDecalColor
    {
        get => decalColors[_decalColorIndex];
    }
}



public class MapCameraController : MonoBehaviour
{
    [SerializeField]
    private MapAssets mapAssets;


    [SerializeField]
    private GameObject mapParent;


    [SerializeField]
    private MapUIController mapUIController;

    [SerializeField]
    public GameObject mapIcon_Pointer;

    [SerializeField]
    public GameObject mapIcon_Player;

    public static float mouseSensitivity = 70f;
    public float MouseSensitivity
    {
        get => mouseSensitivity;
        set => mouseSensitivity = value;
    }
    public const float mouseCoefficient = 0.006f;


    void OnEnable()
    {
        mapParent.SetActive(true);

        mapUIController.Sprite = mapAssets.CurrentDecal.decalSprite;
        mapUIController.Color = mapAssets.CurrentDecalColor.color;

        mapIcon_Pointer.transform.localPosition = new Vector3(0, 0.5f, 0);

        mapIcon_Player.transform.localEulerAngles = new Vector3(0, GameManager.playerManager.transform.rotation.eulerAngles.y, 0);
        mapIcon_Player.transform.localPosition = GameManager.stageManager.panelManager.Offset
            + GameManager.playerManager.transform.localPosition / 8
            + new Vector3(-1, 0.4f, 1);
    }

    private void OnDisable()
    {
        mapParent.SetActive(false);
    }


    private void Update()
    {
        // 入力を取得
        Vector3 vector = Vector3.zero;
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f) vector.x = Input.GetAxis("Horizontal") * mouseSensitivity * mouseCoefficient * Time.deltaTime;
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f) vector.z = Input.GetAxis("Vertical") * mouseSensitivity * mouseCoefficient * Time.deltaTime;

        // ポインターを移動
        Vector2Int size = GameManager.stageManager.stageDataController.Size;

        mapIcon_Pointer.transform.Translate(vector);
        mapIcon_Pointer.transform.localPosition = new Vector3(
            Mathf.Clamp(mapIcon_Pointer.transform.localPosition.x, -size.x * 0.5f, size.x * 0.5f),
            mapIcon_Pointer.transform.localPosition.y,
            Mathf.Clamp(mapIcon_Pointer.transform.localPosition.z, -size.y * 0.5f, size.y * 0.5f)
        );

        // コントローラー入力時の動作

        // デカール設置
        if (Input.GetButtonDown("Action"))
        {
            // ポインターから真下にレイキャストを飛ばして、パネルに当たったらデカールを追加
            RaycastHit[] hits;

            Debug.DrawRay(mapIcon_Pointer.transform.position, new Vector3(0, -10, 0), Color.green, 10f);

            hits = Physics.RaycastAll(mapIcon_Pointer.transform.position, new Vector3(0, -1, 0), 10f);

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.tag == "Panel")
                {
                    // 衝突位置をパネルのローカル座標に変換
                    Vector3 delta = mapIcon_Pointer.transform.position - hit.transform.position;
                    delta.x *= 1 / hit.transform.lossyScale.x;
                    delta.z *= 1 / hit.transform.lossyScale.z;

                    Debug.Log($"Adding decal at position: {delta.x}, {delta.z} on panel: {hit.transform.name}");

                    Material material = new Material(mapAssets.decalMaterial);
                    material.SetTexture("_MainTex", mapAssets.CurrentDecal.decalTexture);
                    material.SetColor("_Color", mapAssets.CurrentDecalColor.color);
                    material.SetFloat("_Emission", mapAssets.CurrentDecalColor.emission);

                    mapAssets.decalMaterial.enableInstancing = true;
                    DecalItem decalItem = new DecalItem()
                    {
                        position = new Vector2(delta.x, delta.z),
                        scale = mapAssets.CurrentDecal.scale,
                        material = material,
                        canDelete = true,
                    };

                    //デカールを全てのパネルに追加
                    Rooms.RoomsManager.OnDecalAdded(
                        hit.transform.GetComponent<PanelDataController>().roomSetData.Position,
                        decalItem
                    );
                }
            }
        }
        // デカール削除
        else if (Input.GetButtonDown("Func2"))
        {
            RaycastHit[] hits;

            Debug.DrawRay(mapIcon_Pointer.transform.position, new Vector3(0, -10, 0), Color.green, 10f);

            hits = Physics.RaycastAll(mapIcon_Pointer.transform.position, new Vector3(0, -1, 0), 10f);

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.tag == "Panel")
                {
                    Vector3 delta = mapIcon_Pointer.transform.position - hit.transform.position;
                    delta.x *= 1 / hit.transform.lossyScale.x;
                    delta.z *= 1 / hit.transform.lossyScale.z;

                    hit.transform.GetComponent<PanelDataController>().RemoveDecal(
                        new Vector2(delta.x, delta.z)
                    );

                    Rooms.RoomsManager.OnDecalRemoved(
                        hit.transform.GetComponent<PanelDataController>().roomSetData.Position,
                        new Vector2(delta.x, delta.z)
                    );
                }
            }
        }


        // デカール設定
        if (Input.GetButtonDown("toRight"))
        {
            mapAssets.DecalIndex++;
            mapUIController.Sprite = mapAssets.CurrentDecal.decalSprite;
        }
        else if (Input.GetButtonDown("toLeft"))
        {
            mapAssets.DecalIndex--;
            mapUIController.Sprite = mapAssets.CurrentDecal.decalSprite;
        }

        // デカールカラー設定
        if (Input.GetButtonDown("Func1"))
        {
            mapAssets.DecalColorIndex++;
            mapUIController.Color = mapAssets.CurrentDecalColor.color;
        }

        // 戻る
        if (Input.GetButtonDown("Cancel"))
        {
            GameManager.playerManager.Mode = PlayerMode.Field;
        }
    }
}
