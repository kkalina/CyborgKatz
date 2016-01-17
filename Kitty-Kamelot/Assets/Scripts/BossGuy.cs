using UnityEngine;
using System.Collections;

public class BossGuy : MonoBehaviour {

    public GameObject weakPoint1;
    public GameObject weakPoint2;
    public GameObject weakPoint3;

    public GameObject missilePrefab;
    public Transform missileEmitter;
    public ParticleSystem dieExplode;

    public int bossHealth = 3;

    // Use this for initialization
    void Start () {
        missileEmitter = transform.Find("Missile Emitter").gameObject.transform;
        //dieExplode = transform.Find("Big Bang").gameObject.GetComponent<ParticleSystem>();
        Instantiate(missilePrefab, missileEmitter.position, Quaternion.identity);
        StartCoroutine(MissileSwarm());


    }

    // Update is called once per frame
    void Update () {
        if (bossHealth <= 0) {
            dieExplode.Play();
            Destroy(this.gameObject, 3f);
        }
    }


    IEnumerator MissileSwarm() {
        yield return new WaitForSeconds(1f);
        Instantiate(missilePrefab, missileEmitter.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        Instantiate(missilePrefab, missileEmitter.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        Instantiate(missilePrefab, missileEmitter.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        StartCoroutine(MissileSwarm());

    }
}
