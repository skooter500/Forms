using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class PlayerController : MonoBehaviour {

        class PlayerState : State
        {
            public override void Enter()
            {

            }
            public virtual void Exit() { }
        }

        class JourneyingState : State
        {
            Cruise c;
            public override void Enter()
            {
                Vector3 pos = owner.transform.position;
                c = owner.GetComponent<Cruise>();
                c.preferredHeight = pos.y - BGE.Forms.WorldGenerator.Instance.SamplePos(pos.x, pos.z);
                c.enabled = true;
            }

            public override void Think()
            {
                if (Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.J))
                {
                    owner.GetComponent<StateMachine>().ChangeState(new FollowState());
                }
            }

            public override void Exit()
            {
                c.enabled = false;
            }
        }

        class FollowState : State
        {
            public override void Enter()
            {
                boid.transform.position = player.transform.position;
                boid.transform.rotation = player.transform.rotation;
                AssignBehaviours();
                boid.UpdateLocalFromTransform();
                player.GetComponent<Rigidbody>().isKinematic = true;
                viveController.enabled = false;
                player.GetComponent<ForceController>().enabled = false;
                boid.enabled = true;
                boid.desiredPosition = transform.position;
                seek.targetGameObject = PickNewTarget();
                //boid.maxSpeed = 300;
                //seek.target = Vector3.zero;
                Utilities.SetActive(seek, true);
                Utilities.SetActive(op, false);
                sceneAvoidance.SetActive(true);
                if (boid.GetComponent<Harmonic>() != null)
                {
                    boid.GetComponent<Harmonic>().auto = true;
                }

                if (boid.GetComponent<PlayerSteering>() != null)
                {
                    boid.GetComponent<PlayerSteering>().controlSpeed = false;
                }
                if (controlType == ControlType.Automatic && Vector3.Distance(player.transform.position, seek.target) < distance)
                {
                    StartCo
                }
            }

        StateMachine sm;

        public float minHeight = 10;
        public float maxHeight = 1000;
        public float fov;

        Seek seek;
        Boid boid;
        SceneAvoidance sceneAvoidance;
        OffsetPursue op;
        public float seekDistange = 5000;
        GameObject player;
        GameObject creature;
        ViveController viveController;
        Cruise cruise;

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
            cruise = GetComponent<Cruise>();
            ctc = GameObject.FindObjectOfType<CameraTransitionController>();

            sm = GetComponent<StateMachine>();
            sm.ChangeState(new PlayerState());
            
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
                op = GetComponent<OffsetPursue>();
            }
        }

        GameObject PickNewTarget()
        {
            creature = Mother.Instance.alive[
                Random.Range(0, Mother.Instance.alive.Count)
                ].gameObject;

            distance = creature.GetComponent<SpawnParameters>().viewingDistance;
            return Mother.Instance.GetCreature(creature);
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

        System.Collections.IEnumerator Journeying()
        {
            float journeyingMult = 10;
            while (true)
            {

                Debug.Log("Journeying");

                break;
                /*
                
                }
                */
                yield return new WaitForSeconds(1);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.J))
            {
                switch (controlType)
                {
                    case State.Player:                        
                        targetingCoroutine = StartCoroutine(Journeying());
                        break;
                    case State.Journeying:
                        Debug.Log("Player");
                        StopCoroutine(targetingCoroutine);
                        controlType = State.Player;
                        cruise.enabled = false;
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
                        
            if (controlType == State.Journeying)
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
