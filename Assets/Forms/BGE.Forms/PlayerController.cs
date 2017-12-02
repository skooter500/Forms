using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class PlayerController : MonoBehaviour {

        public enum ControlType { Player, Automatic };
        public ControlType controlType = ControlType.Player;

        public float minHeight = 10;
        public float maxHeight = 1000;
        public float fov;

        Seek seek;
        public float seekDistange = 5000;
        GameObject player;

        // Use this for initialization
        void Start() {
            seek = GetComponent<Seek>();
            player = Camera.main.gameObject;

            StartCoroutine(CheckForNewTarget());
        }

        GameObject PickNewTarget()
        {
            GameObject candidate = Mother.Instance.alive[
                Random.Range(0, Mother.Instance.alive.Count)
                ].gameObject;
            if (gameObject.GetComponent<SchoolGenerator>() == null)
            {
                return Utilities.FindBoidInHierarchy(candidate).gameObject;
            }
            else
            {
                return candidate;
            }
            /*
            // Project onto the XZ plane
            Vector3 target = player.transform.forward;
            target.y = 0;
            target.Normalize();
            // Rotate by a random angle
            target = Quaternion.AngleAxis(
                Random.Range(-fov, fov)
                , Vector3.up
                ) * target;
            target *= Random.Range(seekDistange / 2, seekDistange);

            ;
            target.y = WorldGenerator.Instance.SamplePos(target.x, target.z)
                + Random.Range(minHeight, maxHeight)
                ;

            return target;
            */
        }

        System.Collections.IEnumerator CheckForNewTarget()
        {
            while (true)
            {
                if (controlType == ControlType.Automatic &&  Vector3.Distance(player.transform.position, seek.target) < 1000)
                {
                    seek.targetGameObject = PickNewTarget();
                }
                yield return new WaitForSeconds(1.0f);
            }
        }


        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton3))
            {
                switch (controlType)
                {
                    case ControlType.Player:
                        Debug.Log("Automatic");
                        controlType = ControlType.Automatic;
                        GetComponent<ViveController>().enabled = false;
                        GetComponent<ForceController>().enabled = false;
                        GetComponent<Boid>().enabled = true;
                        seek.targetGameObject = PickNewTarget();
                        seek.Activate(true);
                        GetComponent<NoiseWander>().Activate(true);
                        GetComponent<SceneAvoidance>().Activate(true);
                        break;
                    case ControlType.Automatic:
                        Debug.Log("Player");
                        controlType = ControlType.Player;
                        GetComponent<ViveController>().enabled = true;
                        GetComponent<ForceController>().enabled = true;
                        GetComponent<Boid>().enabled = false;
                        seek.Activate(false);
                        GetComponent<NoiseWander>().Activate(false);
                        GetComponent<SceneAvoidance>().Activate(false);
                        break;
                }
            }
        }
    }
}
