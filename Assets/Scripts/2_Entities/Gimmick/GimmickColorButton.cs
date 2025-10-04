using System.Collections;
using System.Collections.Generic;
using Rooms;
using Rooms.DoorSystem;
using Rooms.RoomSystem;
using UnityEngine;

public class GimmickColorButton : GimmickController
{
    [SerializeField] private InteractiveObject _interactiveObject;
    [SerializeField] private GameObject _button;
    [SerializeField] private AudioSource _audioSource;



    [SerializeField] private DoorLockColor _buttonColor;

    


    [SerializeField] private bool _isActive = true;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            _interactiveObject.IsInteractable = value;
            _button.transform.localPosition = new Vector3(0,1.1f + (value ? 0 : -0.075f), 0);
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        _interactiveObject.Interact += Interact;
        InteractionEvents.Instance.ColorButtonInteracted += SetColor;
    }

    private void OnDestroy()
    {
        if (InteractionEvents.IsExist())
        {
            InteractionEvents.Instance.ColorButtonInteracted -= SetColor;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void ResetGimmick()
    {
        base.ResetGimmick();
        SetColor(RoomsManager.CurrentDoorLockColor);
    }

    public override void RefreshGimmick()
    {
        base.RefreshGimmick();
        SetColor(RoomsManager.CurrentDoorLockColor);
    }

    void Interact()
    {
        InteractionEvents.Instance.OnColorButtonInteracted(_buttonColor);
    }

    void SetColor(DoorLockColor color)
    {
        if (color == _buttonColor && _isActive)
        {
            _audioSource.Play();
            IsActive = false;
        }
        else if (!_isActive)
        {
            IsActive = true;
        }
    }
}
