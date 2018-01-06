using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrFreeze : MonoBehaviour {
    public struct LocalTransform
    {
        public LocalTransform(Transform child)
        {
            this.position = child.localPosition;
            this.rotation = child.localRotation;
        }
        public Vector3 position;
        public Quaternion rotation;
    }

    List<LocalTransform> childTransforms = new List<LocalTransform>();

	// Use this for initialization
	void Start () {
        Freeze();
	}

    public void Freeze()
    {
        childTransforms.Clear();
        foreach (Transform child in transform)
        {
            childTransforms.Add(new LocalTransform(child));
        }
    }

    public void UnFreeze()
    {
        int i = 0;
        if (childTransforms.Count != transform.childCount)
        {
            return;
        }
        foreach (Transform child in transform)
        {
            child.localPosition = childTransforms[i].position;
            child.localRotation = childTransforms[i].rotation;
            ++i;
        }
    }
}
