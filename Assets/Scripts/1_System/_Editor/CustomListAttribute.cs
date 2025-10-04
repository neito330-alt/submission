using System;
using System.Collections.Generic;
using Rooms.Auto;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using UnityEngine;

[Serializable]
public class PropertyList<T>
{
    public List<T> list = new List<T>();
}

public class CustomListAttribute : PropertyAttribute {
    public readonly GUIContent Label;
    public readonly GUIContent IndexName;

    public CustomListAttribute(string label = null,string indexName = null) {
        if (label != null) Label = new GUIContent(label);
        if (indexName != null) IndexName = new GUIContent(indexName);
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(CustomListAttribute))]
public class CustomListDrawer : PropertyDrawer {
    private Dictionary<string, ReorderableList> reorderableLists = new Dictionary<string, ReorderableList>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        CustomListAttribute customListAttribute = (CustomListAttribute)attribute;
        // 配列プロパティを取得
        SerializedProperty arrayProperty = property.FindPropertyRelative("list");
        if (arrayProperty == null || !arrayProperty.isArray) {
            EditorGUI.HelpBox(position, arrayProperty == null ? "Array is not found! 0" : "Array is not found! 1", MessageType.Error);
            return;
        }

        // ReorderableListの初期化
        string key = property.propertyPath;
        if (!reorderableLists.ContainsKey(key)) {
            reorderableLists[key] = CreateReorderableList(arrayProperty, customListAttribute, label);
        }

        position.x += EditorGUI.indentLevel * 15; // ラベルの幅を考慮して位置を調整
        position.width -= EditorGUI.indentLevel * 15 - 1; // ラベルの幅を考慮して幅を調整

        // リストを描画
        reorderableLists[key].DoList(position);
    }

    private ReorderableList CreateReorderableList(SerializedProperty arrayProperty, CustomListAttribute customListAttribute, GUIContent label) {
        var list = new ReorderableList(
            arrayProperty.serializedObject,
            arrayProperty,
            draggable: true,
            displayHeader: true,
            displayAddButton: true,
            displayRemoveButton: true
        );

        list.drawHeaderCallback = (Rect rect) => {
            rect.x -= EditorGUI.indentLevel * 15; // ラベルの幅を考慮して位置を調整
            //rect.width += EditorGUI.indentLevel * 15; // ラベルの幅を考慮して幅を調整

            EditorGUI.LabelField(rect, customListAttribute.Label != null ? customListAttribute.Label : label, new GUIStyle("BoldLabel"));
        };

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            SerializedProperty element = arrayProperty.GetArrayElementAtIndex(index);
            rect.y += 2; // Adjust the y position to avoid overlap with the header
            rect.height = EditorGUI.GetPropertyHeight(element, true);
            rect.x += 15; // Add some padding to the left
            rect.width -= 15; // Adjust width to fit the label and field

            EditorGUI.PropertyField(rect, element, new GUIContent((customListAttribute.IndexName != null ? customListAttribute.IndexName.text : "要素")+ index), true);
        };

        list.elementHeightCallback = (int index) => {
            return EditorGUI.GetPropertyHeight(arrayProperty.GetArrayElementAtIndex(index)) + 4;
        };

        return list;
    }



    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (reorderableLists.TryGetValue(property.propertyPath, out var list)) {
            return list.GetHeight();
        }
        return EditorGUIUtility.singleLineHeight * 2; // エラー表示用の余裕
    }
}
#endif