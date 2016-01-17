using UnityEngine;
using System.Collections;

public class BossWeakpoint : MonoBehaviour {

    public GameObject Boss;

	// Use this for initialization
	void Start () {
        Boss = GameObject.Find("Boss");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //transform.RotateAround(Boss.transform.position, Vector3.up, 20 * Time.deltaTime);
    }
}
