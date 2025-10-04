using Rooms.RoomSystem;
using UnityEngine;


public class GimmickConfineTrap : GimmickController
{
    [SerializeField] private EventObjectsHolder _eventObjectsHolder;

    [SerializeField] private BoxCollider _boxCollider;

    [SerializeField] private GameObject _door;

    void Awake()
    {
        _door.SetActive(false);
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player"){
            _eventObjectsHolder.EventTrigger(true);
            _boxCollider.enabled = true;
            _door.SetActive(true);
        }
    }

    public override void ResetGimmick()
    {
        base.ResetGimmick();

        _door.SetActive(false);
        _boxCollider.enabled = true;
    }

    public override void RefreshGimmick()
    {
        base.RefreshGimmick();

        _door.SetActive(false);
        _boxCollider.enabled = true;
    }
}
