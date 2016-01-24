using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class playerSelectionManager : MonoBehaviour {

    public static int playerCount = 0;
    static public bool p1Selected = false;
    static public bool p2Selected = false;
    static public bool p3Selected = false;
    static public bool p4Selected = false;

    public GameObject p1Indicator;
    public GameObject p2Indicator;
    public GameObject p3Indicator;
    public GameObject p4Indicator;

    public GameObject p1Preview;
    public GameObject p2Preview;
    public GameObject p3Preview;
    public GameObject p4Preview;

    private GamePadState state1;
    private GamePadState state2;
    private GamePadState state3;
    private GamePadState state4;

    private PlayerIndex player1Index;
    private PlayerIndex player2Index;
    private PlayerIndex player3Index;
    private PlayerIndex player4Index;

    public AudioSource selectNoise;

    // Use this for initialization
    void Start () {

		playerCount = 0;

        if (!p1Selected) { p1Selected = false; }
        if (!p2Selected) { p2Selected = false; }
        if (!p3Selected) { p3Selected = false; }
        if (!p4Selected) { p4Selected = false; }


        player1Index = (PlayerIndex)0;
        player2Index = (PlayerIndex)1;
        player3Index = (PlayerIndex)2;
        player4Index = (PlayerIndex)3;

    }
	
	// Update is called once per frame
	void Update () {


        Debug.Log(playerCount);
        

        state1 = GamePad.GetState(player1Index);
        state2 = GamePad.GetState(player2Index);
        state3 = GamePad.GetState(player3Index);
        state4 = GamePad.GetState(player4Index);

        if (state1.IsConnected)
        {
            if (((state1.Buttons.A == ButtonState.Pressed)||(Input.GetKey(KeyCode.Alpha1))) && !p1Selected)
            {
                p1Selected = true;
                p1Indicator.GetComponent<MeshRenderer>().enabled = true;
                p1Preview.GetComponent<rotate>().enabled = true;
                playerCount++;
                selectNoise.Play();
            }
			GamePad.SetVibration (PlayerIndex.One, 0, 0);
        }
        else {
            Debug.Log("Controller 1 Not Connected.");
        }

        if (state2.IsConnected)
        {
            if (((state2.Buttons.A == ButtonState.Pressed) || (Input.GetKey(KeyCode.Alpha2))) && !p2Selected)
            {
                p2Selected = true;
                p2Indicator.GetComponent<MeshRenderer>().enabled = true;
                p2Preview.GetComponent<rotate>().enabled = true;
                playerCount++;
                selectNoise.Play();
			}
			GamePad.SetVibration (PlayerIndex.Two, 0, 0);
        }
        else {
            Debug.Log("Controller 2 Not Connected.");
        }

        if (state3.IsConnected)
        {
            if (((state3.Buttons.A == ButtonState.Pressed) || (Input.GetKey(KeyCode.Alpha3))) && !p3Selected)
            {
                p3Selected = true;
                p3Indicator.GetComponent<MeshRenderer>().enabled = true;
                p3Preview.GetComponent<rotate>().enabled = true;
                playerCount++;
                selectNoise.Play();
			}
			GamePad.SetVibration (PlayerIndex.Three, 0, 0);
        }
        else {
            Debug.Log("Controller 3 Not Connected.");
        }

        if (state4.IsConnected)
        {
            if (((state4.Buttons.A == ButtonState.Pressed) || (Input.GetKey(KeyCode.Alpha4))) && !p4Selected)
            {
                p4Selected = true;
                p4Indicator.GetComponent<MeshRenderer>().enabled = true;
                p4Preview.GetComponent<rotate>().enabled = true;
                playerCount++;
                selectNoise.Play();
			}
			GamePad.SetVibration (PlayerIndex.Four, 0, 0);
        }
        else {
            Debug.Log("Controller 4 Not Connected.");
        }

        if (Input.GetKey(KeyCode.Space) && playerCount >= 1)
        {
            
            Application.LoadLevel("Karl's_Battleship");
            
        }
    }
}
