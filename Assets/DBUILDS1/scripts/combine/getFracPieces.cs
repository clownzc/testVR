using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getFracPieces : MonoBehaviour {

	public bool isOn = true;

	public Transform chunksRoot;
	public Transform wallChunks;
	public bool useWindowsDestroy = true;//destroy windows/doors chunks when fractured
	public Transform windowsChunks;

	public Transform empty;
	public Transform clone;//first all chunks get here
	public int cx;

	public Transform cloneF;//then chunks that don't move get here and get combined

	//container for fractured pieces
	//waitin to be combined

	public int x, xx;

	public List<Rigidbody> chunkR = new List<Rigidbody>();
	//public List<float> startT = new List<float>();

	public List<Transform> combinedT = new List<Transform>();

	[Space(10)]
	public float checkF = 2.0f;//check every 1sec
	public float cstartT;

	void Update () {

		if (isOn) {
			
			if (cstartT != 0 && Time.time - cstartT > checkF) {
				cstartT = Time.time;

				//clone = Instantiate(empty);//instantiated in destructBuild
				//clone.parent = chunksRoot;


				for (xx = 0; xx < chunkR.Count; xx++) {
						
					if (chunkR [xx].transform.position.y < 1.0f && chunkR [xx].velocity.magnitude < 0.1f) {

						chunkR [xx].useGravity = false;
						chunkR [xx].transform.parent = cloneF;//move to other folder
						chunkR.Remove(chunkR[xx]);
						//chunkR [xx].transform.gameObject.isStatic = true;
					}
				}
		
				if (cloneF.childCount != 0) {
					//add meshcombine script and do combine
					cloneF.gameObject.AddComponent<MeshCombiner> ().doCombine ();
					//isOn = false;
					combinedT.Add (cloneF);

					cloneF = Instantiate (empty);
					cloneF.position = chunksRoot.position;
					cloneF.parent = chunksRoot;
					cx += 1;
				}
			}
		}

	}


}