using UnityEngine;


public class InteractiveColorButton : InteractiveObject
{
    private bool state = true;
    [SerializeField] Rooms.DoorSystem.DoorLockColor _color;
    public Rooms.DoorSystem.DoorLockColor Color => _color;

    [SerializeField]GameObject _button;

    private void Start()
    {
        InteractionEvents.Instance.ColorButtonInteracted += ToDisable;
    }
    // public override void Interact()
    // {
    //     if (state)
    //     {
    //         InteractionEvents.Instance.OnColorButtonInteracted(_color);
    //     }
    // }

    public void OnDestroy()
    {
        if (InteractionEvents.IsExist()) InteractionEvents.Instance.ColorButtonInteracted -= ToDisable;
    }

    public void ToDisable(Rooms.DoorSystem.DoorLockColor color)
    {
        if (color == _color)
        {
            GetComponent<AudioSource>().Play();
            if (state)_button.transform.position -= new Vector3(0, 0.075f, 0);
            state = false;
            IsInteractable = false;
        }
        else
        {
            if (!state)_button.transform.position += new Vector3(0, 0.075f, 0);
            state = true;
            IsInteractable = true;
        }
    }
}
