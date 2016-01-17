using UnityEngine;
using System.Collections;

public class AerialControl : MonoBehaviour {

    private Rigidbody shipRigid;
    public Transform hoverPoint;
    private Vector3 moveVec;
    private RaycastHit groundHit;

    public float gravity = 4.9f;
    public const float airSpeed = 60;
    public const float yawSpeed = 5f;
    public const float pitchSpeed = 40f;

    // Use this for initialization
    void Start () {
        shipRigid = this.gameObject.GetComponent<Rigidbody>();
        hoverPoint = transform.Find("HoverSensor").transform;
    }
	
	// Update is called once per frame
	void Update () {
	    if (gliding())
        {
            float a = -Input.GetAxis("AccelTrigger");
            float y = Input.GetAxis("Horizontal");
            float p = Input.GetAxis("Vertical");
            bool rstrafe = Input.GetButton("StrafeR");
            bool lstrafe = Input.GetButton("StrafeL");

            shipRigid.AddRelativeForce(Vector3.forward * a * airSpeed, ForceMode.Force);
            shipRigid.AddRelativeForce(Vector3.up * p * gravity, ForceMode.Force);
            //shipRigid.AddRelativeForce(0f, 0f, 5f * h * shipRigid.velocity.x, ForceMode.Force);
            shipRigid.AddTorque(0f, y * yawSpeed, 0f, ForceMode.Force);
            shipRigid.AddRelativeTorque(p * pitchSpeed, 0f, 0f, ForceMode.Force);

            //Strafing
            if (rstrafe)
                shipRigid.AddRelativeForce(Vector3.right * 30, ForceMode.Force);
            else if (lstrafe)
                shipRigid.AddRelativeForce(Vector3.left * 30, ForceMode.Force);

            Debug.Log(shipRigid.position.y);
        }
    }

    //Determine whether or not you're gliding
    bool gliding()
    {
        return !Physics.Raycast(transform.position, -Vector3.up, out groundHit, 3f) && shipRigid.velocity.magnitude > 10;
    }
}
