using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class ShipControl : MonoBehaviour
{
    public AudioSource timeSlow;
    public AudioSource timeResume;

    public int health = 6;
    public bool dead = false;
    public GameObject debris;
    public GameObject shipExplosion;

    public GameObject LFlap;
    public GameObject RFlap;

    public AudioSource blast1;

    public PlayerIndex playerIndexNum;
    private GamePadState state;

    public GameObject playerCameraPrefab;
    public GameObject playerCameraObj;
    public Camera playerCamera;

    public GameObject EngineBeam;
    public GameObject RocketBeam;
    public GameObject EngineGlow;
    public Light engineLight;

    public const float baseV = 75f;
    public const float boostV = 150f;
    public const float stopV = 5f;
    public const float chargeRate = 1f / 30f;
    public const float burnRate = 1f / 10f;
    public const float chargeMeter = 30f;

    public float shipVelocity = 75f;
    public float turnSpeed = 25f;
    public float curCharge = 15f;
    public float jumpForce = 50f;

    public float hoverHeight = 0.7f;
    public float hoverForce = 3f;
    public float lanceRange = 1f;
    private RaycastHit groundHit;
    private RaycastHit lanceHit;
    public float correctionSpeed = 0.1f;
    public float leanForce = 1f;

    private Rigidbody shipRigid;
    private Transform lancePoint;

    public Transform hoverSensor1;
    public Transform hoverSensor2;
    public Transform hoverSensor3;
    public Transform hoverSensor4;
    private Transform[] hoverSensors;

    private Vector3 moveVec;

    public enum shipStates { normalMode, rocketMode };
    public shipStates shipState;

    private float rocketEndTime;
    public float rocketBoostDuration = 3f;
    public float rocketPower = 5f;

    public int normalFOV = 60;
    public int rocketFOV = 90;

    public float engineLightIntensityNormal = 4f;
    public float engineLightIntensityBoost = 8f;

    public float engineResponseSpeed = 0.4f;

    //Mesh beneath car and a list of all its vertices
    private Collider belowCar;
    private System.Collections.Generic.List<Collider> colliders;
    //private Vector3[][] mshVerts;

    // Use this for initialization
    void Start()
    {
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
        
        hoverSensor1 = transform.Find("HoverSensor 1").transform;
        hoverSensor2 = transform.Find("HoverSensor 2").transform;
        hoverSensor3 = transform.Find("HoverSensor 3").transform;
        hoverSensor4 = transform.Find("HoverSensor 4").transform;
        hoverSensors = new Transform[4] { hoverSensor1, hoverSensor2, hoverSensor3, hoverSensor4 };

        lancePoint = transform.Find("Lance").transform;
        shipState = shipStates.normalMode;

        Collider[] tempCol = GameObject.FindObjectsOfType(typeof(Collider)) as Collider[];
        foreach (Collider elt in tempCol)
            colliders.Add(elt);
        
    }

    #region oldcode
    void Update()
    {

        if (Input.GetKey(KeyCode.D) || (health <= 0))
            dead = true;

        if (dead)
            die();


        state = GamePad.GetState(playerIndexNum);

        //if ( Input.GetButtonDown ("Fire3" )) {
        if ((state.Buttons.X == ButtonState.Pressed) && (shipState != shipStates.rocketMode))
        {
            shipState = shipStates.rocketMode;
            rocketEndTime = Time.time + rocketBoostDuration;
            Debug.Log("ROCKET MODE");
            playerCamera.gameObject.GetComponent<ThirdPersonCamera>().shake = true;
            RocketBeam.gameObject.SetActive(true);
            RocketBeam.GetComponent<GeroBeam>().NowLength = 0;
            blast1.Play();
            GamePad.SetVibration(playerIndexNum, 0.5f, 0.2f * state.Triggers.Right);
        }

        //if (Input.GetAxis ("AccelTrigger") < -0.1) {
        //Debug.Log(state.Triggers.Right);
        if (state.Triggers.Right > 0.1f)
        {
            //Debug.Log (Input.GetAxis ("AccelTrigger"));
            EngineBeam.gameObject.SetActive(true);
            //EngineBeam.GetComponent<GeroBeam> ().NowLength = 0;

            /*if (this.gameObject.GetComponent<AudioSource> ().pitch < state.Triggers.Right) {
				this.gameObject.GetComponent<AudioSource> ().pitch += engineResponseSpeed * Time.deltaTime;
			} else if (this.gameObject.GetComponent<AudioSource> ().pitch > state.Triggers.Right) {
				this.gameObject.GetComponent<AudioSource> ().pitch -= engineResponseSpeed * Time.deltaTime;
			}*/

            if (shipState == shipStates.normalMode)
            {
                if (engineLight.intensity > engineLightIntensityNormal)
                {
                    engineLight.intensity -= 1f;
                }
                else if (engineLight.intensity < engineLightIntensityNormal)
                {
                    engineLight.intensity += 1f;
                }
                GamePad.SetVibration(playerIndexNum, 0f, 0.2f * state.Triggers.Right);
            }
            else {
                if (engineLight.intensity > engineLightIntensityBoost)
                {
                    engineLight.intensity -= 1f;
                }
                else if (engineLight.intensity < engineLightIntensityBoost)
                {
                    engineLight.intensity += 1f;
                }
                GamePad.SetVibration(playerIndexNum, 0.5f, 0.2f * state.Triggers.Right);
            }

        }
        else {
            EngineBeam.GetComponent<GeroBeam>().NowLength = 0;
            EngineBeam.gameObject.SetActive(false);

            /*if (this.gameObject.GetComponent<AudioSource> ().pitch > 0.1f) {
				this.gameObject.GetComponent<AudioSource> ().pitch -= engineResponseSpeed * Time.deltaTime;
			} else {
				this.gameObject.GetComponent<AudioSource> ().pitch = 0.05f;
			}*/

            if (shipState == shipStates.normalMode)
            {
                if (engineLight.intensity > 2f)
                {
                    engineLight.intensity -= 1f;
                }
                else if (engineLight.intensity < 2f)
                {
                    engineLight.intensity += 1f;
                }

                GamePad.SetVibration(playerIndexNum, 0f, 0f);
            }
            else {
                if (engineLight.intensity > engineLightIntensityBoost)
                {
                    engineLight.intensity -= 1f;
                }
                else if (engineLight.intensity < engineLightIntensityBoost)
                {
                    engineLight.intensity += 1f;
                }
            }

        }
    }
    #endregion

    // Update is called once per frame
    void FixedUpdate()
    {

        state = GamePad.GetState(playerIndexNum);

        if (this.gameObject.GetComponent<AudioSource>().pitch < (state.Triggers.Right))
        {
            this.gameObject.GetComponent<AudioSource>().pitch += engineResponseSpeed;
        }
        else if (this.gameObject.GetComponent<AudioSource>().pitch > (state.Triggers.Right + engineResponseSpeed))
        {
            this.gameObject.GetComponent<AudioSource>().pitch -= engineResponseSpeed;
        }
        if (this.gameObject.GetComponent<AudioSource>().pitch < 0.05f)
            this.gameObject.GetComponent<AudioSource>().pitch = 0.05f;
        if (shipState == shipStates.rocketMode)
            this.gameObject.GetComponent<AudioSource>().pitch = 1.25f;

        //float v = -Input.GetAxis("AccelTrigger");
        float v = state.Triggers.Right;
        //float h = Input.GetAxis("Horizontal");
        float h = state.ThumbSticks.Left.X;
        //bool rstrafe = Input.GetButton("StrafeR");
        //bool lstrafe = Input.GetButton("StrafeL");
        
        //Activate Hovering
        TerrainNormalHover(hoverSensors);



        if (h > 0.1f)
        {
            LFlap.GetComponent<Animator>().SetBool("flapOpen", true);
        }
        else
        {
            LFlap.GetComponent<Animator>().SetBool("flapOpen", false);
        }
        if (h < -0.1f)
        {
            RFlap.GetComponent<Animator>().SetBool("flapOpen", true);
        }
        else
        {
            RFlap.GetComponent<Animator>().SetBool("flapOpen", false);
        }

        if (shipState == shipStates.normalMode)
        {
            if (playerCamera.fieldOfView > normalFOV)
            {
                playerCamera.fieldOfView = playerCamera.fieldOfView - 1;
            }
            else if (playerCamera.fieldOfView < normalFOV)
            {
                playerCamera.fieldOfView = normalFOV;
            }
            if (playerCamera.gameObject.GetComponent<ThirdPersonCamera>().distance < 8f)
            {
                playerCamera.gameObject.GetComponent<ThirdPersonCamera>().distance += 1;
            }


            //ShipStabilizer();

            //Forward Motion
            //Boost ();
            shipRigid.AddRelativeForce(Vector3.forward * v * shipVelocity, ForceMode.Force);
            //shipRigid.AddRelativeForce(0f, 0f, 5f * h * shipRigid.velocity.x, ForceMode.Force);
            shipRigid.AddRelativeTorque(0f, h * turnSpeed, 0f, ForceMode.Force);

            

            //Strafing
            //if (rstrafe)
            if (state.Buttons.RightShoulder == ButtonState.Pressed)
                shipRigid.AddRelativeForce(Vector3.right * 30, ForceMode.Force);
            //else if (lstrafe)
            if (state.Buttons.LeftShoulder == ButtonState.Pressed)
                shipRigid.AddRelativeForce(Vector3.left * 30, ForceMode.Force);
            //Boost ();        
            //Leaning
            //shipRigid.AddRelativeTorque(-Vector3.forward * h * leanForce, ForceMode.Force);
            //Debug.Log(shipRigid.velocity.magnitude + ", " + curCharge.ToString());
            Lance();
        }
        else if (shipState == shipStates.rocketMode)
        {

            if (playerCamera.fieldOfView < rocketFOV)
            {
                playerCamera.fieldOfView = playerCamera.fieldOfView + 1;
            }
            else if (playerCamera.fieldOfView > rocketFOV)
            {
                playerCamera.fieldOfView = rocketFOV;
            }
            if (playerCamera.gameObject.GetComponent<ThirdPersonCamera>().distance > 6f)
            {
                playerCamera.gameObject.GetComponent<ThirdPersonCamera>().distance -= 0.1f;
            }


            playerCamera.gameObject.GetComponent<ThirdPersonCamera>().shakeIntensity = 0.05f * (Time.time - rocketEndTime);
            //ShipStabilizer();

            //Forward Motion
            shipRigid.AddRelativeForce(Vector3.forward * rocketPower * shipVelocity, ForceMode.Force);
            //shipRigid.AddRelativeForce(0f, 0f, 5f * h * shipRigid.velocity.x, ForceMode.Force);
            shipRigid.AddRelativeTorque(0f, h * turnSpeed, 0f, ForceMode.Force);

            //Leaning
            //shipRigid.AddRelativeTorque(-Vector3.forward * h * leanForce, ForceMode.Force);

            Lance();

            if (Time.time > rocketEndTime)
            {
                Debug.Log("Normal Mode");
                shipState = shipStates.normalMode;
                playerCamera.gameObject.GetComponent<ThirdPersonCamera>().shake = false;
                RocketBeam.gameObject.SetActive(false);


            }
        }
    }

    void TerrainNormalHover(Transform[] thrusters)
    {
        int thrustercount = 0;
        foreach (Transform thrustPoint in thrusters)
        {
            //reg thruster check
            if (Physics.Raycast(thrustPoint.position, -this.gameObject.transform.up, out groundHit, hoverHeight))
            {
                if (state.Buttons.A == ButtonState.Pressed)
                {
                    shipRigid.velocity = new Vector3(shipRigid.velocity.x, 0f, shipRigid.velocity.z);
                    shipRigid.AddForce(shipRigid.transform.up * jumpForce, ForceMode.Impulse);
                }
                //Do we want it this way?????
                shipRigid.AddForceAtPosition(groundHit.normal/*Vector3.up*/ * hoverForce * (hoverHeight - groundHit.distance), thrustPoint.position, ForceMode.Force);

                #region hoverdebug
                //You can post it in 3D coordinates.You must think of a sphere, rather than just a circle.
                //Let r = radius, t = angle on x-y plane, & p = angle off of z - axis. Then you get:
                //            float rx = Mathf.Sin(groundHit.normal.z) * Mathf.Cos(groundHit.normal.x);
                //            float rz = Mathf.Sin(groundHit.normal.z) * Mathf.Sin(groundHit.normal.x);
                //            Vector3 terrainGoal = new Vector3(rx, shipRigid.transform.localEulerAngles.y, rz);
                //shipRigid.AddTorque(shipRigid.transform.localEulerAngles - terrainGoal, ForceMode.Force);
                //shipRigid.transform.LookAt(shipRigid.transform.forward, groundHit.normal);
                //            Debug.Log("RX is " + rx + ", RZ is " + rz + " angleDiff = " + (shipRigid.transform.localEulerAngles - terrainGoal));

                //x = r * sin(p) * cos(t)
                //y = r * sin(p) * sin(t)
                //z = r * cos(p)
                #endregion
            }
            else {
                //ShipStabilizer();
            }

            //Gravity
            if (thrustercount == 3)
            {
                //possibly cast a bunch of rays and attract you to the shortest distance

                System.Type colType;
                if (Physics.Raycast(shipRigid.position, -this.gameObject.transform.up, out groundHit, 30f))
                {
                    /*
                    if (belowCar == null || (groundHit.collider != belowCar && groundHit.rigidbody.detectCollisions == true))
                    {
                        belowCar = groundHit.collider;
                    }

                    colType = belowCar.GetType();
                    Vector3[] pointMat = new Vector3[3];
                    float[] vertdists = new float[3] { 100000000000f, 100000000000f, 10000000000f };
                    Vector3 point = transform.InverseTransformPoint(shipRigid.position);

                    foreach (Vector3 vertex in ((MeshCollider)belowCar).sharedMesh.vertices)
                    {
                        Vector3 diff = point - vertex;
                        float distSqr = diff.sqrMagnitude;
                        float tempDist;
                        if (distSqr < (tempDist = Mathf.Min(vertdists)))
                        {
                            if (tempDist == vertdists[0])
                            {
                                vertdists[0] = distSqr;
                                pointMat[0] = vertex;
                            }
                            else if (tempDist == vertdists[1])
                            {
                                vertdists[1] = distSqr;
                                pointMat[1] = vertex;
                            }
                            else if (tempDist == vertdists[2])
                            {
                                vertdists[2] = distSqr;
                                pointMat[2] = vertex;
                            }
                        }
                    }
                    float gravStrength = vertdists[1] + vertdists[2] + vertdists[3];
                    Vector3 gravAngle = (point - pointMat[0]) - (point - pointMat[1]) - (point - pointMat[2]);
                    Physics.Raycast(shipRigid.position, gravAngle, out groundHit, 30f);

                    shipRigid.rotation = Quaternion.LookRotation(new Vector3(gravAngle.x + 90f, gravAngle.y, gravAngle.z));
                    //Vector3 flightGravAngle = groundHit.point - shipRigid.position;
                    shipRigid.AddForce(gravAngle * -1f * shipRigid.mass * Physics.gravity.magnitude * gravStrength / 2f);
                    Debug.DrawRay(shipRigid.position, gravAngle, Color.red);
                    Debug.DrawRay(shipRigid.position, pointMat[0].normalized, Color.green);
                    Debug.DrawRay(shipRigid.position, pointMat[1].normalized, Color.green);
                    Debug.DrawRay(shipRigid.position, pointMat[2].normalized, Color.green);
                    */
                    Vector3 flightGravAngle = Vector3.RotateTowards(shipRigid.transform.up, groundHit.point, 2 * Mathf.PI, 0);
                    shipRigid.AddForce(flightGravAngle * -1f * shipRigid.mass * Physics.gravity.magnitude);
                    shipRigid.rotation = Quaternion.LookRotation(new Vector3(flightGravAngle.x + 90f, flightGravAngle.y, flightGravAngle.z));
                }
                /*
                else
                {
                    if (Physics.Raycast(shipRigid.position, -this.gameObject.transform.up, out groundHit, 100f))
                    {
                        Vector3 flightGravAngle = Vector3.RotateTowards(shipRigid.transform.up, groundHit.point, 2 * Mathf.PI, 0);
                        shipRigid.AddForce(flightGravAngle * -1f * shipRigid.mass * Physics.gravity.magnitude);
                    }

                }

                shipRigid.transform.Find("HoverCG").LookAt(groundHit.point);
                */
            }
            ++thrustercount;
        }
        
        //do gravity
        //Vector3 forceangle = new Vector3(groundHit.normal.x, groundHit.normal.y, groundHit.normal.z);
        //shipRigid.AddForce(forceangle * -1f * shipRigid.mass * Physics.gravity.magnitude);
        //        Debug.DrawRay(shipRigid.transform.Find("HoverCG").position, groundHit.normal * -1f * shipRigid.mass * Physics.gravity.y, Color.blue);
        //        Debug.DrawRay(shipRigid.transform.Find("HoverCG").position, shipRigid.velocity * -1f * shipRigid.mass * Physics.gravity.y, Color.green);
        //        Debug.DrawRay(shipRigid.transform.Find("HoverCG").position, forceangle.normalized * -1f * shipRigid.mass * Physics.gravity.y, Color.red);
        //        Debug.DrawRay(shipRigid.transform.Find("HoverCG").position, groundHit.normal.eulerAngles.normalized * -1f * shipRigid.mass * Physics.gravity.y, Color.black);

    }

    void Lance()
    {
        if (Physics.Raycast(lancePoint.position, lancePoint.forward, out lanceHit, lanceRange))
        {
            if (lanceHit.collider.gameObject.tag == "Boss_Weakpoint")
            {
                Destroy(lanceHit.collider.gameObject);
            }
        }
        if (Physics.Raycast(lancePoint.position, lancePoint.forward, out lanceHit, lanceRange + 3.5f))
        {
            if (lanceHit.collider.gameObject.tag == "Boss_Weakpoint")
            {
                //Start Slow-Mo
                StartCoroutine(KittyTime());
            }
        }
    }

    //Hold Space/A (currently left alt) to slow down and drift and charge boost meter, let go to boost
    void Boost()
    {
        //get boost inputs
        bool bb = Input.GetButton("Fire3");

        if (bb && shipRigid.velocity.magnitude > 3 && curCharge > 0)
        {
            curCharge = System.Math.Max(curCharge - burnRate, 0);
            shipVelocity = boostV;
        }
        else
        {
            shipVelocity = baseV;
            curCharge = System.Math.Min(curCharge + chargeRate, chargeMeter);
        }


    }

    void ShipStabilizer()
    {
        Vector3 stableVec = shipRigid.transform.forward;
        stableVec.y = shipRigid.transform.localEulerAngles.y;
        if (shipRigid.transform.localEulerAngles != Vector3.zero)
        {
            shipRigid.transform.localEulerAngles = Vector3.Lerp(shipRigid.transform.localEulerAngles,
                stableVec, Time.time * correctionSpeed);
        }
    }

    //private bool kittyTimeActive = false;

    public IEnumerator KittyTime()
    {
        //if (!kittyTimeActive)
        //{
            // suspend execution for 5 seconds
            Time.timeScale = .25f;
            Time.fixedDeltaTime = .005f;
            //timeSlow.Play();
            //kittyTimeActive = true;
            yield return new WaitForSeconds(0.3f);
            Time.timeScale = 1f;
            Time.fixedDeltaTime = .01f;
            //timeResume.Play();
            //kittyTimeActive = false;
        //}
    }

    void die()
    {
        GamePad.SetVibration(playerIndexNum, 0f, 0f);
        GameObject debrisGO = Instantiate(debris);
        debrisGO.transform.position = this.transform.position;
        debrisGO.transform.rotation = this.transform.rotation;
        //debrisGO.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity;
        GameObject shipEXPLAD = Instantiate(shipExplosion);
        shipEXPLAD.transform.position = this.transform.position;
        Destroy(this.gameObject);
    }
}