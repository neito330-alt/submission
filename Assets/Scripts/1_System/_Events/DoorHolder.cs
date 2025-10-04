using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class DoorHolder : MonoBehaviour
{
    [SerializeField] private string _eventName;
    public string EventName
    {
        get => _eventName;
        set => _eventName = value;
    }

    public void SetLock(bool isLocked)
    {
        // foreach (var door in doors)
        // {
        //     if (door == null)
        //     {
        //         doors.Remove(door);
        //         continue;
        //     }
        //     foreach (DoorEventHandler handler in GetHandlers<DoorEventHandler>())
        //     {
        //         handler.SetLock(Rooms.DoorSystem.DoorLockType.Event, EventName, isLocked);
        //     }
        // }
    }
}
