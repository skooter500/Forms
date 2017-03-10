using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/*
[CustomEditor(typeof(SpineAnimator))]
public class SpineAnimatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SpineAnimator sa = (SpineAnimator)target;

        sa.autoAssignBones = EditorGUILayout.Toggle("Auto assign bones", sa.autoAssignBones);
        sa.bondDamping = EditorGUILayout.FloatField("Bond damping", sa.bondDamping);
        sa.angularBondDamping = EditorGUILayout.FloatField("Angular Bond damping", sa.angularBondDamping);

        if (GUILayout.Button("Add custom joint parameter"))
        {
            sa.jointParams.Add(new JointParam(sa.bondDamping, sa.angularBondDamping));
        }

        for(int i = 0; i < sa.jointParams.Count; i ++)
        {                       
            JointParam jp = sa.jointParams[i];
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Joint: " + i);
            float bd = EditorGUILayout.FloatField("Bond damping", jp.bondDamping);
            float abd = EditorGUILayout.FloatField("Angular bond damping", jp.angularBondDamping);
            sa.jointParams[i] = new JointParam(bd, abd);
            if (GUILayout.Button("Remove"))
            {
                sa.jointParams.RemoveAt(i);
            }
        }        
    }
}
*/