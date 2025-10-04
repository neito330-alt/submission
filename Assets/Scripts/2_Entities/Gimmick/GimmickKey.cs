using System.Collections;
using System.Collections.Generic;
using Rooms.RoomSystem;
using UnityEngine;

public class GimmickKey : GimmickController
{
    [SerializeField]
    private InteractiveObject _interactiveObject;


    [SerializeField]
    private GameObject _key;

    [SerializeField]
    public AudioClip sound;

    [SerializeField]
    public PlayerItem item;



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
        _key.SetActive(true);
    }

    public override void RefreshGimmick()
    {
        base.RefreshGimmick();
    }

    void Interact()
    {
        _key.SetActive(false);
        PlayerStatus.Instance.AddItem(item, sound);
        InteractionEvents.Instance.OnKeyInteracted();
    }
}
