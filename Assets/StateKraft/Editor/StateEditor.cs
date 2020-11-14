using System.Collections.Generic;
using System.Linq;
using StateKraft;
using UnityEditor;

[CustomEditor(typeof(State), true)]
public class StateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        using (EditorGUI.ChangeCheckScope check = new EditorGUI.ChangeCheckScope())
        {
            DrawDefaultInspector();
            if (check.changed && EditorApplication.isPlaying)
                ReInit();
        }
    }
    private void ReInit()
    {
        //Find all created states in play
        State[] instantiatedStates = FindObjectsOfType<State>();
        //Find those that are instances of this object
        List<State> states = instantiatedStates.Where(state => state.GetType() == target.GetType()).ToList();
        //Reinitialize the states in their state machines with the new variables
        foreach (State state in states)
            state.StateMachine.ReinitializeState(target as State);
    }
}