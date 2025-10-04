using Rooms.RoomSystem;
using UnityEngine;

public class GimmickPool : GimmickController
{
    [SerializeField]
    private GameObject _pool;

    void Start()
    {
        InteractionEvents.Instance.ValveInteracted += OnValve;
    }

    void OnValve()
    {
        _pool.SetActive(false);
    }

    public override void ResetGimmick()
    {
        _pool.SetActive(true);
    }

    public override void RefreshGimmick()
    {
        // No specific refresh logic for the pool
    }

    void OnDestroy()
    {
        if(InteractionEvents.IsExist()) InteractionEvents.Instance.ValveInteracted -= OnValve;
    }
}
