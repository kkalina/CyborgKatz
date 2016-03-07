using UnityEngine;
using System.Collections;
using DG.Tweening;


public class Wiggle : MonoBehaviour {

    Rigidbody rigid;

	// Use this for initialization
	void Start () {
        rigid = gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {

    }

    void OnCollisionEnter(Collision coll) {
        transform.DOScale(rigid.velocity.normalized + new Vector3(.5f, .5f, .5f), .1f);
        StartCoroutine(revert());
    }
    void OnCollisionExit(Collision coll)
    {
        
    }
    IEnumerator revert() {
        yield return new WaitForSeconds(.1f);
        transform.DOScale(new Vector3(1f,1f,1f), .1f);

    }
}
