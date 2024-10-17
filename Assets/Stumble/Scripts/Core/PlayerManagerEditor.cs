using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.UI;

//[CustomEditor(typeof(ExperimentalPlayerManager))]
/*public class PlayerManagerEditor : Editor
{
*//*    public override void OnInspectorGUI()
    {
        ExperimentalPlayerManager myScript = (ExperimentalPlayerManager)target;

        // Draw default inspector fields
        DrawDefaultInspector();

*//*        myScript.alwaysVisibleValue1 = EditorGUILayout.FloatField("Always Visible Value 1", myScript.alwaysVisibleValue1);
        myScript.alwaysVisibleValue2 = EditorGUILayout.FloatField("Always Visible Value 2", myScript.alwaysVisibleValue2);*//*

        // Conditional logic to show/hide additional fields
        if (myScript.giveUIAccessOnStart)
        {
            EditorGUILayout.Space();
            myScript.UIEventSystem = (InputSystemUIInputModule)EditorGUILayout.ObjectField("My GameObject", myScript.UIEventSystem, typeof(InputSystemUIInputModule), true);
        }

        EditorGUILayout.HelpBox("Assign a GameObject to see more options.", MessageType.Warning);

        // Apply changes to the serialized object
        if (GUI.changed)
        {
            EditorUtility.SetDirty(myScript);
        }
    }*//*
}*/
