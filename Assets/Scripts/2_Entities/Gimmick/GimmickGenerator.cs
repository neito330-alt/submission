using Rooms.Auto;
using Rooms.RoomSystem;
using UnityEngine;

class GimmickGenerator : GimmickController
{
    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private Material _genericMaterial;

    [SerializeField]
    private Color _energyColor;
    public Color EnergyColor
    {
        get => _energyColor;
        set
        {
            _energyColor = value;
            Color.RGBToHSV(_energyColor, out float h, out float s, out float v);
            _materialCash.SetColor("_BodyEmission", _energyColor);
            _materialCash.SetFloat("_Saturation", s);
            _materialCash.SetFloat("_Hue", Mathf.Repeat(h + 0.5f, 1f));
        }
    }

    [SerializeField]
    private float _energyValue = 0f;
    public float EnergyValue
    {
        get => _energyValue;
        set
        {
            _energyValue = value;
            _materialCash.SetFloat("_Emission", _energyValue * 15);
            _audioSource.volume = _energyValue;
        }
    }

    [SerializeField]
    private Material _materialCash;

    void Awake()
    {
        _genericMaterial.enableInstancing = true;
        _materialCash = new Material(_genericMaterial);

        Color.RGBToHSV(_energyColor, out float h, out float s, out float v);

        _materialCash.SetFloat("_Saturation", s);

        _materialCash.SetFloat("_Hue", Mathf.Repeat(h + 0.5f, 1f));

        EnergyValue = _energyValue;
    }

    void Start()
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.sharedMaterial = _materialCash;
        }
    }

    void OnDestroy()
    {
        Destroy(_materialCash);
    }



    
}
