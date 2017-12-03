using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BGE.Forms
{
    public class Toad : MonoBehaviour
    {
        Camera camera;

        float startFOV;

        // Use this for initialization
        void Start()
        {
            camera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {

        }

        System.Collections.IEnumerator UnToad()
        {
            float t = 0;
            while (camera.fieldOfView -startFOV > 0.1f)
            {
                camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, startFOV, Time.deltaTime / 4.0f);
                yield return null;
            }
            camera.fieldOfView = startFOV;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                startFOV = camera.fieldOfView;
                camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 179, Time.deltaTime);
            }
        }

        void OnTriggerStay(Collider other)
        {
            if (other.tag == "Player")
            {
                camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 179, Time.deltaTime);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(UnToad());
            }
        }
    }
}
