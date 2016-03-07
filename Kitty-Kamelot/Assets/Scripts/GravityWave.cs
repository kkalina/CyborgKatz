using UnityEngine;
using System.Collections;
using DG.Tweening;


public class GravityWave : MonoBehaviour {

    public float magnitude = -9.8f;
    public Vector3 direction = Vector3.forward;
    public float duration = 3f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        StartCoroutine(SetSubjectiveGravity(other.GetComponent<Rigidbody>()));
    }

    IEnumerator SetSubjectiveGravity(Rigidbody target)
    {
        target.useGravity = false;
        Vector3 oldDir = target.GetComponent<GravityEngine>().direction;
        target.GetComponent<GravityEngine>().direction = -this.gameObject.transform.up;
        target.GetComponent<GravityEngine>().gravity = magnitude;
        yield return new WaitForSeconds(duration);
        target.GetComponent<GravityEngine>().direction = oldDir;
    }
}
