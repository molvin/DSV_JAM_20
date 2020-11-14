using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StateBoxEditor
{
    /*
    public static void DrawStateEditor(ref Rect position, SerializedProperty property, Editor editor, Action changeCallback, GUIContent context = default)
    {
        if (property.objectReferenceValue == null)
        {
            DrawStateBox(ref position, EditorGUIUtility.singleLineHeight * TitleHeight, Color.red, "Missing Reference");
            position.y += EditorGUIUtility.singleLineHeight * TitleHeight;
            return;
        }
        // Draw background box
        DrawStateBox(ref position, GetHeightOfStateEditor(property, editor, context), StateKraftSettings.Colors.StateBorder);
        // boarder
        position.y += EditorGUIUtility.singleLineHeight * StateEditorTopBorder;
        // Draw TitleBar
        position.height = EditorGUIUtility.singleLineHeight * InspectorTitleHeight;
        property.isExpanded = EditorGUI.InspectorTitlebar(position, property.isExpanded, editor);
        position.y += EditorGUIUtility.singleLineHeight * InspectorTitleHeight;

        // Draw the editor
        if (property.isExpanded)
        {
            using (EditorGUI.ChangeCheckScope check = new EditorGUI.ChangeCheckScope())
            {
                DrawEditor(ref position, editor, context);
                if (check.changed)
                    changeCallback?.Invoke();
            }
        }

        // boarder
        position.y += EditorGUIUtility.singleLineHeight * StateEditorBottomBorder;
    }
    private static void StateEditorLogic(ref Rect position, SerializedProperty state, int arrayIndex)
    {
        Event e = Event.current;
        position.height = EditorGUIUtility.singleLineHeight * InspectorTitleHeight;
        if (e.type != EventType.MouseDown || e.button != 1 || !position.Contains(e.mousePosition))
            return;
        //Create context menu
        GenericMenu context = new GenericMenu();
        context.AddItem(new GUIContent("Remove"), false, () => _actionsToPerform.Enqueue((s) => { RemoveStateFromArray(s, arrayIndex); }));
        context.AddItem(new GUIContent("MoveUp"), false, () => _actionsToPerform.Enqueue((s) => { MoveStateInArray(s, arrayIndex, -1); }));
        context.AddItem(new GUIContent("MoveDown"), false, () => _actionsToPerform.Enqueue((s) => { MoveStateInArray(s, arrayIndex, 1); }));
        context.AddSeparator("");
        context.AddItem(new GUIContent("Unfold"), state.isExpanded, () => { state.isExpanded = true; });
        context.AddItem(new GUIContent("Fold"), !state.isExpanded, () => { state.isExpanded = false; });
        context.ShowAsContext();
        //Consume event
        e.Use();

        //Local methods
        void RemoveStateFromArray(SerializedProperty stateArray, int index)
        {
            stateArray.GetArrayElementAtIndex(index).objectReferenceValue = null;
            stateArray.DeleteArrayElementAtIndex(index);
        }
        void MoveStateInArray(SerializedProperty stateArray, int index, int direction)
        {
            int destination = (index + stateArray.arraySize + direction) % stateArray.arraySize;
            stateArray.MoveArrayElement(index, destination);
        }
    }
    public static float GetHeightOfStateEditor(SerializedProperty property, Editor editor, GUIContent context = default)
    {
        float border = EditorGUIUtility.singleLineHeight * StateEditorBottomBorder + EditorGUIUtility.singleLineHeight * StateEditorTopBorder;
        if (property.objectReferenceValue == null)
            return EditorGUIUtility.singleLineHeight * TitleHeight + border;

        if (!property.isExpanded)
            return EditorGUIUtility.singleLineHeight * InspectorTitleHeight + border;
        return GetHeightOfEditor(editor, context) + EditorGUIUtility.singleLineHeight * InspectorTitleHeight + border;
    }
    private void DrawStateBox(ref Rect position, float height, Color borderColor, string text = "")
    {
        Rect box = new Rect(position) { height = height };
        box.width -= EditorGUIUtility.singleLineHeight * StateEditorHorizontalBorder;
        box.x += EditorGUIUtility.singleLineHeight * StateEditorHorizontalBorder;
        GUI.backgroundColor = StateKraftSettings.Colors.State;
        EditorGUI.LabelField(box, new GUIContent(text), StateBoxStyle);
        GUI.backgroundColor = borderColor;
        EditorGUI.LabelField(box, new GUIContent(""), StateBoxBorderStyle);
        GUI.backgroundColor = Color.white;
    }
    */
}
