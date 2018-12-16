using UnityEditor;
using UnityEngine;
using System.Collections;

namespace AudioVisualizer
{
    [CustomEditor(typeof(LineWaveform))]
    class LineWaveformEditor : Editor
    {
        string name = "";

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            LineWaveform myTarget = (LineWaveform)target;


            if (GUILayout.Button("Orient Points"))
            {
                Debug.Log("Orienting Points on: " + target.name);
                myTarget.OrientPoints();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Rename Points"))
            {
                Debug.Log("Renaming Points: " + name);
                myTarget.RenamePoints(name);
            }
            name = EditorGUILayout.TextField("Name:",name);
            GUILayout.EndHorizontal();
        }
    }
}
