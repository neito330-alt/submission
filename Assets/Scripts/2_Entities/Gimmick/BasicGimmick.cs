using System.Collections;
using System.Collections.Generic;
using Rooms.RoomSystem;
using UnityEngine;

public class BasicGimmick : GimmickController
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void ResetGimmick()
    {
        base.ResetGimmick();
        gameObject.SetActive(true);
    }

    public override void RefreshGimmick()
    {
        base.RefreshGimmick();
        gameObject.SetActive(true);
    }
}
