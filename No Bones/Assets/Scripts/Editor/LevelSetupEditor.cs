using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LevelSetup))]
public class LevelSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector ();

        LevelSetup myScript = (LevelSetup) target;
        if (GUILayout.Button("Setup Level"))
        {
            myScript.SetupLevel (myScript.levelNumber);
        }
    }
}
