using UnityEngine;
using System.Collections;

namespace BGE.Forms
{
    public class PlayerController : MonoBehaviour {

        class PlayerState : State
        {
            PlayerController pc;

            public override void Enter()
            {
                pc = owner.GetComponent<PlayerController>();
                pc.controlType = ControlType.Player;
                pc.player.GetComponent<Rigidbody>().isKinematic = false;
                pc.viveController.enabled = true;
                pc.player.GetComponent<ForceController>().enabled = true;
                pc.cruise.enabled = false;
            }
            public override void Exit() { }

        }

        class JourneyingState : State
        {
            PlayerController pc;
            Cruise c;
            public override void Enter()
            {
                Debug.Log("Journeying state");
                pc = owner.GetComponent<PlayerController>();
                pc.controlType = ControlType.Journeying;
                Vector3 pos = owner.transform.position;
                c = pc.cruise;
                c.preferredHeight = pos.y - BGE.Forms.WorldGenerator.Instance.SamplePos(pos.x, pos.z);
                c.enabled = true;
            }

            public override void Exit()
            {
                c.enabled = false;
            }
        }

        class FollowState : State
        {
            PlayerController pc;
            
            public override void Enter()
            {
                pc = owner.GetComponent<PlayerController>();
                pc.controlType = ControlType.Following;
                pc.player.GetComponent<Rigidbody>().isKinematic = true;
                pc.viveController.enabled = false;
                pc.player.GetComponent<ForceController>().enabled = false;
                pc.PickNewTarget();                
                // Calculate the position to move to
                Vector3 lp = Random.insideUnitSphere * pc.distance;
                lp.z = Mathf.Abs(lp.z);
                lp.y = 0;
                Vector3 p = pc.creature.GetComponent<Boid>().TransformPoint(lp);
                //
                pc.playerBoid.enabled = true;
                pc.playerBoid.maxSpeed = 300;
                pc.playerBoid.desiredPosition = p;
                pc.playerBoid.transform.position = p;
                pc.playerBoid.UpdateLocalFromTransform();

                pc.op.leader = pc.creature;
                pc.op.Start();
                pc.player.transform.position = p;
                pc.player.transform.rotation =
                    Quaternion.LookRotation(pc.op.leaderBoid.transform.position - p);

                Utilities.SetActive(pc.op, true);
                Utilities.SetActive(pc.sceneAvoidance, true);
            }

            public override void Exit()
            {
                Quaternion q = Quaternion.LookRotation(pc.op.leaderBoid.transform.position - pc.player.transform.position);
                Vector3 euler = q.eulerAngles;
                q = Quaternion.Euler(euler.x, euler.y, 0);
                pc.player.GetComponent<ForceController>().desiredRotation = q;
                //pc.playerBoid.enabled = false;
            }
        }

        StateMachine sm;

        public enum ControlType { Player, Journeying, Following };
        public ControlType controlType = ControlType.Player;

        public float minHeight = 10;
        public float maxHeight = 1000;
        public float fov;

        Seek seek;
        Boid playerBoid;
        SceneAvoidance sceneAvoidance;
        OffsetPursue op;
        public float seekDistange = 5000;
        GameObject player;
        GameObject species;
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

        GameObject PickNewTarget()
        {
            species = Mother.Instance.alive[
                Random.Range(0, Mother.Instance.alive.Count)
                ].gameObject;
            creature = Mother.Instance.GetCreature(species);
            distance = species.GetComponent<SpawnParameters>().viewingDistance;
            return creature;
        }


        // Use this for initialization
        void Start() {
            player = GameObject.FindGameObjectWithTag("Player");
            sm = GetComponent<StateMachine>();
            viveController = player.GetComponent<ViveController>();
            cruise = GetComponent<Cruise>();
            ctc = GameObject.FindObjectOfType<CameraTransitionController>();


            playerBoid = GameObject.FindGameObjectWithTag("PlayerBoid").GetComponent<Boid>();
            seek = playerBoid.GetComponent<Seek>();
            sceneAvoidance = playerBoid.GetComponent<SceneAvoidance>();
            op = playerBoid.GetComponent<OffsetPursue>();
            
            sm = GetComponent<StateMachine>();
            sm.ChangeState(new PlayerState());            
        }

        /*void AssignBehaviours()
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
        */

        
        /*
        System.Collections.IEnumerator Journeying()
        {

            float journeyingMult = 10;
            while (true)
            {

                Debug.Log("Journeying");

                break;
                
                }
                yield return new WaitForSeconds(1);
            }
        }
        */

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.J))
            {

                switch (controlType)
                {
                    case ControlType.Player:
                        sm.ChangeState(new JourneyingState());
                        break;
                    case ControlType.Journeying:
                        sm.ChangeState(new FollowState());
                        break;
                    case ControlType.Following:
                        sm.ChangeState(new PlayerState());
                        break;
                }
                /*
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
                */
            }
        }


        void FixedUpdate()
        {
            if (controlType == ControlType.Following)
            {
                player.transform.position = playerBoid.transform.position;
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation
                    , Quaternion.LookRotation(op.leaderBoid.transform.position - player.transform.position)
                    , Time.deltaTime
                );
            }
            /*
                //player.transform.position = Vector3.Lerp(
                //    player.transform.position
                //    , this.transform.position
                //    , Time.deltaTime
                //    );
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
            */
        }
    }
}
