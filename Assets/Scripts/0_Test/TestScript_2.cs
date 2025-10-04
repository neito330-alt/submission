using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript_2 : MonoBehaviour
{
    [SerializeField, Button("ButtonEvent1")]
    private bool button1;
    [SerializeField, Button("ButtonEvent2")]
    private bool button2;

    [Space(10)]
    [Header("Cube Renderers")]
    [SerializeField]
    private Renderer _cube1;
    [SerializeField]
    private Renderer _cube2;
    [SerializeField]
    private Renderer _cube3;
    [SerializeField]
    private Renderer _cube4;
    [SerializeField]
    private Renderer _cube5;
    [SerializeField]
    private Renderer _cube6;
    [SerializeField]
    private Renderer _cube7;
    [SerializeField]
    private Renderer _cube8;
    [SerializeField]
    private Renderer _cube9;
    [SerializeField]
    private Renderer _cube10;

    [Space(10)]
    [Header("Cube Materials")]
    [SerializeField]
    private Material _material1;
    [SerializeField]
    private Material _material2;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ButtonEvent1()
    {
        Debug.Log("ButtonEvent1 called");
        _cube1.material = _material1;
        _cube2.sharedMaterial = _material1;

        _cube3.material = _material2;
        _cube4.sharedMaterial = _material2;
    }

    public void ButtonEvent2()
    {
        Material material = _cube1.sharedMaterial;
        material.color = Color.red;

        material = _cube3.material;
        material.color = Color.blue;
    }
}
