using UnityEngine;

public class InteractiveWallButton : InteractiveObject
{
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
            _renderer.material.color = _isActive ? _enableColor : _disableColor;
            _renderer.transform.localPosition = new Vector3(0, _isActive ? 0 : 0.02f, 0);
        }
    }


    void Awake()
    {
        _doorHolder = GetComponent<EventObjectsHolder>();
        _materialCache = new Material(_material);
    }

    void OnDestroy()
    {
        Destroy(_materialCache);
    }


    // public override void Interact()
    // {
    //     IsActive = false;
    //     IsInteractable = false;

    //     _audioSource.Play();
    //     _doorHolder.EventTrigger(false);
    // }
}
