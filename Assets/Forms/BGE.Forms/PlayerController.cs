﻿using UnityEngine;
using System.Collections;
using Ibuprogames.CameraTransitionsAsset;

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
                pc.fc.enabled = true;

                pc.cruise.enabled = false;
            }
            public override void Exit() { }

        }

        class JourneyingState : State
        {
            PlayerController pc;
            Cruise c;
            private static bool assigned = false;
            public override void Enter()
            {
                Debug.Log("Journeying state");
                pc = owner.GetComponent<PlayerController>();
                pc.controlType = ControlType.Journeying;
                Vector3 pos = owner.transform.position;
                c = pc.cruise;
                //c.preferredHeight = pos.y - BGE.Forms.WorldGenerator.Instance.SamplePos(pos.x, pos.z);
                c.enabled = true;

                if (!assigned)
                {
                    assigned = true;
                    pc.playerCruise.transform.position = pc.player.transform.position;
                    pc.playerCruise.transform.rotation = pc.player.transform.rotation;
                }
                else
                {
                    pc.player.transform.position = pc.cruise.transform.position;
                    pc.player.transform.rotation = pc.cruise.transform.rotation;
                    pc.fc.desiredRotation = pc.player.transform.rotation;
                }
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
                pc.fc.enabled = false;
                pc.PickNewTarget();
                // Calculate the position to move to
                float angle = Random.Range(-30, 30);
                Vector3 lp = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;
                lp *= pc.distance;
                Vector3 p = pc.creature.GetComponent<Boid>().TransformPoint(lp);
                //
                pc.playerBoid.enabled = true;
                pc.playerBoid.maxSpeed = 300;
                pc.playerBoid.desiredPosition = p;
                pc.playerBoid.transform.position = p;
                pc.playerBoid.UpdateLocalFromTransform();

                pc.op.leader = pc.creature;
                pc.op.Start();
                Utilities.SetActive(pc.sceneAvoidance, true);
                Utilities.SetActive(pc.op, true);
                pc.player.transform.position = p;
                pc.player.transform.rotation =
                    Quaternion.LookRotation(pc.op.leaderBoid.transform.position - p);

                Utilities.SetActive(pc.op, true);
                Utilities.SetActive(pc.seek, false);
                Utilities.SetActive(pc.sceneAvoidance, true);

                //pc.sm.ChangeStateDelayed(new FollowState(), Random.Range(20, 30));

                /*
                pc.playerCruise.GetComponent<Camera>().enabled = true;
                pc.cameraTransition.ProgressMode = CameraTransition.ProgressModes.Automatic;
                pc.cameraTransition.DoTransition(
                    CameraTransitionEffects.FadeToColor
                    , pc.playerCruise.GetComponent<Camera>()
                    , pc.player.GetComponentInChildren<Camera>()
                    , 1.0f, false, new object[] { 0.5f, Color.black});
                    */
            }

            public override void Exit()
            {
                Quaternion q = Quaternion.LookRotation(pc.op.leaderBoid.transform.position - pc.player.transform.position);
                Vector3 euler = q.eulerAngles;
                q = Quaternion.Euler(euler.x, euler.y, 0);
                pc.fc.desiredRotation = q;
                Utilities.SetActive(pc.sceneAvoidance, false);
                Utilities.SetActive(pc.op, false);
                //pc.sm.CancelDelayedStateChange();
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
        GameObject playerCruise;
        ViveController viveController;
        ForceController fc;
        Cruise cruise;
        CameraTransition cameraTransition;


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
            playerCruise = GameObject.FindGameObjectWithTag("PlayerCruise");


            fc = player.GetComponent<ForceController>();

            sm = GetComponent<StateMachine>();
            viveController = player.GetComponent<ViveController>();
            cruise = playerCruise.GetComponent<Cruise>();
            ctc = GameObject.FindObjectOfType<CameraTransitionController>();


            playerBoid = GameObject.FindGameObjectWithTag("PlayerBoid").GetComponent<Boid>();
            seek = playerBoid.GetComponent<Seek>();
            sceneAvoidance = playerBoid.GetComponent<SceneAvoidance>();
            op = playerBoid.GetComponent<OffsetPursue>();
            
            sm = GetComponent<StateMachine>();
            sm.ChangeState(new PlayerState());

            cameraTransition = GameObject.FindObjectOfType<CameraTransition>();
            if (cameraTransition == null)
                Debug.LogWarning(@"CameraTransition not found.");

        }


        public float ellapsed = 0;
        public float toPass = 0.3f;
        public int clickCount = 0;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.J))
            {
                clickCount = (clickCount + 1) % 4;
                ellapsed = 0;                
            }
            ellapsed += Time.deltaTime;
            if (ellapsed > toPass && clickCount > 0)
            {
                switch (clickCount)
                {
                    case 1:
                        sm.ChangeState(new JourneyingState());
                        break;
                    case 2:
                        sm.ChangeState(new FollowState());
                        break;
                    case 3:
                        sm.ChangeState(new PlayerState());
                        break;
                }
                clickCount = 0;
            }

            switch (controlType)
            {
                case ControlType.Journeying:
                    player.transform.position = cruise.transform.position;
                    player.transform.rotation = cruise.transform.rotation;
                    break;
                case ControlType.Following:
                    player.transform.position = playerBoid.transform.position;
                    player.transform.rotation = Quaternion.Slerp(player.transform.rotation
                        , Quaternion.LookRotation(op.leaderBoid.transform.position - player.transform.position)
                        , Time.deltaTime
                    );
                    break;
            }
        }


        //void FixedUpdate()
        //{
            
        //}
    }
}
