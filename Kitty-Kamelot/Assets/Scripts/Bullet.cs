using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 180f, ForceMode.Impulse);
        Destroy(this.gameObject, 2f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
