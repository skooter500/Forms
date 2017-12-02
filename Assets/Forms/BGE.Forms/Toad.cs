using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BGE.Forms
{
    public class Toad : MonoBehaviour
    {
        Camera camera;

        // Use this for initialization
        void Start()
        {
            camera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerStay()
        {
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 180, Time.deltaTime);
        }
    }
}
