using System.Collections;
using System.Collections.Generic;
using Rooms.RoomSystem;
using UnityEngine;

public class GimmickValve : GimmickController
{
    [SerializeField]
    private InteractiveObject _interactiveObject;


    // Start is called before the first frame update
    void Start()
    {
        _interactiveObject.Interact += Interact;
    }

    // Update is called once per frame
    void Update()
    {

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
        InteractionEvents.Instance.OnValveInteracted();
        InteractionEvents.Instance.OnEventInteracted("Valve", false);
        Vector3 angle = transform.localEulerAngles;
        angle.x += 90f;
        transform.localEulerAngles = angle;
        GetComponent<AudioSource>().Play();
    }
}
