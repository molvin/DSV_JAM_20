using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorExtension
{
    public static float GetHeightOfEditor(this Editor editorToDraw, GUIContent context = default)
    {
        float height = 0f;
        if (editorToDraw == null)
            return height;
        SerializedProperty it = editorToDraw.serializedObject.GetIterator();
        it.NextVisible(true);
        while (it.NextVisible(false))
            height += EditorGUI.GetPropertyHeight(it, context);
        return height;
    }
    public static void DrawEditor(this Editor editorToDraw, ref Rect position, GUIContent context = default)
    {
        SerializedProperty it = editorToDraw.serializedObject.GetIterator();
        it.NextVisible(true);
        editorToDraw.serializedObject.Update();
        int indent = EditorGUI.indentLevel;
        while (it.NextVisible(false))
        {
            EditorGUI.indentLevel = indent + it.depth;
            position.height = EditorGUI.GetPropertyHeight(it, context);
            EditorGUI.PropertyField(position, it, context, true);
            position.y += EditorGUI.GetPropertyHeight(it);
        }
        if (GUI.changed)
            editorToDraw.serializedObject.ApplyModifiedProperties();
    }
}
