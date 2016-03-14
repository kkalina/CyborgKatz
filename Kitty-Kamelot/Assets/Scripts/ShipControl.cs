using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class ShipControl : MonoBehaviour
{
    public AudioSource timeSlow;
    public AudioSource timeResume;

    public GameObject bullet;

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
    public bool grounded = false;

    public GravityEngine gravEngine;

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
    private float groundBufferTime = 0;

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

    public Collider belowCar;

    private System.Type MSHCOL = new MeshCollider().GetType();
    private System.Type BOXCOL = new BoxCollider().GetType();
    private System.Type SPHCOL = new SphereCollider().GetType();
    private System.Type CAPSCOL = new CapsuleCollider().GetType();
    private System.Type TERCOL = new TerrainCollider().GetType();
    private System.Type WHECOL = new WheelCollider().GetType();


    // Use this for initialization
    void Start()
    {
        gravEngine = this.gameObject.GetComponent<GravityEngine>();
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
        //Set the center of mass
        //shipRigid.centerOfMass = new Vector3(1f, -1f, 0);


        hoverSensor1 = transform.Find("HoverSensor 1").transform;
        hoverSensor2 = transform.Find("HoverSensor 2").transform;
        hoverSensor3 = transform.Find("HoverSensor 3").transform;
        hoverSensor4 = transform.Find("HoverSensor 4").transform;
        hoverSensors = new Transform[4] { hoverSensor1, hoverSensor2, hoverSensor3, hoverSensor4 };

        lancePoint = transform.Find("Lance").transform;
        shipState = shipStates.normalMode;

        
        
    }

    #region oldcode
    void Update()
    {

        if (Input.GetKey(KeyCode.D) || (health <= 0))
            dead = true;

        if (dead)
            die();


        state = GamePad.GetState(playerIndexNum);

        //Shooting
        if (state.Buttons.B == ButtonState.Pressed) {
            

        }

        //if ( Input.GetButtonDown ("Fire3" )) {
        if ((state.Buttons.X == ButtonState.Pressed) && (shipState != shipStates.rocketMode))
        {
            shipState = shipStates.rocketMode;

            //Falling sideways
            //StartCoroutine(gravEngine.tempGravChange(shipRigid.transform.forward, 1f, -40f));

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
        float p = state.ThumbSticks.Left.Y;
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
            //Grounded propulsion
            if (grounded == true || Time.time > groundBufferTime)
            {
                //gravEngine.direction = -Vector3.down; //Revert Gravity Engine
                shipRigid.AddRelativeForce(Vector3.forward * v * shipVelocity, ForceMode.Force);
                //shipRigid.AddRelativeForce(0f, 0f, 5f * h * shipRigid.velocity.x, ForceMode.Force);
                shipRigid.AddRelativeTorque(0f, h * turnSpeed, 0f, ForceMode.Force);
                //shipRigid.useGravity = false;
                shipRigid.drag = 1.4f;
                //Strafing
                //if (rstrafe)
                if (state.Buttons.RightShoulder == ButtonState.Pressed)
                    shipRigid.AddRelativeForce(Vector3.right * 30, ForceMode.Force);
                //else if (lstrafe)
                if (state.Buttons.LeftShoulder == ButtonState.Pressed)
                    shipRigid.AddRelativeForce(Vector3.left * 30, ForceMode.Force);
            }
            //Flight Propulsion
            else {
                //shipRigid.useGravity = true;
                gravEngine.ApplyGravity(); //Apply gravity engine
                shipRigid.drag = .3f;
                shipRigid.AddRelativeForce(Vector3.forward * v * shipVelocity/10f, ForceMode.Force);
                shipRigid.AddRelativeTorque(p * turnSpeed, h * turnSpeed, 0f, ForceMode.Force);
                //Strafing
                //if (rstrafe)
                if (state.Buttons.RightShoulder == ButtonState.Pressed){
                    shipRigid.AddRelativeTorque(Vector3.back * 8, ForceMode.Force);
                }
                //else if (lstrafe)
                if (state.Buttons.LeftShoulder == ButtonState.Pressed) {
                    shipRigid.AddRelativeTorque(Vector3.forward * 8, ForceMode.Force);
                }



            }




            
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
        #region terrain
        int thrustercount = 0;
        foreach (Transform thrustPoint in thrusters)
        {
            //reg thruster check
            if (Physics.Raycast(thrustPoint.position, -this.gameObject.transform.up, out groundHit, hoverHeight))
            {
                grounded = true;

                if (state.Buttons.A == ButtonState.Pressed)
                {
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
                grounded = false;
                groundBufferTime = Time.time + 1f;
                //ShipStabilizer();
            }

            //Gravity
            if (thrustercount == 3 && shipState != shipStates.rocketMode)
            {
                //float gravStr = 9.8f;
                Vector3 gravAng = Vector3.down;
                //possibly cast a bunch of rays and attract you to the shortest distance
                System.Type colType;
                if (!Physics.Raycast(shipRigid.position, -this.gameObject.transform.up, out groundHit, hoverHeight)
                    && Physics.Raycast(shipRigid.position, -this.gameObject.transform.up, out groundHit, 40f)
                    && groundHit.transform.gameObject.tag != "NoGrav")
                {
                    #region HighHover
                    if (groundHit.collider.tag != "NoGrav")
                    {
                        if (groundHit.collider == null && belowCar == null)
                        {
                            belowCar = GameObject.Find("Floor").GetComponent<Collider>();
                        }
                        else if ((belowCar == null && groundHit.collider != null) || groundHit.collider != belowCar)
                        {
                            belowCar = groundHit.collider;
                        }

                        if (belowCar == null)
                        {
                            belowCar = GameObject.Find("Floor").GetComponent<Collider>();
                        }
                    }
                    colType = belowCar.GetType();
                    Vector3[] pointMat = new Vector3[3];
                    float[] vertdists = new float[3];
                    Vector3 point = shipRigid.position;
                    Vector3[] thrusterGravs = new Vector3[4];

                    if (Types.Equals(colType, MSHCOL))
                    {
                        #region Mesh Colliders
                        vertdists[0] = (point - ((MeshCollider)belowCar).sharedMesh.vertices[0]).sqrMagnitude;
                        pointMat[0] = ((MeshCollider)belowCar).sharedMesh.vertices[0];

                        vertdists[1] = (point - ((MeshCollider)belowCar).sharedMesh.vertices[1]).sqrMagnitude;
                        pointMat[1] = ((MeshCollider)belowCar).sharedMesh.vertices[1];

                        vertdists[2] = (point - ((MeshCollider)belowCar).sharedMesh.vertices[2]).sqrMagnitude;
                        pointMat[2] = ((MeshCollider)belowCar).sharedMesh.vertices[2];

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
                        //gravStr = Mathf.Sqrt(Mathf.Sqrt((Mathf.Sqrt(vertdists[0]) + Mathf.Sqrt(vertdists[1]) + Mathf.Sqrt(vertdists[2])) / 3));
                        gravAng = -Vector3.RotateTowards(-shipRigid.transform.up, (pointMat[0] + pointMat[1] + pointMat[2]) / 3f, Mathf.PI / 8f, .2f * shipRigid.velocity.magnitude);
                        Physics.Raycast(shipRigid.position, gravAng, out groundHit, 20f);
                        //Debug.DrawRay(shipRigid.position, -gravAng, Color.red);

                        RaycastHit hov1out, hov2out, hov3out, hov4out;
                        Physics.Raycast(hoverSensor1.position, -gravAng, out hov1out, 20f);
                        Physics.Raycast(hoverSensor2.position, -gravAng, out hov2out, 20f);
                        Physics.Raycast(hoverSensor3.position, -gravAng, out hov3out, 20f);
                        Physics.Raycast(hoverSensor4.position, -gravAng, out hov4out, 20f);
                        thrusterGravs[0] = -Mathf.Pow(hov1out.distance, 2) * gravAng.normalized;
                        thrusterGravs[1] = -Mathf.Pow(hov2out.distance, 2) * gravAng.normalized;
                        thrusterGravs[2] = -Mathf.Pow(hov3out.distance, 2) * gravAng.normalized;
                        thrusterGravs[3] = -Mathf.Pow(hov4out.distance, 2) * gravAng.normalized;

                        shipRigid.AddForceAtPosition(thrusterGravs[0] * shipRigid.mass, hoverSensor1.position);
                        shipRigid.AddForceAtPosition(thrusterGravs[1] * shipRigid.mass, hoverSensor2.position);
                        shipRigid.AddForceAtPosition(thrusterGravs[2] * shipRigid.mass, hoverSensor3.position);
                        shipRigid.AddForceAtPosition(thrusterGravs[3] * shipRigid.mass, hoverSensor4.position);

                        //                        Debug.DrawRay(hoverSensor1.position, -gravAng, Color.green);
                        //                        Debug.DrawRay(hoverSensor1.position, thrusterGravs[0], Color.blue);
                        //                        Debug.DrawRay(hoverSensor2.position, -gravAng, Color.green);
                        //                        Debug.DrawRay(hoverSensor2.position, thrusterGravs[1], Color.blue);
                        //                        Debug.DrawRay(hoverSensor3.position, -gravAng, Color.green);
                        //                        Debug.DrawRay(hoverSensor3.position, thrusterGravs[2], Color.blue);
                        //                        Debug.DrawRay(hoverSensor4.position, -gravAng, Color.green);
                        //                        Debug.DrawRay(hoverSensor4.position, thrusterGravs[3], Color.blue);
                        #endregion
                    }
                    else if (Types.Equals(colType, SPHCOL))
                    {
                        shipRigid.AddForceAtPosition(Vector3.RotateTowards(hoverSensor1.position, belowCar.transform.position, Mathf.PI / 8f, .2f * shipRigid.velocity.magnitude) * shipRigid.mass,
                            hoverSensor1.position);
                        shipRigid.AddForceAtPosition(Vector3.RotateTowards(hoverSensor2.position, belowCar.transform.position, Mathf.PI / 8f, .2f * shipRigid.velocity.magnitude) * shipRigid.mass,
                            hoverSensor2.position);
                        shipRigid.AddForceAtPosition(Vector3.RotateTowards(hoverSensor3.position, belowCar.transform.position, Mathf.PI / 8f, .2f * shipRigid.velocity.magnitude) * shipRigid.mass,
                            hoverSensor3.position);
                        shipRigid.AddForceAtPosition(Vector3.RotateTowards(hoverSensor4.position, belowCar.transform.position, Mathf.PI / 8f, .2f * shipRigid.velocity.magnitude) * shipRigid.mass,
                            hoverSensor4.position);
                    }
                    else if (Types.Equals(colType, TERCOL) || belowCar.transform.up == Vector3.up || belowCar.tag == "Floor")
                    {
                        shipRigid.AddForceAtPosition(gravAng * shipRigid.mass, hoverSensor1.position);
                        shipRigid.AddForceAtPosition(gravAng * shipRigid.mass, hoverSensor2.position);
                        shipRigid.AddForceAtPosition(gravAng * shipRigid.mass, hoverSensor3.position);
                        shipRigid.AddForceAtPosition(gravAng * shipRigid.mass, hoverSensor4.position);
                    }
                    else if (Types.Equals(colType, BOXCOL))
                    {
                        RaycastHit hov1out, hov2out, hov3out, hov4out;
                        Physics.Raycast(hoverSensor1.position, -groundHit.normal, out hov1out, 40f);
                        Physics.Raycast(hoverSensor2.position, -groundHit.normal, out hov2out, 40f);
                        Physics.Raycast(hoverSensor3.position, -groundHit.normal, out hov3out, 40f);
                        Physics.Raycast(hoverSensor4.position, -groundHit.normal, out hov4out, 40f);

                        shipRigid.AddForceAtPosition(-hov1out.normal * (hoverSensor1.position - belowCar.transform.position).magnitude * shipRigid.mass, hoverSensor1.position);
                        shipRigid.AddForceAtPosition(-hov2out.normal * (hoverSensor2.position - belowCar.transform.position).magnitude * shipRigid.mass, hoverSensor2.position);
                        shipRigid.AddForceAtPosition(-hov3out.normal * (hoverSensor3.position - belowCar.transform.position).magnitude * shipRigid.mass, hoverSensor3.position);
                        shipRigid.AddForceAtPosition(-hov4out.normal * (hoverSensor4.position - belowCar.transform.position).magnitude * shipRigid.mass, hoverSensor4.position);
                    }
                    //Vector3 flightGravAngle = Vector3.RotateTowards(shipRigid.transform.up, groundHit.point, 2 * Mathf.PI, 0);
                    //shipRigid.AddForceAtPosition(-groundHit.normal * shipRigid.mass * Physics.gravity.magnitude * (groundHit.distance), hoverSensor1.position);
                    //shipRigid.AddForceAtPosition(-groundHit.normal * shipRigid.mass * Physics.gravity.magnitude * (groundHit.distance), hoverSensor2.position);
                    //shipRigid.AddForceAtPosition(-groundHit.normal * shipRigid.mass * Physics.gravity.magnitude * (groundHit.distance), hoverSensor3.position);
                    //shipRigid.AddForceAtPosition(-groundHit.normal * shipRigid.mass * Physics.gravity.magnitude * (groundHit.distance), hoverSensor4.position);

                    //shipRigid.transform.LookAt(shipRigid.transform.forward,groundHit.point.normalized);

                    //                    transform.Find("HoverCG").LookAt(new Vector3(groundHit.point.x, groundHit.point.y, groundHit.point.z ));
                    //                    shipRigid.transform.forward = Vector3.RotateTowards(shipRigid.transform.forward, -transform.Find("HoverCG").up, Mathf.Deg2Rad*2f, 10f);
                    //                    shipRigid.transform.RotateAround(shipRigid.transform.position, -shipRigid.position, 90);

                    //  shipRigid.rotation = Quaternion.LookRotation(new Vector3(flightGravAngle.x + 90f, flightGravAngle.y, flightGravAngle.z));
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
                #endregion
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
        #endregion
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

    void OnCollisionEnter(Collision coll)
    {
        grounded = true;
    }
}