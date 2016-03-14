using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class ThirdPersonCamera : MonoBehaviour
{
    public GameObject player1Icon;
    public GameObject player2Icon;
    public GameObject player3Icon;
    public GameObject player4Icon;

    public int playerCamNumber;
	public int numPlayers;
    public PlayerIndex playerIndexNum;

	public Transform poi;
	public Transform camTarget;
	public float distance = 5, height = 1;
	public float u = 01f;
	private float v = 01f;

	public bool shake = false;
	public float shakeIntensity = 0.1f;
    private Camera cam;

    private GamePadState state;

    void Start()
    {
        if(playerCamNumber == 1)
            player1Icon.SetActive(true);
        else if (playerCamNumber == 2)
            player2Icon.SetActive(true);
        else if (playerCamNumber == 3)
            player3Icon.SetActive(true);
        else 
            player4Icon.SetActive(true);

        numPlayers = playerSelectionManager.playerCount;

        cam = this.gameObject.GetComponent<Camera>();

        if (numPlayers <= 1)
        {
            cam.rect = new Rect(0, 0, 1, 1);
        }
        else if (numPlayers == 2)
        {
            //Debug.Log("Setting Cam Rect");
            cam.rect = new Rect(0, 0.5f*playerCamNumber-0.5f, 1, 0.5f);
        }
        else
        {
            if(playerCamNumber == 1)
                cam.rect = new Rect(0, 0, 0.5f, 0.5f);
            if (playerCamNumber == 2)
                cam.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            if (playerCamNumber == 3)
                cam.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
            if (playerCamNumber == 4)
                cam.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
        }
        
    }

	void FixedUpdate()
	{

        state = GamePad.GetState(playerIndexNum);

        Vector3 pos = poi.position;
        if(state.Buttons.Y == ButtonState.Pressed)
		    pos -= poi.forward * -distance;
        else
            pos -= poi.forward * distance;
        pos += poi.up * height;
		Vector3 rot = poi.localEulerAngles;

		Vector3 pos2 = (1 - u) * transform.position + u * pos;
		//Vector3 rot2 = (1 - v) * transform.localEulerAngles + v * rot;
        //rot2.y = poi.transform.localEulerAngles.y;



        /*if (shake) {
			pos2.x += Random.value * shakeIntensity * Random .Range(-1, 1);
			pos2.y += Random.value * shakeIntensity * Random .Range(-1, 1);
			pos2.z += Random.value * shakeIntensity * Random .Range(-1, 1);
		}*/

        
		transform.position = pos2;

        transform.rotation = Quaternion.Slerp(camTarget.rotation, transform.rotation, 2f * Time.fixedDeltaTime);
        //transform.localEulerAngles = rot2;

    }
}
