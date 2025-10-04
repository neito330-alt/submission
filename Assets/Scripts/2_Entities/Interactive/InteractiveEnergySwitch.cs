using UnityEngine;


public class InteractiveEnergySwitch : InteractiveObject
{

    [SerializeField]
    private GimmickEnergyLine _energyLine;

    [SerializeField]
    private Renderer _renderer;

    private Material _material;

    [SerializeField]
    private AudioSource _generatorAudio;

    [SerializeField]
    private bool _isNext;
    public bool IsNext
    {
        get => _isNext;
        set
        {
            _isNext = value;
            GetComponent<Animator>().SetBool("IsOn", _isNext);
            GetComponent<AudioSource>().Play();
        }
    }

    [SerializeField]
    private bool _isOn;
    public bool IsOn
    {
        get => _isOn;
        set
        {
            _isOn = value;
            _energyLine.IsPower = _isOn;
        }
    }

    [SerializeField]
    private int _current = 0;

    void Start()
    {
        _material = _renderer.material;
    }

    void Update()
    {
        if (IsNext != IsOn)
        {
            int next = IsNext ? 100 : 0;
            if (_current < next)
            {
                _current ++;
            }
            else if (_current > next)
            {
                _current --;
            }
            if (_current == next)
            {
                IsOn = IsNext;
            }
            _material.SetFloat("_Emission",15* (_current / 100f));
            _renderer.material = _material;
            if (_generatorAudio != null)
            {
                _generatorAudio.volume = _current / 100f;
            }
        }
    }

    // public override void Interact()
    // {
    //     IsNext = !IsNext;
    // }

    void OnDestroy()
    {
        Destroy(_material);
    }

}
