using System;
using System.Collections;
using BGE.Forms;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Route))]
public class RouteEditor: Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Route path = (Route)target;
        if (GUILayout.Button("Add waypoint"))
        {
            GameObject waypoint = new GameObject();
            if (path.transform.childCount == 0)
            {
                waypoint.transform.position = path.transform.position;
            }
            else
            {
                Transform last = path.transform.GetChild(path.transform.childCount - 1);
                waypoint.transform.position = last.transform.position;
            }
            waypoint.name = "Waypoint";
            waypoint.transform.parent = path.transform;
        }
    }
}