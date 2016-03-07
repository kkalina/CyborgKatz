using UnityEngine;
using System.Collections;
using DG.Tweening;


public class Wiggle : MonoBehaviour {

	// Use this for initialization
	void Start () {
        transform.DOScaleY(.5f, 5f);
	}
	
	// Update is called once per frame
	void Update () {
        transform.DOScaleY(.5f, 10f);

    }
}
