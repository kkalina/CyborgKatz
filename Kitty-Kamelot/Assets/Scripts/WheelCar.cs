using UnityEngine;
using System.Collections;
using XInputDotNetPure;
using System.Collections.Generic;

public class WheelCar : MonoBehaviour {

    public GameObject playerCameraPrefab;
    public GameObject playerCameraObj;
    public Camera playerCamera;

    public Rigidbody shipRigid;

    public PlayerIndex playerIndexNum;
    private GamePadState state;

    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have



    // Use this for initialization
    void Start () {
        playerCameraObj = Instantiate(playerCameraPrefab);
        playerCamera = playerCameraObj.GetComponent<Camera>();
        playerCameraObj.GetComponent<ThirdPersonCamera>().poi = this.transform;
        playerCameraObj.GetComponent<ThirdPersonCamera>().camTarget = this.transform;
        playerCameraObj.GetComponent<ThirdPersonCamera>().playerIndexNum = playerIndexNum;
        if (playerIndexNum == XInputDotNetPure.PlayerIndex.One)
        {
            playerCameraObj.GetComponent<ThirdPersonCamera>().playerCamNumber = 1;
        }
        else if (playerIndexNum == XInputDotNetPure.PlayerIndex.Two)
        {
            playerCameraObj.GetComponent<ThirdPersonCamera>().playerCamNumber = 2;
        }
        else if (playerIndexNum == XInputDotNetPure.PlayerIndex.Three)
        {
            playerCameraObj.GetComponent<ThirdPersonCamera>().playerCamNumber = 3;
        }
        else if (playerIndexNum == XInputDotNetPure.PlayerIndex.Four)
        {
            playerCameraObj.GetComponent<ThirdPersonCamera>().playerCamNumber = 4;
        }

        shipRigid = this.gameObject.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    public void FixedUpdate()
    {
        //state = GamePad.GetState(playerIndexNum);
        //float v = state.Triggers.Right;
        //float h = state.ThumbSticks.Left.X;

        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
        }
    }


}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}
