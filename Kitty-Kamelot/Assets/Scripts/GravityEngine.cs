using UnityEngine;
using System.Collections;

public class GravityEngine : MonoBehaviour {

    public Vector3 direction;
    public float gravity = -9.8f;
    float oldGrav;
    Rigidbody rigid;



    // Use this for initialization
    void Start () {
        oldGrav = gravity;
        direction = Vector3.up;
        rigid = this.gameObject.GetComponent<Rigidbody>();
        rigid.useGravity = false;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        //rigid.velocity += direction * gravity * Time.fixedDeltaTime;
    }

    public void ApplyGravity()
    {
        rigid.velocity += direction * gravity * Time.fixedDeltaTime;
    }

    public IEnumerator tempGravChange(Vector3 dir, float duration, float magnitude)
    {
        direction = dir;
        gravity = magnitude;
        yield return new WaitForSeconds(duration);
        gravity = oldGrav;
        direction = Vector3.up;
    }
}
