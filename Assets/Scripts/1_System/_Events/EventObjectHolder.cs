using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class EventObjectsHolder : MonoBehaviour
{
    [SerializeField] private string _eventName;
    public string EventName
    {
        get => _eventName;
        set => _eventName = value;
    }

    [SerializeField] private AudioClip _eventLockSound;
    public AudioClip EventLockSound
    {
        get => _eventLockSound;
        set => _eventLockSound = value;
    }
    [SerializeField] private AudioClip _eventUnlockSound;
    public AudioClip EventUnlockSound
    {
        get => _eventUnlockSound;
        set => _eventUnlockSound = value;
    }

    [SerializeField] private List<GimmickEventHandler> _objects;
    public List<GimmickEventHandler> Objects => _objects;

    public GimmickEventHandler[] GetHandlers<T>() where T : GimmickEventHandler
    {
        List<GimmickEventHandler> handlers = new List<GimmickEventHandler>();
        foreach (var obj in _objects)
        {
            if (obj is T handler)
            {
                handlers.Add(handler);
            }
        }
        return handlers.ToArray();
    }

    public void EventTrigger(bool state = true)
    {
        foreach (var obj in _objects)
        {
            if (obj == null)
            {
                _objects.Remove(obj);
                continue;
            }
            obj.OnEvent(_eventName, state);
        }
    }
}
