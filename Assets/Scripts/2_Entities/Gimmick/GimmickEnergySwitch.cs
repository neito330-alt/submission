using System.Collections;
using System.Collections.Generic;
using Rooms.Auto;
using Rooms.RoomSystem;
using UnityEngine;

public class GimmickEnergySwitch : GimmickController
{
    [SerializeField]
    private InteractiveObject _interactiveObject;

    [SerializeField] private GimmickGenerator _gimmickGenerator;
    [SerializeField] private GimmickEnergyLine _energyLine;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Animator _animator;

    [SerializeField] private Color _energyColor = Color.white;


    [SerializeField]
    private bool _isNext;
    public bool IsNext
    {
        get => _isNext;
        set
        {
            _isNext = value;
            _animator.SetBool("IsOn", _isNext);
            _audioSource.Play();
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
    private float _current = 0;
    public float Current
    {
        get => _current;
        set
        {
            _current = value;
        }
    }

    
    // Start is called before the first frame update
    void Start()
    {
        _interactiveObject.Interact += Interact;

        _gimmickGenerator.EnergyColor = _energyColor;
        _gimmickGenerator.EnergyValue = _current;
        
        _energyLine.EnergyColor = _energyColor;
        _energyLine.State = IsOn;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsNext != IsOn)
        {
            _gimmickGenerator.EnergyValue = _current;
            if (_current == 0) IsOn = false;
            if (_current == 1) IsOn = true;
        }
    }

    public override void ResetGimmick()
    {
        base.ResetGimmick();
    }

    public override void RefreshGimmick()
    {
        base.RefreshGimmick();
    }

    void Interact()
    {
        IsNext = !IsNext;
    }

    public override void SetUp(RoomSetController roomSetController)
    {
        base.SetUp(roomSetController);
        _gimmickGenerator.decalData.color = _energyColor;
    }
}
