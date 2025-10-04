using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif


/// <summary>
/// 他のフィールドの値を条件として有効状態を切り替える
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property, AllowMultiple = false)]
public class EnabledIfAttribute : PropertyAttribute 
{
    /// <summary>
    /// 非表示状態の表現方法
    /// </summary>
    public enum HideMode
    {
        Invisible, // 見えなくする
        Disabled, // 非アクティブにする
    }

    /// <summary>
    /// 条件として指定するフィールドの型
    /// </summary>
    public enum SwitcherType
    {
        Bool,
        Enum,
    }

    public SwitcherType switcherType;
    public HideMode hideMode;
    public string switcherFieldName;
    public int enableIfValueIs;
 
    public EnabledIfAttribute(string switcherFieldName, bool enableIfValueIs, HideMode hideMode = HideMode.Disabled)
        : this(SwitcherType.Bool, switcherFieldName, enableIfValueIs ? 1 : 0, hideMode)
    {
    }

    public EnabledIfAttribute(string switcherFieldName, int enableIfValueIs, HideMode hideMode = HideMode.Disabled)
        : this(SwitcherType.Enum, switcherFieldName, enableIfValueIs, hideMode)
    {
    }

    private EnabledIfAttribute(SwitcherType switcherType, string switcherFieldName, int enableIfValueIs, HideMode hideMode)
    {
        this.switcherType = switcherType;
        this.hideMode = hideMode;
        this.switcherFieldName = switcherFieldName;
        this.enableIfValueIs = enableIfValueIs;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(EnabledIfAttribute))]
public class EnabledIfAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = attribute as EnabledIfAttribute;       
        var isEnabled = GetIsEnabled(attr, property);

        // 非表示処理
        if (attr.hideMode == EnabledIfAttribute.HideMode.Disabled) {
            GUI.enabled &= isEnabled;
        }

        // プロパティを描画
        if (GetIsVisible(attr, property)) {
            EditorGUI.PropertyField(position, property, label, true);
        }

        // GUI.enabledを戻す
        if (attr.hideMode == EnabledIfAttribute.HideMode.Disabled) {
            GUI.enabled = true;
        }
    }

    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        var attr = attribute as EnabledIfAttribute;
        return GetIsVisible(attr, property) ? EditorGUI.GetPropertyHeight(property) : -2; // 表示されていない場合スペースを詰めるため-2を返す
    }

    /// <summary>
    /// 表示されるか
    /// 非アクティブでも表示されていればtrueを返す
    /// </summary>
    private bool GetIsVisible(EnabledIfAttribute attribute, SerializedProperty property)
    {
        if (GetIsEnabled(attribute, property)) {
            return true;
        }
        if (attribute.hideMode != EnabledIfAttribute.HideMode.Invisible) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// フィールドが有効か
    /// </summary>
    private bool GetIsEnabled(EnabledIfAttribute attribute, SerializedProperty property)
    {
        return attribute.enableIfValueIs == GetSwitcherPropertyValue(attribute, property);
    }

    private int GetSwitcherPropertyValue(EnabledIfAttribute attribute, SerializedProperty property)
    {
        var propertyNameIndex = property.propertyPath.LastIndexOf(property.name, StringComparison.Ordinal);
        var switcherPropertyName = property.propertyPath.Substring(0, propertyNameIndex) + attribute.switcherFieldName;
        var switcherProperty = property.serializedObject.FindProperty(switcherPropertyName);
        switch (switcherProperty.propertyType) {
        case SerializedPropertyType.Boolean:
            return switcherProperty.boolValue ? 1 : 0;
        case SerializedPropertyType.Enum:
            return switcherProperty.intValue;
        default:
            throw new System.Exception("unsupported type.");
        }
    }
}
#endif