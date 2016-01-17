using UnityEngine;
using System.Collections;

public class playerSpawner : MonoBehaviour {
    public bool p1active = false;
    public bool p2active = false;
    public bool p3active = false;
    public bool p4active = false;

    public GameObject playerPrefab;

    public float playerSpacing = 2f;

    // Use this for initialization
    void Start () {

        p1active = playerSelectionManager.p1Selected;
        p2active = playerSelectionManager.p2Selected;
        p3active = playerSelectionManager.p3Selected;
        p4active = playerSelectionManager.p4Selected;

        if (p1active)
        {
            GameObject player1 = Instantiate(playerPrefab);
            player1.GetComponent<ShipControl>().playerIndexNum = XInputDotNetPure.PlayerIndex.One;
            player1.transform.position = new Vector3(this.transform.position.x + (playerSpacing*1),this.transform.position.y,this.transform.position.z);
        }
        if (p2active)
        {
            GameObject player2 = Instantiate(playerPrefab);
            player2.GetComponent<ShipControl>().playerIndexNum = XInputDotNetPure.PlayerIndex.Two;
            player2.transform.position = new Vector3(this.transform.position.x + (playerSpacing * 2), this.transform.position.y, this.transform.position.z);
        }
        if (p3active)
        {
            GameObject player3 = Instantiate(playerPrefab);
            player3.GetComponent<ShipControl>().playerIndexNum = XInputDotNetPure.PlayerIndex.Three;
            player3.transform.position = new Vector3(this.transform.position.x + (playerSpacing * 3), this.transform.position.y, this.transform.position.z);
        }
        if (p4active)
        {
            GameObject player4 = Instantiate(playerPrefab);
            player4.GetComponent<ShipControl>().playerIndexNum = XInputDotNetPure.PlayerIndex.Four;
            player4.transform.position = new Vector3(this.transform.position.x + (playerSpacing * 4), this.transform.position.y, this.transform.position.z);
        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
