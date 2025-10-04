using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Rooms.Auto;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using UnityEngine;


[Serializable]
public class TestClass_0A
{
    public int intValue = 0;
    public string stringValue = "Default String";
}

[Serializable]
public class TestClass_0
{
    public List<TestClass_0A> intList;
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(TestClass_0))]
public class TestClassDrawer : PropertyDrawer
{
    ReorderableList reorderableList;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (reorderableList == null)
        {
            reorderableList = new ReorderableList(property.serializedObject, property.FindPropertyRelative("intList"), true, true, true, true);

            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) => 
            {
                var elementProperty = property.FindPropertyRelative("intList").GetArrayElementAtIndex(index);
                rect.y += 2; // Adjust the y position to avoid overlap with the header
                rect.height = EditorGUI.GetPropertyHeight(elementProperty, true);
                rect.x += 15; // Add some padding to the left
                rect.width -= 15; // Adjust width to fit the label and field
                EditorGUI.PropertyField(rect, elementProperty, new GUIContent("要素" + index), true);
            };
        
        }
        reorderableList.DoList(position);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (reorderableList == null)
        {
            return EditorGUIUtility.singleLineHeight;
        }
        return reorderableList.GetHeight();
    }
}
#endif


public class TestScript_0 : MonoBehaviour
{
    public RoomSetController roomSetController;
}