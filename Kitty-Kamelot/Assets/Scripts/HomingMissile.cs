using UnityEngine;
using System.Collections;

public class HomingMissile : MonoBehaviour 
{
	public Transform target;
	private Rigidbody rb;
	private RaycastHit missileHit;
    public float missileSpeed = 40f;
	public float slowMoRange = 3f;
	public float detonationRange = 2f;
    public GameObject explosion;

	public float timeLeft = 5f;
	private bool isDead = false;

	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody>();
        //explosion = transform.Find("Explosion").gameObject;
        GameObject playerSpawner = GameObject.Find("PlayerSpawner");
        Transform chosenPlayer = this.transform;
        float bestRoll = 0;
        if (playerSpawner.GetComponent<playerSpawner>().p1active)
        {
            float roll = Random.value;
            if (roll > bestRoll){
                bestRoll = roll;
                chosenPlayer = playerSpawner.GetComponent<playerSpawner>().player1.transform;
            }
        }
        if (playerSpawner.GetComponent<playerSpawner>().p2active)
        {
            float roll = Random.value;
            if (roll > bestRoll)
            {
                bestRoll = roll;
                chosenPlayer = playerSpawner.GetComponent<playerSpawner>().player2.transform;
            }
        }
        if (playerSpawner.GetComponent<playerSpawner>().p3active)
        {
            float roll = Random.value;
            if (roll > bestRoll)
            {
                bestRoll = roll;
                chosenPlayer = playerSpawner.GetComponent<playerSpawner>().player3.transform;
            }
        }
        if (playerSpawner.GetComponent<playerSpawner>().p4active)
        {
            float roll = Random.value;
            if (roll > bestRoll)
            {
                bestRoll = roll;
                chosenPlayer = playerSpawner.GetComponent<playerSpawner>().player4.transform;
            }
        }
        target = chosenPlayer;
        //rb.AddRelativeForce(Vector3.forward * 10f, ForceMode.Impulse);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft > 0)
        {
            transform.LookAt(target);
            rb.AddRelativeForce(Vector3.forward * missileSpeed, ForceMode.Force);
        }
        else if (timeLeft <= 0 && isDead == false)
        {
            rb.AddRelativeForce(Vector3.forward * 5, ForceMode.Impulse);
            rb.useGravity = true;
            //explosion.GetComponent<ParticleSystem>().Play();
            //Destroy(this.gameObject, 2f);
            //isDead = true;
            GameObject explosionInst = Instantiate(explosion);
            explosionInst.transform.position = this.transform.position;
            Destroy(this.gameObject);
        }

        if (Physics.Raycast(transform.position + transform.forward, transform.forward, out missileHit, detonationRange)
            && isDead == false)
        {
            if (missileHit.collider.gameObject.tag == "Player")
            {
                /*
                explosion.GetComponent<ParticleSystem>().Play();
              
                Destroy(this.gameObject, .8f);
                isDead = true;
                missileHit.collider.gameObject.GetComponent<Rigidbody>().AddExplosionForce(
                    100f, transform.position, 10f, 1f, ForceMode.Impulse);
                this.gameObject.GetComponent<Collider>().enabled = false;
                this.gameObject.GetComponent<MeshRenderer>().enabled = false;
                */

                missileHit.collider.gameObject.GetComponent<ShipControl>().health -= 1;
                GameObject explosionInst = Instantiate(explosion);
                explosionInst.transform.position = this.transform.position;
                Destroy(this.gameObject);
            }

        }
        if (Physics.Raycast(transform.position + transform.forward, transform.forward, out missileHit, slowMoRange) &&
            !isDead)
        {
            if (missileHit.collider.gameObject.tag == "Player")
            {
                missileHit.collider.gameObject.GetComponent<ShipControl>().StartCoroutine("KittyTime");
            }
        }
    }

	void OnDestroy()
	{
        //explosion code here
	}

	void OnCollisonEnter(Collision collision)
	{
		Debug.Log ("foo");
		Destroy (this.gameObject, 1f);
	}
}
