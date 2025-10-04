using System;
using UnityEngine;


public class InteractiveObject : MonoBehaviour
{
    public event Action Interact;
    public void OnInteract() => Interact?.Invoke();


    protected bool _isInteractable = true;
    public bool IsInteractable
    {
        get => _isInteractable;
        set => _isInteractable = value;
    }
}
