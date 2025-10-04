using System;
using UnityEngine;

public class InteractionEvents : SingletonMonoBehaviour<InteractionEvents>
{
    public event Action DoorInteracted;
    public void OnDoorInteracted() => DoorInteracted?.Invoke();

    public event Action ValveInteracted;
    public void OnValveInteracted() => ValveInteracted?.Invoke();

    public event Action KeyInteracted;
    public void OnKeyInteracted() => KeyInteracted?.Invoke();

    public event Action BlueButtonInteracted;
    public void OnBlueButtonInteracted() => BlueButtonInteracted?.Invoke();

    public event Action RedButtonInteracted;
    public void OnRedButtonInteracted() => RedButtonInteracted?.Invoke();

    public event Action<Rooms.DoorSystem.DoorLockColor> ColorButtonInteracted;
    public void OnColorButtonInteracted(Rooms.DoorSystem.DoorLockColor color) => ColorButtonInteracted?.Invoke(color);

    public event Action<string,bool> EventInteracted;
    public void OnEventInteracted(string eventName,bool state) => EventInteracted?.Invoke(eventName,state);
}
