using UnityEngine;
using System.Collections;
//using Ibuprogames.CameraTransitionsAsset;
using UnityEngine.SceneManagement;

namespace BGE.Forms
{
    public class PlayerController : MonoBehaviour {

        class PlayerState : State
        {
            PlayerController pc;

            public override void Enter()
            {
                //Debug.Log("Player control state");
                pc = owner.GetComponent<PlayerController>();
                pc.controlType = ControlType.Player;
                //pc.player.GetComponent<Rigidbody>().isKinematic = false;
                pc.vrController.enabled = true;
                pc.fc.enabled = true;
            }

            public override void Exit() { }

        }

        private bool assigned = false;

        class JourneyingState : State
        {
            PlayerController pc;
            Cruise c;
            public override void Enter()
            {
                //Debug.Log("Journeying state");
                pc = owner.GetComponent<PlayerController>();
                pc.controlType = ControlType.Journeying;
                c = pc.cruise;
                //c.preferredHeight = pos.y - BGE.Forms.WorldGenerator.Instance.SamplePos(pos.x, pos.z);
                if (!pc.assigned)
                {
                    pc.assigned = true;
                    pc.playerCruise.transform.position = pc.player.transform.position;
                }
                
                // pc.player.GetComponent<Rigidbody>().isKinematic = true;
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
               // pc.player.GetComponent<Rigidbody>().isKinematic = false;
                c.enabled = false;
            }
        }

        public void StartFollowing()
        {
            StartCoroutine(FollowCoRoutine());
        }

        System.Collections.IEnumerator FollowCoRoutine()
        {
            PlayerController pc;
            pc = this;
            pc.PickNewSpecies();
            yield return new WaitForSeconds(0.1f);
            pc.PickNewTarget();
            pc.controlType = ControlType.Following;
            pc.player.GetComponent<Rigidbody>().isKinematic = true;
            pc.vrController.enabled = false;
            pc.fc.enabled = false;
            WorldGenerator.Instance.ForceCheck();

            // Calculate the position to move to
            SpawnParameters sp = pc.species.GetComponent<SpawnParameters>();
            float a = sp.followCameraHalfFOV;
            float angle = Random.Range(-a, a);

            Vector3 lp = Quaternion.Euler(30, angle, 0) * Vector3.forward;
            lp.Normalize();
            lp *= pc.distance;
            Vector3 p = pc.creature.GetComponent<Boid>().TransformPoint(lp);
            //p = Utilities.TransformPointNoScale(lp, pc.creature.GetComponent<Boid>().transform);
            float y = WorldGenerator.Instance.SamplePos(p.x, p.z);
            if (p.y < y)
            {
                p.y = y + 50;
            }

            /*Debug.Log("Angle: " + angle);
            Debug.Log("lp: " + lp);
            Debug.Log("Desired position: " + p);
            Debug.Log("Viewing distance: " + sp.viewingDistance);
            Debug.Log("Boid pos: " + pc.creature.GetComponent<Boid>().position);
            Debug.Log("Camera pos: " + p);
            Debug.Log("leader: " + pc.creature);
            */

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
        }

        class FollowState : State
        {
            PlayerController pc;
            
            public override void Enter()
            {
                pc = owner.GetComponent<PlayerController>();
                pc.StartFollowing();



                

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

                //pc.player.GetComponent<Rigidbody>().isKinematic = false;
                pc.vrController.enabled = true;
                pc.fc.enabled = true;


                //pc.sm.CancelDelayedStateChange();
                //pc.playerBoid.enabled = false;
            }
        }

        StateMachine sm;

        public enum BuildType { Vive, Oculus, PC, VJ };

        public BuildType buildType = BuildType.Vive;

        public enum ControlType { Player, Journeying, Following };
        public ControlType controlType = ControlType.Player;

        public GameObject oculusStuff;
        public GameObject viveStuff;

        Seek seek;
        Boid playerBoid;
        SceneAvoidance sceneAvoidance;
        OffsetPursue op;
        GameObject player;

        public GameObject species;
        public GameObject creature;
        GameObject playerCruise;
        MonoBehaviour vrController;
        ForceController fc;
        Cruise cruise;
        //CameraTransition cameraTransition;

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
            ctc = GameObject.FindObjectOfType<CameraTransitionController>();
            ConfigureBuild();
        }

        int logoIndex = 0;
        int reactiveIndex = 0;
        int logoIndexToad = 0;
        int reactiveIndexToad = 0;
        int videoIndex = 100;

        public float journeying = 5.0f;
        public float delayMin = 30.0f;
        public float delayMax = 40.0f;

        public int creatureReps = 1;
        public float creaturesToLogosRatio = 1.5f;

        public System.Collections.IEnumerator Show()
        {
            StateMachine sm = GetComponent<StateMachine>();
            while (true)
            {
                WorldGenerator.Instance.SetGroundMaterialASync(0);
                while (logoIndex < ctc.leftEffects.Count)
                {
                    sm.ChangeState(new JourneyingState());
                    ctc.HideEffect();
                    yield return new WaitForSeconds(journeying);
                    ctc.left = logoIndex;
                    ctc.ShowLeftEffect();
                    logoIndex++;
                    yield return new WaitForSeconds(Random.Range(delayMin, delayMax));
                    ctc.HideEffect();
                    yield return new WaitForSeconds(2);
                    for (int i = 0; i < creatureReps; i++)
                    {
                        sm.ChangeState(new FollowState());
                        yield return new WaitForSeconds(Random.Range(delayMin * creaturesToLogosRatio, delayMax * creaturesToLogosRatio));
                        SpawnParameters sp = species.GetComponent<SpawnParameters>();
                        if (sp.effects.Length > 0)
                        {
                            int ei = Random.Range(0, sp.effects.Length);
                            ctc.ShowEffect(sp.effects[ei]);
                            yield return new WaitForSeconds(Random.Range(delayMin * creaturesToLogosRatio, delayMax * creaturesToLogosRatio));
                        }
                    }
                }
                while (reactiveIndex < ctc.rightEffects.Count)
                {
                    sm.ChangeState(new JourneyingState());
                    ctc.HideEffect();
                    yield return new WaitForSeconds(journeying);
                    ctc.right = reactiveIndex;
                    ctc.ShowRightEffect();
                    reactiveIndex++;
                    yield return new WaitForSeconds(Random.Range(delayMin, delayMax));
                    ctc.HideEffect();
                    yield return new WaitForSeconds(2);
                    for (int i = 0; i < creatureReps; i++)
                    {
                        sm.ChangeState(new FollowState());
                        yield return new WaitForSeconds(Random.Range(delayMin * creaturesToLogosRatio, delayMax * creaturesToLogosRatio));
                        SpawnParameters sp = species.GetComponent<SpawnParameters>();
                        if (sp.effects.Length > 0)
                        {
                            int ei = Random.Range(0, sp.effects.Length);
                            ctc.ShowEffect(sp.effects[ei]);
                            yield return new WaitForSeconds(Random.Range(delayMin * creaturesToLogosRatio, delayMax * creaturesToLogosRatio));
                        }
                    }                    
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
                        yield return new WaitForSeconds(Random.Range(delayMin * creaturesToLogosRatio, delayMax * creaturesToLogosRatio));
                        SpawnParameters sp = species.GetComponent<SpawnParameters>();
                        if (sp.effects.Length > 0)
                        {
                            int ei = Random.Range(0, sp.effects.Length);
                            ctc.ShowEffect(sp.effects[ei]);
                            yield return new WaitForSeconds(Random.Range(delayMin * creaturesToLogosRatio, delayMax * creaturesToLogosRatio));
                        }
                    }
                    videoIndex++;
                }
                sm.ChangeState(new JourneyingState());
                ctc.HideEffect();
                yield return new WaitForSeconds(journeying);
                newToad.Toad();
                while (logoIndexToad < ctc.leftEffects.Count)
                {                    
                    sm.ChangeState(new JourneyingState());
                    ctc.HideEffect();
                    yield return new WaitForSeconds(journeying);                    
                    ctc.left = logoIndexToad;
                    ctc.ShowLeftEffect();
                    logoIndexToad++;
                    yield return new WaitForSeconds(Random.Range(delayMin, delayMax));                    
                    //yield return new WaitForSeconds(2);                    
                }
                while (reactiveIndexToad < ctc.rightEffects.Count)
                {                    
                    sm.ChangeState(new JourneyingState());
                    ctc.HideEffect();
                    yield return new WaitForSeconds(journeying);
                    ctc.right = reactiveIndexToad;
                    ctc.ShowRightEffect();
                    reactiveIndexToad++;
                    yield return new WaitForSeconds(Random.Range(delayMin, delayMax));
                }
                newToad.Toad();
                logoIndex = 0;
                reactiveIndex = 0;
                videoIndex = 0;
                reactiveIndexToad = 0;
                logoIndexToad = 0;
            }
        }


        int nextSpecies = 0;

        GameObject PickNewSpecies()
        {
            //species = mother.GetSpecies(nextSpecies, true);
            nextSpecies = (nextSpecies + 1) % mother.prefabs.Length;
            return species;
        }

        GameObject PickNewTarget()
        {
            //creature = mother.GetCreature(species);
            distance = species.GetComponent<SpawnParameters>().viewingDistance;
            return creature;

            /*
            //nextSpecies = nextSpecies % mother.alive.Count;
            nextSpecies = (nextSpecies + 1) % mother.prefabs.Length;
            species = mother.alive[
                nextSpecies
                ].gameObject;
            creature = Mother.Instance.GetCreature(species);
            distance = species.GetComponent<SpawnParameters>().viewingDistance;
            return creature;
            */
        }


        // Use this for initialization
        void Start() {
            //AudioListener.pause = true;
            player = GameObject.FindGameObjectWithTag("Player");
            playerCruise = GameObject.FindGameObjectWithTag("PlayerCruise");

            fc = player.GetComponent<ForceController>();

            sm = GetComponent<StateMachine>();
            vrController = player.GetComponent<ViveController>();
            cruise = playerCruise.GetComponent<Cruise>();
            


            playerBoid = GameObject.FindGameObjectWithTag("PlayerBoid").GetComponent<Boid>();
            seek = playerBoid.GetComponent<Seek>();
            sceneAvoidance = playerBoid.GetComponent<SceneAvoidance>();
            op = playerBoid.GetComponent<OffsetPursue>();
            
            //sm = GetComponent<StateMachine>();
            //sm.ChangeState(new PlayerState());

            //cameraTransition = GameObject.FindObjectOfType<CameraTransition>();
            //if (cameraTransition == null)
            //    Debug.LogWarning(@"CameraTransition not found.");

            newToad = GetComponent<NewToad>();

            Invoke("LateStart", 5);

        }

        public void ConfigureBuild()
        {
            switch (buildType)
            {
                case BuildType.Oculus:
                    //oculusStuff.SetActive(true);
                    //viveStuff.SetActive(false);
                    //vrController = GetComponent<OculusController>();
                    //vrController.enabled = true;
                    //mother.maxcreatures = 2;
                    //GetComponent<ViveController>().enabled = false;
                    ////GetComponent<AudioSource>().enabled = true;
                    //ctc.enabled = false;
                    break;
                case BuildType.Vive:
                    oculusStuff.SetActive(false);
                    viveStuff.SetActive(true);
                    vrController = GetComponent<ViveController>();
                    vrController.enabled = true;
                    mother.maxcreatures = 2;
                    GetComponent<OculusController>().enabled = false;
                    //GetComponent<AudioSource>().enabled = true;
                    ctc.enabled = false;
                    break;
                case BuildType.PC:
                    oculusStuff.SetActive(false);
                    viveStuff.SetActive(true);
                    vrController = GetComponent<ViveController>();
                    vrController.enabled = false;
                    mother.maxcreatures = 15;
                    GetComponent<AudioSource>().enabled = false;
                    GetComponent<OculusController>().enabled = false;
                    ctc.enabled = false;
                    break;
                case BuildType.VJ:
                    oculusStuff.SetActive(false);
                    viveStuff.SetActive(true);
                    vrController = GetComponent<ViveController>();
                    vrController.enabled = false;
                    mother.maxcreatures = 10;
                    GetComponent<OculusController>().enabled = false;
                    GetComponent<AudioSource>().enabled = false;
                    ctc.enabled = true;
                    break;
            }
        }

        public void SetVRControllerEnabled(MonoBehaviour controller, bool enabled)
        {
            if (buildType == BuildType.Oculus || buildType == BuildType.Vive)
            {
                vrController.enabled = enabled;
            }
        }

        public float ellapsed = 0;
        public float toPass = 0.5f;
        public int clickCount = 0;

        public void LateStart()
        {
            StopAllCoroutines();
            showCoroutine = null;
            //showCoroutine = StartCoroutine(Show());
        }

        private void Update()
        {

        //    if (Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.J))
        //    {
        //        clickCount = (clickCount + 1) % 6;
        //        ellapsed = 0;                
        //    }
        //    ellapsed += Time.deltaTime;
        //    if (ellapsed > toPass && clickCount > 0)
        //    {
        //        switch (clickCount)
        //        {
        //            case 1:
        //                StopAllCoroutines();
        //                showCoroutine = null;
        //                sm.ChangeState(new JourneyingState());
        //                break;
        //            case 2:
        //                StopAllCoroutines();
        //                if (showCoroutine == null || ! (sm.currentState is FollowState))
        //                {                            
        //                    sm.ChangeState(new FollowState());
        //                }
        //                showCoroutine = null;
        //                break;
        //            case 3:
        //                StopAllCoroutines();
        //                showCoroutine = null;
        //                sm.ChangeState(new PlayerState());
        //                break;
        //            case 4:
        //                StopAllCoroutines();
        //                showCoroutine = null;
        //                showCoroutine = StartCoroutine(Show());
        //                break;
        //            case 5:
        //                newToad.Toad();
        //                break;
        //        }
        //        clickCount = 0;
        //    }

        //    switch (controlType)
        //    {
        //        case ControlType.Journeying:
        //            player.transform.localPosition = Vector3.zero;
        //            break;
        //        case ControlType.Following:
        //            player.transform.position = playerBoid.transform.position;
        //            player.transform.rotation = Quaternion.Slerp(player.transform.rotation
        //                , Quaternion.LookRotation(op.leaderBoid.transform.position - player.transform.position)
        //                , Time.deltaTime / 2
        //            );
        //            break;
        //    }
        }
    }
}
