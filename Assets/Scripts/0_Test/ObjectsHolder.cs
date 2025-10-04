using System.Collections.Generic;
using UnityEngine;

public class ObjectsHolder : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _objects;
    public List<GameObject> Objects
    {
        get => _objects;
        set => _objects = value;
    }
}