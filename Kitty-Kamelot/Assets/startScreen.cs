using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class startScreen : MonoBehaviour {

    private GamePadState state;
    //public PlayerIndex index;
    public bool proceed = false;
    public float moveSpeed = 1f;

    public GameObject GO1;
    //public GameObject GO2;
    //public GameObject GO3;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (!proceed)
        {
            state = GamePad.GetState(XInputDotNetPure.PlayerIndex.One);
            if (state.Buttons.Start == ButtonState.Pressed)
            {
                proceed = true;
                GO1.SetActive(true);
                //GO2.SetActive(true);
                //GO3.SetActive(true);
            }
        }
        else
        {
            this.transform.position = new Vector3(this.transform.position.x,this.transform.position.y+(moveSpeed*Time.deltaTime),this.transform.position.z);
        }
        
    }
}
