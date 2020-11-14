using System;
using System.IO;
using UnityEngine;
using UnityEditor;

public class StateKraftSettings : EditorWindow
{
    public static string StatePath
    {
        get => EditorPrefs.GetString(StatePathPrefsKey, DefaultStatePath);
        set => EditorPrefs.SetString(StatePathPrefsKey, value);
    }
    private const string DefaultStatePath = "Assets";
    private static bool UseCustomColors
    {
        get => EditorPrefs.GetBool(UseCustomColorsEditorPrefsKey);
        set => EditorPrefs.SetBool(UseCustomColorsEditorPrefsKey, value);
    }
    private static StateKraftColors _colors;
    private static readonly StateKraftColors DefaultColors = new StateKraftColors
    {
        Title = new Color(0.25f, 0.25f, 0.25f, 1f),
        TitleBorder =  new Color(1.00f, 1.00f, 1.00f, 1f),
        TitleFont = new Color(1.00f, 1.00f, 1.00f, 1f),
        State= new Color(1.00f, 1.00f, 1.00f, 1f),
        StateBorder = new Color(0.40f, 0.70f, 1.00f, 1f),
        Background = new Color(0.90f, 0.90f, 0.90f, 1f),
        DropArea = new Color(0.90f, 0.80f, 0.3f, 1f),
        DropAreaFont = new Color(1.00f, 1.00f, 1.00f, 1f),
        CurrentStateColor = Color.green
    };
    private static readonly StateKraftColors DefaultDarkThemeColors = new StateKraftColors
    {
        Title = new Color(0.34f, 0.34f, 0.34f, 1f),
        TitleBorder =  new Color(1.00f, 1.00f, 1.00f, 1f),
        TitleFont = new Color(1.00f, 1.00f, 1.00f, 1f),
        State = new Color(0.18f, 0.18f, 0.18f, 1f),
        StateBorder = new Color(0.40f, 0.70f, 1.00f, 1f),
        Background = new Color(0.16f, 0.16f, 0.16f, 1f),
        DropArea = new Color(0.90f, 0.80f, 0.3f, 1f),
        DropAreaFont = new Color(1.00f, 1.00f, 1.00f, 1f),
        CurrentStateColor =  Color.green
    };
    private const string StatePathPrefsKey = "StateKraftSettingsPath";
    private const string UseCustomColorsEditorPrefsKey = "StateKraftSettingsUseCustomColors";
    private const string ColorEditorPrefsKey = "StateKraftSettingsColors";
    //Properties
    public static StateKraftColors Colors
    {
        get
        {
            if (!UseCustomColors) return EditorGUIUtility.isProSkin ? DefaultDarkThemeColors : DefaultColors;
            if (_colors != null) return _colors;
            _colors = EditorPrefs.HasKey(ColorEditorPrefsKey) ? JsonUtility.FromJson<StateKraftColors>(EditorPrefs.GetString(ColorEditorPrefsKey)) : EditorGUIUtility.isProSkin ? DefaultDarkThemeColors : DefaultColors;
            return _colors;
        }
    }
    public static Color TitleBoxColor { get { return Colors.Title; } }
    public static Color TitleBoxBorderColor { get { return Colors.TitleBorder; } }
    public static Color TitleFontColor { get { return Colors.TitleFont; } }
    public static Color StateBoxColor { get { return Colors.State; } }
    public static Color StateBoxBorderColor { get { return Colors.StateBorder; } }
    public static Color BackgroundColor { get { return Colors.Background; } }
    public static Color DropAreaColor { get { return Colors.DropArea; } }
    public static Color DropAreaFontColor { get { return Colors.DropAreaFont; } }
    
    [Serializable]
    public class StateKraftColors
    {
        public Color Title;
        public Color TitleBorder;
        public Color TitleFont;
        public Color State;
        public Color StateBorder;
        public Color Background;
        public Color DropArea;
        public Color DropAreaFont;
        public Color CurrentStateColor;
    }
    
    [MenuItem("Window/StateKraft/Settings")]
    private static void Init()
    {
        StateKraftSettings window = (StateKraftSettings) GetWindow(typeof(StateKraftSettings));
        window.Show();
    }
    private void OnEnable()
    {
        StatePath = EditorPrefs.GetString(StatePathPrefsKey, DefaultStatePath);
    }
    private void OnGUI()
    {
        GUILayout.Label("StateKraft Settings", EditorStyles.boldLabel);

        using (EditorGUI.ChangeCheckScope check = new EditorGUI.ChangeCheckScope())
        {
            string temp = StatePath;
            StatePath = EditorGUILayout.TextField("Default States Path", StatePath);
            if (check.changed)
            {
                if(!Directory.Exists(StatePath))
                    StatePath = temp;
            }
        }
        using (EditorGUI.ChangeCheckScope check = new EditorGUI.ChangeCheckScope())
        {
            UseCustomColors = EditorGUILayout.BeginToggleGroup("Use Custom Colors", UseCustomColors);
            if(check.changed)
                EditorPrefs.SetBool(UseCustomColorsEditorPrefsKey, UseCustomColors);
        }
        
        using (EditorGUI.ChangeCheckScope check = new EditorGUI.ChangeCheckScope())
        {
            GUILayout.Label("Colors", EditorStyles.boldLabel);
            Colors.Title = EditorGUILayout.ColorField("Title Box", Colors.Title);
            Colors.TitleBorder = EditorGUILayout.ColorField("Title Box Border", Colors.TitleBorder);
            Colors.TitleFont = EditorGUILayout.ColorField("Title Font", Colors.TitleFont);
            Colors.State = EditorGUILayout.ColorField("State Box", Colors.State);
            Colors.StateBorder = EditorGUILayout.ColorField("State Box Border", Colors.StateBorder);
            Colors.Background = EditorGUILayout.ColorField("Background", Colors.Background);
            Colors.DropArea = EditorGUILayout.ColorField("Drop Area", Colors.DropArea);
            Colors.DropAreaFont = EditorGUILayout.ColorField("Drop Area Font", Colors.DropAreaFont);
            Colors.CurrentStateColor = EditorGUILayout.ColorField("Current State", Colors.CurrentStateColor);
            
            if(check.changed)
                EditorPrefs.SetString(ColorEditorPrefsKey, JsonUtility.ToJson(Colors));
        }

        if (GUILayout.Button("Reset Colors"))
        {
            _colors = null;
            EditorPrefs.DeleteKey(ColorEditorPrefsKey);
            EditorPrefs.SetString(ColorEditorPrefsKey, JsonUtility.ToJson(Colors));
        }
        
        EditorGUILayout.EndToggleGroup();

    }
}
