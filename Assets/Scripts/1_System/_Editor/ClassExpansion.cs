using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ClassExpansion
{
    // color
    #if UNITY_EDITOR
    public static Color ToBackgroundIgnoreColor(this Color color)
    {
        Color backgroundColor = EditorGUIUtility.isProSkin
            ? new Color32(56, 56, 56, 255)
            : new Color32(194, 194, 194, 255);
        return new Color(
            color.r/backgroundColor.r, 
            color.g/backgroundColor.g, 
            color.b/backgroundColor.b, 
            color.a/backgroundColor.a);
    }
    #endif
}