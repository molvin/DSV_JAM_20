using System.Linq;

namespace  StateKraft.Editor
{
using System;
using UnityEditor;
using UnityEngine;
using StateKraft;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Collections;

[CustomPropertyDrawer(typeof(StateMachine))]
public class StateMachineEditor : PropertyDrawer
{
    //Appearance
    private const float TitleHeight = 1.6f;
    private const float BetweenStateEditorLineHeight = 0.5f;
    private const float InspectorTitleHeight = 1.5f;
    private const float StateEditorHorizontalBorder = -0.1f;
    private const float StateEditorTopBorder = -0.05f;
    private const float StateEditorBottomBorder = 0.4f;
    private const float DropAreaHeight = 2.5f;
    private const float CollapsedDropAreaHeight = 0.50f;
    private static readonly GUIStyle TitleBoxStyle = new GUIStyle { fontSize = 14, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, border = new RectOffset(5, 5, 5, 5) };
    private static readonly GUIStyle StateNoneBoxStyle = new GUIStyle { fontSize = 13, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, normal = new GUIStyleState { textColor = Color.red }, border = new RectOffset(5, 5, 5, 5) };
    private static readonly GUIStyle StateBoxStyle = new GUIStyle { fontSize = 13, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, border = new RectOffset(5, 5, 5, 5) };
    private static readonly GUIStyle StateBoxBorderStyle = new GUIStyle { fontSize = 13, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, border = new RectOffset(5, 5, 5, 5) };
    private static readonly GUIStyle DropAreaStyle = new GUIStyle { fontSize = 14, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, border = new RectOffset(15, 15, 15, 15) };
    private readonly Texture2D[] _texts = Resources.LoadAll<Texture2D>("StateKraft");
    //Data
    private readonly Queue<Action<SerializedProperty>> _actionsToPerform = new Queue<Action<SerializedProperty>>();
    [SerializeField] private Editor[] _cachedEditors;

    //Update loop
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Gui styles
        StateNoneBoxStyle.normal.background = _texts[2];
        StateBoxStyle.normal.background = _texts[2];
        StateBoxBorderStyle.normal.background = _texts[3];
        TitleBoxStyle.normal.background = _texts[2];

        position.height = EditorGUIUtility.singleLineHeight * TitleHeight;
        //Foldout
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, "", true);
        //Title
        GUI.backgroundColor = StateKraftSettings.Colors.Title;
        TitleBoxStyle.normal.textColor = StateKraftSettings.Colors.TitleFont;
        EditorGUI.LabelField(position, new GUIContent(property.displayName), TitleBoxStyle);
        GUI.backgroundColor = StateKraftSettings.Colors.TitleBorder;
        EditorGUI.LabelField(position, new GUIContent(""), StateBoxBorderStyle);
        GUI.backgroundColor = Color.white;
        position.y += EditorGUIUtility.singleLineHeight * TitleHeight;

        SerializedProperty states = property.FindPropertyRelative("_states");
        
        //Update cached editors
        if(_cachedEditors == null || _cachedEditors.Length != states.arraySize)
            _cachedEditors = new Editor[states.arraySize]; 

        //Run queue
        while (_actionsToPerform.Count > 0)
            _actionsToPerform.Dequeue()(states);

        if (!property.isExpanded)
            return;
        
        //Background Box 
        Rect box = new Rect(position);
        box.height = GetPropertyHeight(property, label) - EditorGUIUtility.singleLineHeight * TitleHeight;
        GUI.backgroundColor = StateKraftSettings.Colors.Background;
        EditorGUI.LabelField(box, new GUIContent(""), TitleBoxStyle);
        GUI.backgroundColor = Color.white;
        
        DrawStates(ref position, states);
        DrawDropArea(ref position, states, true, states.arraySize);
    }
    private void DrawStates(ref Rect position, SerializedProperty states)
    {
        //Draw all states
        EditorGUI.indentLevel++;
        object obj = fieldInfo.GetValue(states.serializedObject.targetObject);
        if (!(obj is StateMachine stateMachine)) return;
        for (int i = 0; i < states.arraySize; i++)
        {
            position.y += EditorGUIUtility.singleLineHeight * BetweenStateEditorLineHeight;
            position.height += EditorGUIUtility.singleLineHeight * BetweenStateEditorLineHeight;
            SerializedProperty state = states.GetArrayElementAtIndex(i);

            Editor.CreateCachedEditor(state.objectReferenceValue, typeof(StateEditor), ref _cachedEditors[i]);
            StateEditorLogic(ref position, state, i);
            Action callback = null;
            bool isCurrentState = false;
            if (EditorApplication.isPlaying)
            {
                callback = () => stateMachine.ReinitializeState(state.objectReferenceValue as State);
                isCurrentState = stateMachine.CurrentState != null && stateMachine.CurrentState.GetType() == state.objectReferenceValue.GetType();
            }

            DrawStateEditor(ref position, state, _cachedEditors[i], callback, isCurrentState);
            if(i < states.arraySize - 1)
                DrawDropArea(ref position, states, false, i);
        }
    }
    private void DrawStateEditor(ref Rect position, SerializedProperty stateProperty, Editor editor, Action changeCallback, bool isCurrentState, GUIContent context = default)
    {
        if (stateProperty.objectReferenceValue == null)
        {
            DrawStateBox(ref position, EditorGUIUtility.singleLineHeight * TitleHeight, Color.red, "Missing Reference");
            position.y += EditorGUIUtility.singleLineHeight * TitleHeight;
            return;
        }
        // Draw background box
        DrawStateBox(ref position, GetHeightOfStateEditor(stateProperty, editor, context), isCurrentState ? StateKraftSettings.Colors.CurrentStateColor : StateKraftSettings.Colors.StateBorder);
        // boarder
        position.y += EditorGUIUtility.singleLineHeight * StateEditorTopBorder;
        // Draw TitleBar
        position.height = EditorGUIUtility.singleLineHeight * InspectorTitleHeight;
        stateProperty.isExpanded = EditorGUI.InspectorTitlebar(position, stateProperty.isExpanded, editor);
        position.y += EditorGUIUtility.singleLineHeight * InspectorTitleHeight;
        
        // Draw the editor
        if (stateProperty.isExpanded)
        {
            using (EditorGUI.ChangeCheckScope check = new EditorGUI.ChangeCheckScope())
            {
                editor.DrawEditor(ref position, context);
                if (check.changed)
                    changeCallback?.Invoke();
            }
        }

        // boarder
        position.y += EditorGUIUtility.singleLineHeight * StateEditorBottomBorder; 
    }
    private void StateEditorLogic(ref Rect position, SerializedProperty state, int arrayIndex)
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
    private void DrawDropArea(ref Rect position, SerializedProperty states, bool expanded, int index)
    {
        //Drop To add state area
        position.y += EditorGUIUtility.singleLineHeight * BetweenStateEditorLineHeight;
        
        Rect box = new Rect(position);

        if (!expanded)
        {
            box.height = EditorGUIUtility.singleLineHeight * CollapsedDropAreaHeight;
            box.y += box.height * 0.5f;
            EditorGUI.DrawRect(box, StateKraftSettings.DropAreaColor);
            //if (box.Contains(Event.current.mousePosition))
            //   expanded = true;
            //TODO: figure out a way to expand the drop area on mouse over, here and in get height
        }
        
        if (expanded)
        {
            box.height = EditorGUIUtility.singleLineHeight * DropAreaHeight;
            GUI.backgroundColor = StateKraftSettings.Colors.DropArea;
            DropAreaStyle.normal.textColor = StateKraftSettings.Colors.DropAreaFont;
            //Drop area
            DropAreaStyle.normal.background = _texts[1];
            EditorGUI.LabelField(box, new GUIContent("Drop State Here"), DropAreaStyle);
            //Dotted area
            DropAreaStyle.normal.background = _texts[0];
            GUI.backgroundColor = Color.white;
            box.x = (position.width / 2f) - 100 + EditorGUI.indentLevel * 15f;
            box.width = 200;
            box.y += box.height * 0.7f * 0.25f;
            box.height *= 0.7f;
            EditorGUI.LabelField(box, new GUIContent(""), DropAreaStyle);
        }

        position.y += EditorGUIUtility.singleLineHeight * BetweenStateEditorLineHeight;
        
        //Logic
        if (!box.Contains(Event.current.mousePosition)) 
            return;

        switch (Event.current.type)
        {
            case EventType.DragUpdated:
                DragAndDrop.visualMode = DragAndDrop.objectReferences.Any(o => o is State || o is MonoScript s && !s.GetClass().IsAbstract && s.GetClass().IsSubclassOf(typeof(State))) ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
                Event.current.Use();
                break;
            case EventType.DragPerform:
            {
                foreach (Object obj in DragAndDrop.objectReferences)
                {
                    if (obj is State)
                    {
                        AddNewState(obj, states, index);
                    }
                    else if (obj is MonoScript script)
                    {
                        Type classType = script.GetClass();
                        if (classType.IsSubclassOf(typeof(State)) && !classType.IsAbstract)
                        {
                            State stateInstance = ScriptableObject.CreateInstance(classType) as State;
                            string path = $"{AssetDatabase.GetAssetPath(script).Split('.')[0]}.asset";
                            path = AssetDatabase.GenerateUniqueAssetPath(path);
                            AssetDatabase.CreateAsset(stateInstance, path);
                            AssetDatabase.SaveAssets();
                            AddNewState(stateInstance, states, index);
                        }
                    }
                }
                Event.current.Use();
                break;
            }
        }
    }
    //Editor shit
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
    //Functionality
    private void AddNewState(Object obj, SerializedProperty states, int index)
    {
        for (int i = states.arraySize - 1; i >= 0; i--)
        {
            Object o = states.GetArrayElementAtIndex(i).objectReferenceValue;
            if (o == null || o.GetType() == obj.GetType())
            {
                states.GetArrayElementAtIndex(i).objectReferenceValue = null;
                states.DeleteArrayElementAtIndex(i);
            }
        }
        
        index = Mathf.Clamp(index, 0, Mathf.Max(states.arraySize, 0));
        states.InsertArrayElementAtIndex(index);
        states.GetArrayElementAtIndex(index).objectReferenceValue = obj;
    }
    //Heights
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0;
        SerializedProperty states = property.FindPropertyRelative("_states");
        height += EditorGUIUtility.singleLineHeight * TitleHeight;

        //States height
        if (!property.isExpanded)
            return height;

        //States height
        if(_cachedEditors == null || _cachedEditors.Length != states.arraySize)
            _cachedEditors = new Editor[states.arraySize];
        for (int i = 0; i < states.arraySize; i++)
        {
            if(_cachedEditors[i] == null)
                Editor.CreateCachedEditor(states.GetArrayElementAtIndex(i).objectReferenceValue, typeof(StateEditor), ref _cachedEditors[i]);
            height += GetHeightOfStateEditor(states.GetArrayElementAtIndex(i), _cachedEditors[i]) + EditorGUIUtility.singleLineHeight * BetweenStateEditorLineHeight;
        }

        //Drop area
        height += GetDropBoxHeight(true);
        return height;
    }
    private float GetHeightOfStateEditor(SerializedProperty property, Editor editor, GUIContent context = default)
    {
        float border = EditorGUIUtility.singleLineHeight * StateEditorBottomBorder  + EditorGUIUtility.singleLineHeight * StateEditorTopBorder;
        if (property.objectReferenceValue == null)
            return EditorGUIUtility.singleLineHeight * TitleHeight + border;

        float height = EditorGUIUtility.singleLineHeight * InspectorTitleHeight + border;
        height += GetDropBoxHeight(false);

        if (!property.isExpanded || editor == null)
            return height;

        SerializedProperty it = editor.serializedObject.GetIterator();
        it.NextVisible(true);
        while (it.NextVisible(false))
            height += EditorGUI.GetPropertyHeight(it, context, true);

        return height;
    }
    private float GetDropBoxHeight(bool expanded)
    {
        if (!expanded)
            return EditorGUIUtility.singleLineHeight * CollapsedDropAreaHeight;

        return EditorGUIUtility.singleLineHeight * BetweenStateEditorLineHeight
               + EditorGUIUtility.singleLineHeight * DropAreaHeight
                + EditorGUIUtility.singleLineHeight * BetweenStateEditorLineHeight;
    }
}
}

