using System.Collections.Generic;
using Rooms.DoorSystem;
using UnityEngine;



public class DoorEventHandler : GimmickEventHandler
{
    [SerializeField] private InteractiveDoor door;

    public override void OnEvent(string eventName, bool isLocked)
    {
        Debug.Log($"DoorEventHandler: Setting lock for door '{door.name}' with event '{eventName}' to {isLocked}");
        door.LockData.SetLock(DoorLockType.Event, eventName, isLocked);
    }
}
