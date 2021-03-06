using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destructBuild : MonoBehaviour {

	public KeyCode startFTest = KeyCode.Space;

	public getFracPieces getFracPieces;
	int x;

	//oncollision + overlapbox for explo
	//raycast for bullet (?)
	[System.Serializable] 
	public class OBox { 
		public LayerMask layerMask;//destruct
		public LayerMask layerMaskFrac;//chunks
		public Vector3 size = new Vector3(3,3,3);
		public Vector3 offset = new Vector3(0,0,0);
		public float radius = 2.0f;
	}
	public OBox oBox = new OBox();//overlap sphere


	public Collider[] hitColl;
	public string dbuildtag = "dbuild";//destructible wall tag [for collision]
	public string chunkstag = "chunks";//chunks tag [for collision]

	public Transform[] hitCollix;

	int hix;
	public Collider[] hitCollFrac;
	Transform fracx;

	public bool addExplo = true;
	public float exploForce = 200;

	public bool doDebug;
	int i,ix;

	bool explod;
	bool destruct = true;

	[System.Serializable] 
	public class DLAY { 

		public float T = 1;//1sec timeout when oncollisionenter
		public float startT;
	}
	public DLAY timeOut = new DLAY();//overlap sphere



	void OnCollisionEnter(Collision collision){

		if (Time.time - timeOut.startT > timeOut.T) {
			if (collision.transform.tag == dbuildtag) destruct = true;
			if (collision.transform.tag == chunkstag && addExplo) getExplo ();

			timeOut.startT = Time.time;
		}
	}

	void FixedUpdate () {

		//if (Input.GetKeyDown (startFTest)) destruct = true;

		if (destruct) {
			destruct = false;
			hitColl = new Collider[0];

			hitCollFrac = new Collider[0];

			if(doDebug) print ("fracturing");
			//find destructibles
			//hitColl = Physics.OverlapSphere (transform.TransformPoint (oBox.offset), oBox.radius, oBox.layerMask);
			hitColl = Physics.OverlapBox (transform.TransformPoint (oBox.offset), oBox.size, Quaternion.identity, oBox.layerMask);

			if (doDebug) {
				for (i = 0; i < hitColl.Length; i++) print ("o-box detect " + hitColl [i]);
				print ("coll N: " + hitColl.Length);
			}

			if (hitColl.Length == 0) return;

			if (!explod) {
				explod = true;

				//find destructible objects and their fractured objects
				for (i = 0; i < hitColl.Length; i++) {

					if (doDebug) print ("hitColl i" + i +  " " + hitColl [i].transform.name + " chcount " + hitColl [i].transform.childCount);
						
					hix = hitColl [i].transform.childCount;
					hitCollix = new Transform[hix];

					//have to create array first because we are unparenting (changing array) later
					for (ix = 0; ix < hix; ix++) {

						if (doDebug) print ("hitColl " + ix + " " + hitColl [i].transform.GetChild (ix));
						hitCollix [ix] = hitColl [i].transform.GetChild (ix).transform;

					}

					//find the frac roots
					for (ix = 0; ix < hix; ix++) {
						
						if (hitCollix [ix].name == "frac") {

							if (!getFracPieces.useWindowsDestroy) {
								fracx = hitCollix [ix];
								fracx.parent = null;
								fracx.gameObject.SetActive (true);
							}

							if (getFracPieces.useWindowsDestroy) {
								fracx = hitCollix [ix];
								fracx.parent = getFracPieces.wallChunks;
								fracx.gameObject.SetActive (true);
							}

						} else {
							//windows etc
							fracx = hitCollix [ix].Find ("frac");
							if (fracx) {

								if (!getFracPieces.useWindowsDestroy) {
									fracx.transform.parent = null;
									//fracx.transform.parent = getFracPieces.wallChunks;
									fracx.transform.gameObject.SetActive (true);
								}

								if (getFracPieces.useWindowsDestroy) {
									fracx.transform.parent = getFracPieces.windowsChunks;
									fracx.transform.gameObject.SetActive (true);
								}
							}
						}

					}

					hitColl [i].gameObject.SetActive (false);

				}
				//add explo force to fractured
				getExplo ();
			}

		}
	}

	void getExplo(){

		if(doDebug) print ("fracture get explo");

		//new overlapshere to define affected chunks
		hitCollFrac = Physics.OverlapSphere (transform.TransformPoint (oBox.offset), oBox.radius, oBox.layerMaskFrac);
		//hitCollFrac = Physics.OverlapBox (transform.TransformPoint (oBox.offset), oBox.size, Quaternion.identity, oBox.layerMaskFrac);

		if(doDebug) print ("fracture N: " + hitCollFrac.Length);

		//add explo
		for (i = 0; i < hitCollFrac.Length; i++) {

			if (!getFracPieces.useWindowsDestroy) {
				Rigidbody rb = hitCollFrac [i].transform.GetComponent<Rigidbody> ();
				rb.isKinematic = false;
				rb.useGravity = true;
				if (addExplo)
				{
					Debug.Log("111111");
					rb.AddExplosionForce (exploForce, transform.position, oBox.radius);

				}
					

				//add to shedule list meshcombine
				hitCollFrac [i].transform.parent = getFracPieces.clone;
				getFracPieces.chunkR.Add (rb);
			}

			if (getFracPieces.useWindowsDestroy) {
				
				Rigidbody rb = hitCollFrac [i].transform.GetComponent<Rigidbody> ();
				//SELECT WALL
				if (rb.transform.parent.parent == getFracPieces.wallChunks ||
					rb.transform.parent.parent.parent == getFracPieces.wallChunks) {//eg.balcony
					rb.isKinematic = false;
					rb.useGravity = true;
					if (addExplo)
					{
						Debug.Log("111111");
						rb.AddExplosionForce (exploForce, transform.position, oBox.radius);
					}


					//add to shedule list meshcombine
					hitCollFrac [i].transform.parent = getFracPieces.clone;
					getFracPieces.chunkR.Add (rb);
				} 
				//SELECT WINDOWS not wall: windows doors etc
				if (rb.transform.parent.parent.parent == getFracPieces.windowsChunks) {

					rb.transform.gameObject.SetActive(false);
				}


			}

		}

		getFracPieces.cstartT = Time.time;

		explod = false;
	}

} 