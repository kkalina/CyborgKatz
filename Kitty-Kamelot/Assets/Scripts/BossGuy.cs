using UnityEngine;
using System.Collections;

public class BossGuy : MonoBehaviour {

    public GameObject weakPoint1;
    public GameObject weakPoint2;
    public GameObject weakPoint3;

    public GameObject missilePrefab;
    public Transform missileEmitter;

    // Use this for initialization
    void Start () {
        missileEmitter = transform.Find("Missile Emitter").gameObject.transform;
        //Instantiate(missilePrefab, missileEmitter.position, Quaternion.identity);
        StartCoroutine(MissileSwarm());


    }

    // Update is called once per frame
    void Update () {
    }

    IEnumerator MissileSwarm() {
        yield return new WaitForSeconds(10f);
        Instantiate(missilePrefab, missileEmitter.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        Instantiate(missilePrefab, missileEmitter.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        Instantiate(missilePrefab, missileEmitter.position, Quaternion.identity);
        yield return new WaitForSeconds(10f);
        StartCoroutine(MissileSwarm());

    }
}
