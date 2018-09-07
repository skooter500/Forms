using UnityEngine;
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
                Debug.Log("Player control state");
                pc = owner.GetComponent<PlayerController>();
                pc.controlType = ControlType.Player;
                pc.player.GetComponent<Rigidbody>().isKinematic = false;
                pc.viveController.enabled = true;
                pc.fc.enabled = true;
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
                c = pc.cruise;
                //c.preferredHeight = pos.y - BGE.Forms.WorldGenerator.Instance.SamplePos(pos.x, pos.z);
                if (!assigned)
                {
                    assigned = true;
                    pc.playerCruise.transform.position = pc.player.transform.position;
                }
                
                pc.player.GetComponent<Rigidbody>().isKinematic = true;
                pc.player.transform.parent = pc.playerCruise.transform;
                pc.player.transform.localPosition = Vector3.zero;
                pc.player.transform.rotation = pc.cruise.transform.rotation;
                pc.fc.desiredRotation = pc.cruise.transform.rotation;
                c.gameObject.SetActive(true);
                c.enabled = true;
            }

            public override void Exit()
            {
                pc.player.transform.parent = null;
                pc.player.GetComponent<Rigidbody>().isKinematic = false;
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
                float a = pc.species.GetComponent<SpawnParameters>().followCameraHalfFOV;
                float angle = Random.Range(-a, a);
                Vector3 lp = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;
                lp.Normalize();
                lp *= pc.distance;
                Vector3 p = pc.creature.GetComponent<Boid>().TransformPoint(lp);
                //
                pc.playerBoid.enabled = true;
                pc.playerBoid.maxSpeed = pc.species.GetComponent<SpawnParameters>().followCameraSpeed;
                pc.playerBoid.desiredPosition = p;
                pc.playerBoid.transform.position = p;
                pc.playerBoid.UpdateLocalFromTransform();

                pc.op.leader = pc.creature;
                pc.playerBoid.velocity = pc.creature.GetComponent<Boid>().velocity;
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

                pc.player.GetComponent<Rigidbody>().isKinematic = false;
                pc.viveController.enabled = true;
                pc.fc.enabled = true;


                //pc.sm.CancelDelayedStateChange();
                //pc.playerBoid.enabled = false;
            }
        }

        StateMachine sm;

        public enum ControlType { Player, Journeying, Following };
        public ControlType controlType = ControlType.Player;

        Seek seek;
        Boid playerBoid;
        SceneAvoidance sceneAvoidance;
        OffsetPursue op;
        GameObject player;
        GameObject species;
        GameObject creature;
        GameObject playerCruise;
        ViveController viveController;
        ForceController fc;
        Cruise cruise;
        CameraTransition cameraTransition;

        public Mother mother;

        CameraTransitionController ctc;

        bool waiting = false;

        float distance = 500;

        public static PlayerController Instance;

        public Coroutine showCoroutine;
        NewToad newToad;

        public void Awake()
        {
            PlayerController.Instance = this;
        }

        int logoIndex = 0;
        int reactiveIndex = 0;
        int logoIndexToad = 0;
        int reactiveIndexToad = 0;
        int videoIndex = 0;

        public float journeying = 10.0f;
        public float delayMin = 30.0f;
        public float delayMax = 40.0f;
        public int creatureReps = 1;


        public System.Collections.IEnumerator Show()
        {
            StateMachine sm = GetComponent<StateMachine>();
            while (true)
            {
                while (logoIndex < ctc.leftEffects.Count)
                {
                    sm.ChangeState(new JourneyingState());
                    ctc.HideEffect();
                    yield return new WaitForSeconds(journeying);
                    ctc.left = logoIndex;
                    ctc.ShowLeftEffect();
                    yield return new WaitForSeconds(Random.Range(delayMin, delayMax));
                    ctc.HideEffect();
                    yield return new WaitForSeconds(2);
                    for (int i = 0; i < creatureReps; i++)
                    {
                        sm.ChangeState(new FollowState());
                        yield return new WaitForSeconds(Random.Range(delayMin, delayMax));
                    }
                    logoIndex++;
                }
                while (reactiveIndex < ctc.rightEffects.Count)
                {
                    sm.ChangeState(new JourneyingState());
                    ctc.HideEffect();
                    yield return new WaitForSeconds(journeying);
                    ctc.right = reactiveIndex;
                    ctc.ShowRightEffect();
                    yield return new WaitForSeconds(Random.Range(delayMin, delayMax));
                    ctc.HideEffect();
                    yield return new WaitForSeconds(2);
                    for (int i = 0; i < creatureReps; i++)
                    {
                        sm.ChangeState(new FollowState());
                        yield return new WaitForSeconds(Random.Range(delayMin, delayMax));
                    }
                    reactiveIndex++;
                }
                while (videoIndex < ctc.videoPlayer.videos.Count)
                {
                    sm.ChangeState(new JourneyingState());
                    ctc.HideEffect();
                    yield return new WaitForSeconds(journeying);
                    ctc.video = videoIndex;
                    ctc.ShowVideo();
                    yield return new WaitForSeconds(Random.Range(delayMin, delayMax));
                    ctc.HideEffect();
                    yield return new WaitForSeconds(2);
                    for (int i = 0; i < creatureReps; i++)
                    {
                        sm.ChangeState(new FollowState());
                        yield return new WaitForSeconds(Random.Range(delayMin, delayMax));
                    }
                    videoIndex++;
                }
                newToad.Toad();
                while (logoIndexToad < ctc.leftEffects.Count)
                {
                    sm.ChangeState(new JourneyingState());
                    ctc.HideEffect();
                    yield return new WaitForSeconds(journeying);
                    ctc.left = logoIndexToad;
                    ctc.ShowLeftEffect();
                    yield return new WaitForSeconds(Random.Range(delayMin, delayMax));
                    ctc.HideEffect();
                    yield return new WaitForSeconds(2);
                    logoIndexToad++;
                }
                while (reactiveIndexToad < ctc.rightEffects.Count)
                {
                    sm.ChangeState(new JourneyingState());
                    ctc.HideEffect();
                    yield return new WaitForSeconds(journeying);
                    ctc.right = reactiveIndexToad;
                    ctc.ShowRightEffect();
                    yield return new WaitForSeconds(Random.Range(delayMin, delayMax));
                    ctc.HideEffect();
                    yield return new WaitForSeconds(2);
                    reactiveIndexToad++;
                }
                newToad.Toad();
                logoIndex = 0;
                reactiveIndex = 0;
                videoIndex = 0;
                reactiveIndexToad = 0;
                logoIndexToad = 0;
            }
        }

        GameObject PickNewTarget()
        {
            species = mother.alive[
                Random.Range(0, mother.alive.Count)
                ].gameObject;
            creature = Mother.Instance.GetCreature(species);
            distance = species.GetComponent<SpawnParameters>().viewingDistance;
            return creature;
        }


        // Use this for initialization
        void Start() {
            //AudioListener.pause = true;
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

            newToad = GetComponent<NewToad>();

        }

        

        public float ellapsed = 0;
        public float toPass = 0.5f;
        public int clickCount = 0;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.J))
            {
                clickCount = (clickCount + 1) % 6;
                ellapsed = 0;                
            }
            ellapsed += Time.deltaTime;
            if (ellapsed > toPass && clickCount > 0)
            {
                switch (clickCount)
                {
                    case 1:
                        StopAllCoroutines();
                        sm.ChangeState(new JourneyingState());
                        break;
                    case 2:
                        StopAllCoroutines();
                        sm.ChangeState(new FollowState());
                        break;
                    case 3:
                        StopAllCoroutines();
                        sm.ChangeState(new PlayerState());
                        break;
                    case 4:
                        StopAllCoroutines();
                        showCoroutine = StartCoroutine(Show());
                        break;
                    case 5:
                        newToad.Toad();
                        break;
                }
                clickCount = 0;
            }

            switch (controlType)
            {
                case ControlType.Journeying:
                    player.transform.localPosition = Vector3.zero;
                    break;
                case ControlType.Following:
                    player.transform.position = playerBoid.transform.position;
                    player.transform.rotation = Quaternion.Slerp(player.transform.rotation
                        , Quaternion.LookRotation(op.leaderBoid.transform.position - player.transform.position)
                        , Time.deltaTime / 2
                    );
                    break;
            }
        }
    }
}
