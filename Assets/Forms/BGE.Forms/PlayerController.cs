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

        CameraTransitionController ctc;

        bool waiting = false;

        float distance = 500;

        public static PlayerController Instance;

        Coroutine targetingCoroutine;

        public void Awake()
        {
            PlayerController.Instance = this;
        }

        // Use this for initialization
        void Start() {
            player = GameObject.FindGameObjectWithTag("Player");
            viveController = player.GetComponent<ViveController>();
            ctc = GameObject.FindObjectOfType<CameraTransitionController>();
            
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

            distance = candidate.GetComponent<SpawnParameters>().viewingDistance;
            return Mother.Instance.GetCreature(candidate);
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
                if (controlType == ControlType.Automatic &&  Vector3.Distance(player.transform.position, seek.target) < distance)
                {
                    Utilities.SetActive(seek, false);
                    boid.damping = 0.5f;
                    waiting = true;
                    Debug.Log("Waiting...");
                    //boid.enabled = false;
                    //ctc.HideEffect();
                    yield return new WaitForSeconds(Random.Range(20, 30));
                    boid.enabled = true;
                    Debug.Log("Finding new target...");
                    waiting = false;
                    boid.damping = 0.01f;
                    seek.targetGameObject = PickNewTarget();
                    transform.position = player.transform.position;
                    transform.rotation = player.transform.rotation;
                    boid.UpdateLocalFromTransform();
                    Utilities.SetActive(seek, true);
                    //ctc.ShowLeftEffect();
                }
                yield return new WaitForSeconds(1);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton3))
            {
                switch (controlType)
                {
                    case ControlType.Player:
                        Debug.Log("Automatic");
                        controlType = ControlType.Automatic;
                        transform.position = player.transform.position;
                        transform.rotation = player.transform.rotation;
                        AssignBehaviours();
                        boid.UpdateLocalFromTransform();
                        player.GetComponent<Rigidbody>().isKinematic = true;
                        viveController.enabled = false;
                        player.GetComponent<ForceController>().enabled = false;
                        boid.enabled = true;
                        boid.desiredPosition = transform.position;
                        seek.targetGameObject = PickNewTarget();
                        //seek.target = Vector3.zero;
                        seek.SetActive(true);
                        sceneAvoidance.SetActive(true);
                        if (boid.GetComponent<Harmonic>() != null)
                        {
                            boid.GetComponent<Harmonic>().auto = true;
                        }

                        if (boid.GetComponent<PlayerSteering>() != null)
                        {
                            boid.GetComponent<PlayerSteering>().controlSpeed = false;
                        }
                        targetingCoroutine = StartCoroutine(CheckForNewTarget());
                        break;
                    case ControlType.Automatic:
                        Debug.Log("Player");
                        StopCoroutine(targetingCoroutine);
                        controlType = ControlType.Player;
                        AssignBehaviours();
                        player.GetComponent<Rigidbody>().isKinematic = false;
                        viveController.enabled = true;
                        player.GetComponent<ForceController>().enabled = true;
                        player.GetComponent<ForceController>().desiredRotation =
                            Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
                        if (viveController.boid == null)
                        {
                            boid.enabled = false;
                            sceneAvoidance.SetActive(false);
                        }
                        seek.SetActive(false);

                        if (boid.GetComponent<PlayerSteering>() != null)
                        {
                            boid.GetComponent<PlayerSteering>().SetActive(true);
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

        void FixedUpdate()
        {
                        
            if (controlType == ControlType.Automatic)
            {
                player.transform.position = this.transform.position;
                /*
                player.transform.position = Vector3.Lerp(
                    player.transform.position
                    , this.transform.position
                    , Time.deltaTime
                    );
                    */
                if (waiting)
                {
                    player.transform.rotation = Quaternion.Slerp(player.transform.rotation
                        , Quaternion.LookRotation(seek.targetGameObject.transform.position - transform.position)
                        , Time.deltaTime
                        );
                }
                else
                {
                    //player.transform.rotation = transform.rotation;
                    player.transform.rotation = Quaternion.Slerp(player.transform.rotation
                        , transform.rotation
                        , Time.deltaTime
                        );                    
                }
            }
        }
    }
}
