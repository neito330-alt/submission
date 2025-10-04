using System;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;


public class GimmickEnergyLine : MonoBehaviour
{
    private DoorHolder _doorHolder;
    [SerializeField]
    private EventObjectsHolder _eventObjectHolder;
    
    [SerializeField]
    private List<GimmickEnergyLine> _energyLines = new List<GimmickEnergyLine>();
    public List<GimmickEnergyLine> EnergyLines => _energyLines;

    public Material line;


    [SerializeField]
    private Color _energyColor = Color.white;
    public Color EnergyColor
    {
        get => _energyColor;
        set
        {
            _energyColor = value;
        }
    }

    [SerializeField]
    private bool _state = false;
    public bool State
    {
        get => _state;
        set
        {
            foreach (GimmickEnergyLine line in _energyLines)
            {
                if (line == this) continue;
                if (value) line.EnergyColor = _energyColor;
                line.NextState = value;
            }

            _state = value;
            _eventObjectHolder.EventTrigger(!_state);
        }
    }

    private bool _isPower = false;
    public bool IsPower
    {
        get => _isPower;
        set
        {
            _isPower = value;
            NextState = _isPower;
        }
    }

    [SerializeField]
    private MeshRenderer _meshRenderer;

    public bool NextState
    {
        get => _nextState;
        set
        {
            _nextState = value;
        }
    }

    [SerializeField]
    private bool _nextState = false;

    [SerializeField]
    private int _current = 0;

    
    private void Start()
    {
        line = _meshRenderer.material;
        line.color = _state ? _energyColor : Color.black;
        _meshRenderer.material = line;
        StateUpdate();
    }

    private void Update()
    {
        if (_nextState != _state)
        {
            int next = _nextState ? 100 : 0;
            if (_current == next)
            {
                State = _nextState;
            }
            else if (_current < next)
            {
                _current++;
                if (_current == 100) State = true;
            }
            else if (_current > next)
            {
                _current--;
                if (_current == 0) State = false;
            }
            line.color = Color.Lerp(Color.black, _energyColor, _current / 100f);
            _meshRenderer.material = line;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<GimmickEnergyLine>())
        {
            _energyLines.Add(other.gameObject.GetComponent<GimmickEnergyLine>());
            StateUpdate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<GimmickEnergyLine>())
        {
            _energyLines.Remove(other.gameObject.GetComponent<GimmickEnergyLine>());
            StateUpdate();
        }
    }

    public void StateUpdate()
    {
        bool flag = false;
        foreach (GimmickEnergyLine line in _energyLines)
        {
            flag |= line.State;
        }
        if (flag)NextState = flag;
    }

    void OnDestroy()
    {
        Destroy(line);
    }
}