using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CollisionFlagsAttribute))]
public class CollisionFlagsAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        CollisionFlags flags = (CollisionFlags)property.intValue;
        flags = (CollisionFlags)EditorGUI.EnumFlagsField(position, label, flags);
        property.intValue = (int)flags;
    }
}
