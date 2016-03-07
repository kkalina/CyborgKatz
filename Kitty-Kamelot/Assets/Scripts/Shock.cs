using UnityEngine;
using System.Collections;

public class Shock : MonoBehaviour {
	[Header("Settings")]
	public float		length = 0.5f;
	public float		maxForce = 1;
	public float		restitutionForce = 1f;
	[Header("Calculated")]
	public Vector3		collisionPoint;
	public Vector3		surfaceNormal;
	public RaycastHit	hit;
	public float		forceMultiplier;
	public bool			grounded = false;
	ParticleSystem		ps;

	Rigidbody		rigid; // The Rigidbody of the parent

	void Awake() {
		rigid = transform.parent.GetComponent<Rigidbody>();
		ps = GetComponent<ParticleSystem>();
	}

	void FixedUpdate() {
		// Raycast to find ground
		grounded = Physics.Raycast(transform.position, -transform.up, out hit, length);
		if (!grounded) {
			Debug.DrawRay(transform.position, -transform.up*length);
			ps.enableEmission = false;
			return;
		}

		ps.enableEmission = true;
        
		// Only continue if there was a hit
		forceMultiplier = 1 - (hit.distance / length);
		rigid.AddForceAtPosition(transform.up*forceMultiplier*restitutionForce, transform.position);
		Debug.DrawRay(transform.position, -transform.up*hit.distance);
	}
}
