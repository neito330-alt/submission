using System.Collections;
using System.Collections.Generic;
using Rooms.RoomSystem;
using UnityEngine;

public class GimmickWallButton : GimmickController
{
    [SerializeField]
    private InteractiveObject _interactiveObject;
    [SerializeField]
    private EventObjectsHolder _doorHolder;

    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private Renderer _renderer;

    [SerializeField]
    private Material _material;

    private Material _materialCache;

    [SerializeField]
    private Color _enableColor;
    [SerializeField]
    private Color _disableColor;

    [SerializeField]
    private bool _isActive = true;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            _materialCache.color = _isActive ? _enableColor : _disableColor;
            _materialCache.SetColor("_EmissionOfColor", _isActive ? _enableColor : _disableColor);
            _materialCache.SetFloat("_Emission", _isActive ? 5 : 0);
            _renderer.transform.localPosition = new Vector3(0, _isActive ? 0.04f : 0.02f, 0);
        }
    }

    void Awake()
    {
        _materialCache = new Material(_material);
    }


    void Start()
    {
        IsActive = _isActive;

        _renderer.sharedMaterial = _materialCache;

        _interactiveObject.Interact += Interact;
    }

    void OnDestroy()
    {
        Destroy(_materialCache);
    }


    public override void ResetGimmick()
    {
        base.ResetGimmick();
        IsActive = true;
    }

    public override void RefreshGimmick()
    {
        base.RefreshGimmick();
        IsActive = true;
    }


    void Interact()
    {
        IsActive = false;
        _interactiveObject.IsInteractable = false;

        _audioSource.Play();
        _doorHolder.EventTrigger(false);
    }
}
