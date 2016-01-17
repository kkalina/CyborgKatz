using UnityEngine;
using System.Collections;

public class woodCrash : MonoBehaviour {
    public AudioSource clip;

	// Use this for initialization
	void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "player")
        {
            clip.Play();
        }
    }
}
