using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Rooms.DoorSystem;

[Serializable]
public class TestClass_1
{
    [SerializeField]
    private int _age = 0;
    public int Age
    {
        get => _age;
        set => _age = value;
    }
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(TestClass_1))]
public class TestClassDrawer_1 : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        var ageProperty = property.FindPropertyRelative("_age");
        EditorGUI.PropertyField(position, ageProperty, new GUIContent("Age"));
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_age"), true);
    }
}
#endif



public class TestScript_1 : MonoBehaviour
{
    [SerializeField]
    private DoorLockDataList doorLockData;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
