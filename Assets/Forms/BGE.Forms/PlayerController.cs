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
        Boid boid;
        SceneAvoidance sceneAvoidance;
        public float seekDistange = 5000;
        GameObject player;
        ViveController viveController;

        // Use this for initialization
        void Start() {
            player = Camera.main.gameObject;
            viveController = GetComponent<ViveController>();
            StartCoroutine(CheckForNewTarget());
        }

        void AssignBehaviours()
        {
            if (viveController.boid != null)
            {
                boid = viveController.boid;
                seek = boid.GetComponent<Seek>();
                sceneAvoidance = boid.GetComponent<SceneAvoidance>();
            }
            else
            {
                boid = GetComponent<Boid>();
                seek = GetComponent<Seek>();
                sceneAvoidance = GetComponent<SceneAvoidance>();
            }
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
                        AssignBehaviours();
                        viveController.enabled = false;
                        GetComponent<ForceController>().enabled = false;
                        boid.enabled = true;
                        seek.targetGameObject = PickNewTarget();
                        seek.Activate(true);
                        sceneAvoidance.Activate(true);
                        if (boid.GetComponent<Harmonic>() != null)
                        {
                            boid.GetComponent<Harmonic>().auto = true;
                        }

                        if (boid.GetComponent<PlayerSteering>() != null)
                        {
                            boid.GetComponent<PlayerSteering>().controlSpeed = false;
                        }
                        break;
                    case ControlType.Automatic:
                        Debug.Log("Player");
                        controlType = ControlType.Player;
                        AssignBehaviours();
                        viveController.enabled = true;
                        GetComponent<ForceController>().enabled = true;

                        if (viveController.boid == null)
                        {
                            boid.enabled = false;
                            sceneAvoidance.Activate(false);
                        }
                        seek.Activate(false);

                        if (boid.GetComponent<PlayerSteering>() != null)
                        {
                            boid.GetComponent<PlayerSteering>().Activate(true);
                            boid.GetComponent<PlayerSteering>().controlSpeed = true;
                        }

                        if (boid.GetComponent<Harmonic>() != null)
                        {
                            boid.GetComponent<Harmonic>().auto = false;
                        }
                        break;
                }
            }
        }
    }
}
