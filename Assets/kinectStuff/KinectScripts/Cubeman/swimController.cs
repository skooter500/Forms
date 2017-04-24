using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swimController : MonoBehaviour
{
    protected KinectManager kinectManager;
    private Vector3 oldLeftHandPos;
    private Vector3 oldRightHandPos;
    public Vector3 force;
    private Quaternion rotForce;

    public GameObject player;
    private Vector3 combinedHandMovement;
    public Vector3 combinedHandDir;
    public float speed;
    public float dampening;

    // Use this for initialization
    void Start () {
        oldLeftHandPos = new Vector3(0, 0, 0);
        oldRightHandPos = new Vector3(0, 0, 0);
        combinedHandMovement = new Vector3(0, 0, 0);
        force = new Vector3(0, 0, 0);
        rotForce = Quaternion.Euler(0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {

        if (kinectManager == null)
        {
            kinectManager = KinectManager.Instance;
        }
        else
        {
            if (kinectManager.IsUserDetected())
            {
                //Initialize hand, head, leftHand and rightHand vectors
                Vector3 rightHandPos = new Vector3(0, 0, 0);
                Vector3 leftHandPos = new Vector3(0, 0, 0);
                Vector3 playerPos = kinectManager.GetUserPosition(kinectManager.GetPlayer1ID());
                Vector3 worldHandPos = kinectManager.GetJointPosition(kinectManager.GetPlayer1ID(), (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight);
                Vector3 worldLeftHandPos = kinectManager.GetJointPosition(kinectManager.GetPlayer1ID(), (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft);

                leftHandPos = worldLeftHandPos - playerPos;
                rightHandPos = worldHandPos - playerPos;

                //flip the Z positon to match the cubemanController
                leftHandPos.z = -leftHandPos.z;
                rightHandPos.z = -rightHandPos.z;

                if(oldLeftHandPos == new Vector3(0,0,0))
                {
                    oldLeftHandPos = leftHandPos;
                }
                if(oldRightHandPos == new Vector3(0,0,0))
                {
                    oldRightHandPos = rightHandPos;
                }
                float leftHandDist = Vector3.Distance(oldLeftHandPos, leftHandPos);
                float rightHandDist = Vector3.Distance(rightHandPos, oldRightHandPos);
                if ((leftHandDist >= 0.1f) || (rightHandDist >= 0.1f))
                {
                    Vector3 relLeftHandPos = oldLeftHandPos - leftHandPos;
                    Vector3 relRightHandPos = oldRightHandPos - rightHandPos;
                    Vector3 leftHandDir = Vector3.Normalize(relLeftHandPos);
                    Vector3 rightHandDir = Vector3.Normalize(relRightHandPos);

                    combinedHandMovement = relLeftHandPos + relRightHandPos;
                    combinedHandMovement = Quaternion.Inverse(Quaternion.Euler(combinedHandMovement)) * combinedHandMovement;
                    if(leftHandDist >= 0.3f || rightHandDist >= 0.3f)
                    {
                        Vector3 tempHandDir = leftHandDir + rightHandDir;
                        tempHandDir.x = -tempHandDir.x * 15;
                        tempHandDir.z = 0;
                        combinedHandDir += tempHandDir;
                    }
                    //combinedHandDir = Quaternion.Inverse(combinedHandDir);

                    force += combinedHandMovement * speed * (Vector3.Distance(oldLeftHandPos, leftHandPos) + Vector3.Distance(oldRightHandPos, rightHandPos));
                    //rotForce = rotForce * combinedHandDir;
                    //
                }

                oldLeftHandPos = leftHandPos;
                oldRightHandPos = rightHandPos;
                
            }
            player.transform.position += transform.rotation * force * Time.deltaTime;
            Quaternion rotChange = Quaternion.Inverse(Quaternion.Euler(combinedHandDir.y, combinedHandDir.x, combinedHandDir.z));
            player.transform.rotation = player.transform.rotation * rotChange;
            //player.transform.rotation = player.transform.rotation * rotForce;
            //rotForce = player.transform.rotation;
            force = force / dampening;
            combinedHandDir = combinedHandDir / (dampening * 1.5f);
            //rotForce = rotForce / dampening;
        }
    }
}
