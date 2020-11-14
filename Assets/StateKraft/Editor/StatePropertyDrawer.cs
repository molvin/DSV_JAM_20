using System;
using UnityEditor;
using UnityEngine;
using StateKraft;
using Object = UnityEngine.Object;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(State))]
public class StatePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
       // Debug.Log("Draw State Property");
    }
    private float GetHeightOfEditor(Editor editorToDraw, GUIContent context = default)
    {
        return 100;
    }
}
