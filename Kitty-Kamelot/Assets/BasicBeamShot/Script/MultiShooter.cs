using UnityEngine;
using System.Collections;
using DG.Tweening;


public class MultiShooter : MonoBehaviour {

	public GameObject Shot1;
	public GameObject Shot2;
    public GameObject Wave;
	public float Disturbance = 0;

    Vector3 bottom;
    Vector3 top;
    Vector3 original;

    bool laserActive = false;

	public int ShotType = 0;

	private GameObject NowShot;

	void Start () {
		NowShot = null;
        original = transform.localEulerAngles;
        bottom = transform.localEulerAngles + new Vector3(90f, 0f, 0f);
        top = transform.localEulerAngles - new Vector3(45f, 0f, 0f);
	}

	void Update () {
		GameObject Bullet;
		
        //create BasicBeamShot
        if (Input.GetButtonDown("Fire1"))
        {
            Bullet = Shot1;
            //Fire
            GameObject s1 = (GameObject)Instantiate(Bullet, this.transform.position, this.transform.rotation);
            s1.GetComponent<BeamParam>().SetBeamParam(this.GetComponent<BeamParam>());
            
            GameObject wav = (GameObject)Instantiate(Wave, this.transform.position, this.transform.rotation);
            wav.transform.localScale *= 0.25f;
            wav.transform.Rotate(Vector3.left, 90.0f);
            wav.GetComponent<BeamWave>().col = this.GetComponent<BeamParam>().BeamColor;
            
        }


        //create GeroBeam
        if (Input.GetButtonDown("Fire2") && laserActive == false)
        {
            GameObject wav = (GameObject)Instantiate(Wave, this.transform.position, this.transform.rotation);
            wav.transform.Rotate(Vector3.left, 90.0f);
            wav.GetComponent<BeamWave>().col = this.GetComponent<BeamParam>().BeamColor;

            Bullet = Shot2;
            //Fire
            NowShot = (GameObject)Instantiate(Bullet, this.transform.position, this.transform.rotation);
            StartCoroutine(setLaserActive(2f));
        }
            //it's Not "GetButtonDown"
        if (laserActive == true)
		{
			BeamParam bp = this.GetComponent<BeamParam>();
			if(NowShot.GetComponent<BeamParam>().bGero)
				NowShot.transform.parent = transform;

			Vector3 s = new Vector3(bp.Scale,bp.Scale,bp.Scale);

			NowShot.transform.localScale = s;
			NowShot.GetComponent<BeamParam>().SetBeamParam(bp);
		}
        if (laserActive == false)
		{
			if(NowShot != null)
			{
				NowShot.GetComponent<BeamParam>().bEnd = true;
			}
		}
	}

    IEnumerator setLaserActive(float duration) {
        laserActive = true;
        transform.localEulerAngles = bottom;
        Vector3 currentRotation = transform.localEulerAngles;
        //currentRotation.x = Mathf.LerpAngle(currentRotation.x, top.x, 2f * Time.fixedDeltaTime);
        transform.DORotate(top, 2f, RotateMode.LocalAxisAdd);
        yield return new WaitForSeconds(duration);
        transform.localEulerAngles = original;
        laserActive = false;
    }
}
