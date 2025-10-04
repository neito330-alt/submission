using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class NoFoldoutAttribute : PropertyAttribute
{
    public readonly GUIContent label;
    public readonly bool isIndented = true;
    public NoFoldoutAttribute(string label, bool isIndented = true)
    {
        this.label = new GUIContent(label);
        this.isIndented = isIndented;
    }

    public NoFoldoutAttribute(bool isIndented = true)
    {
        this.isIndented = isIndented;
    }

    //public NoFoldoutAttribute() { }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(NoFoldoutAttribute))]
public class NoFoldoutAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        NoFoldoutAttribute noFoldoutAttribute = (NoFoldoutAttribute)attribute;

        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(position, noFoldoutAttribute.label ?? label, EditorStyles.boldLabel);
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = noFoldoutAttribute.isIndented ? indent + 1 : indent;

        // 1. propertyのコピーを作る
        SerializedProperty iterator = property.Copy();

        // 2. 子プロパティに入る（最初は親そのもの）
        SerializedProperty endProperty = iterator.GetEndProperty();

        // 3. 最初の子プロパティに進む
        iterator.Next(true); // true: enterChildren

        float lineHeight = EditorGUIUtility.singleLineHeight;
        Rect fieldRect = new Rect(position.x, position.y, position.width, lineHeight);

        // 4. endPropertyに達するまで繰り返す
        while (!SerializedProperty.EqualContents(iterator, endProperty))
        {
            EditorGUI.PropertyField(fieldRect, iterator, true);
            fieldRect.y += lineHeight + EditorGUIUtility.standardVerticalSpacing;

            iterator.Next(false); // false: enterChildren = false → 同じ階層内の次へ
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int fieldCount = 1;
        SerializedProperty iterator = property.Copy();
        SerializedProperty endProperty = iterator.GetEndProperty();

        iterator.Next(true);
        while (!SerializedProperty.EqualContents(iterator, endProperty))
        {
            fieldCount++;
            iterator.Next(false);
        }

        return fieldCount * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
    }
}

#endif