using UnityEngine;
using XInputDotNetPure;
using System.Collections;
using System.Collections.Generic;

public class ArcadeVehicle : MonoBehaviour {
	[Header("Set in Inspector")]
	public Vector3		centerOfMass = new Vector3(0,-1,0);
	public Vector3		accelPointRel = new Vector3(0, -.25f, 0.5f);
	public float		groundedDrag = 0.5f;
	public float		groundedAngularDrag = 2;
	public float		airDrag = 0;
	public float		airAngularDrag = 2;
	public float		accel = 10;
	public float		turningTorque = 10;
	public float		horizontalFriction = 0.5f;
	public float		horizontalFrictionSkidding = 0.2f;

    public GameObject playerCameraPrefab;
    private GameObject playerCameraObj;
    private Camera playerCamera;
    public PlayerIndex playerIndexNum;
    private GamePadState state;


    [Header("Calculated Dynamically")]
	public float		iH;
	public float		iV;
	public bool			grounded;
	public bool			skidding;
	public List<Shock>	shocks;


	Rigidbody			rigid;



	void Awake() {
		rigid = GetComponent<Rigidbody>();
		rigid.centerOfMass = new Vector3(0,-1,0);
		shocks = new List<Shock>(transform.GetComponentsInChildren<Shock>());

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
    }

	void FixedUpdate () {
		iH = Input.GetAxis("Horizontal");
		iV = Input.GetAxis("Vertical");

		grounded = Grounded;
		skidding = Skidding;

		// Steering
		if (grounded) rigid.AddTorque(0,iH*turningTorque,0);

		rigid.centerOfMass = centerOfMass;

		Vector3 fwdAlongGround = transform.forward;
		fwdAlongGround.y = 0;
		fwdAlongGround.Normalize();

		Vector3 accelPoint;
		if (grounded) {
			rigid.drag = groundedDrag;
			rigid.angularDrag = groundedAngularDrag;

			accelPoint = transform.TransformPoint(accelPointRel);

			Debug.DrawLine(transform.position, accelPoint);
			rigid.AddForceAtPosition(iV * accel * fwdAlongGround, accelPoint);

			// Add horizontal resistance to mimic directional friction of tires
			// This could use some work!
			Vector3 vel = rigid.velocity;
			Vector3 horizontalVel = transform.right * Vector3.Dot(transform.right, vel);
			horizontalVel.y = 0; // nullify y effects of this

			if (!skidding) {
				rigid.AddForce(-horizontalVel*horizontalFriction, ForceMode.VelocityChange);
			} else {
				rigid.AddForce(-horizontalVel*horizontalFrictionSkidding, ForceMode.VelocityChange);
			}
		} else {
			rigid.drag = airDrag;
			rigid.angularDrag = airAngularDrag;
			//accelPoint = transform.TransformPoint(centerOfMass);
		}
	}

	public bool Grounded {
		get {
			foreach (Shock sh in shocks) {
				if (sh.grounded) return true;
			}
			return false;
		}
	}

	public bool Skidding {
		get {
			return Input.GetKey(KeyCode.LeftShift);
		}
	}
}
