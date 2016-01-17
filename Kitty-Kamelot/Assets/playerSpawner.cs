using UnityEngine;
using System.Collections;

public class playerSpawner : MonoBehaviour {
    public bool p1active = false;
    public bool p2active = false;
    public bool p3active = false;
    public bool p4active = false;

    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;


    public GameObject playerPrefab;

    public float playerSpacing = 2f;

    public bool gameOver = false;
    public float gameOverDelay = 5f;
    private float gameOverTime = 0f;

    // Use this for initialization
    void Start () {

        p1active = playerSelectionManager.p1Selected;
        p2active = playerSelectionManager.p2Selected;
        p3active = playerSelectionManager.p3Selected;
        p4active = playerSelectionManager.p4Selected;

        if (p1active)
        {
            player1 = Instantiate(playerPrefab);
            player1.GetComponent<ShipControl>().playerIndexNum = XInputDotNetPure.PlayerIndex.One;
            player1.transform.position = new Vector3(this.transform.position.x + (playerSpacing*1),this.transform.position.y,this.transform.position.z);
        }
        if (p2active)
        {
            player2 = Instantiate(playerPrefab);
            player2.GetComponent<ShipControl>().playerIndexNum = XInputDotNetPure.PlayerIndex.Two;
            player2.transform.position = new Vector3(this.transform.position.x + (playerSpacing * 2), this.transform.position.y, this.transform.position.z);
        }
        if (p3active)
        {
            player3 = Instantiate(playerPrefab);
            player3.GetComponent<ShipControl>().playerIndexNum = XInputDotNetPure.PlayerIndex.Three;
            player3.transform.position = new Vector3(this.transform.position.x + (playerSpacing * 3), this.transform.position.y, this.transform.position.z);
        }
        if (p4active)
        {
            player4 = Instantiate(playerPrefab);
            player4.GetComponent<ShipControl>().playerIndexNum = XInputDotNetPure.PlayerIndex.Four;
            player4.transform.position = new Vector3(this.transform.position.x + (playerSpacing * 4), this.transform.position.y, this.transform.position.z);
        }

    }
	
	// Update is called once per frame
	void Update () {
        if (player1 == null)
        {
            p1active = false;
        }
        if (player2 == null)
        {
            p2active = false;
        }
        if (player3 == null)
        {
            p3active = false;
        }
        if (player4 == null)
        {
            p4active = false;
        }

        if((!(p1active || p2active || p3active || p4active))&&(!gameOver))
        {
            //Game Over
            gameOver = true;
            gameOverTime = Time.time + gameOverDelay;
            //Application.LoadLevel("CharacterSelect");
        }

        if (gameOver && (Time.time > gameOverTime))
        {

            Application.LoadLevel("CharacterSelect");


        }
    }
}
