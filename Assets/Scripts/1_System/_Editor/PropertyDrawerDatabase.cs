using UnityEditor;
using System.Collections.Generic;
#if UNITY_EDITOR
public static class PropertyDrawerDatabase
{
    private  static Dictionary<System.Type, PropertyDrawer> _drawers = new Dictionary<System.Type, PropertyDrawer>();

    static PropertyDrawerDatabase()
    {
        _drawers = new Dictionary<System.Type, PropertyDrawer>();
        
        // クラスと対応するPropertyDrawerを登録しておく
        //_drawers.Add(typeof(SomeClass), new SomeClassDrawer());

        //_drawers.Add(typeof(RoomSystem), new SomeClassDrawer());
    }

    public static PropertyDrawer GetDrawer(System.Type fieldType)
    {
        PropertyDrawer drawer;
        return _drawers.TryGetValue(fieldType, out drawer) ? drawer : null;
    }
}
#endif