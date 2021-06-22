using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switchObj : MonoBehaviour {

	//just drag the piece [obj0[x]] you want ALL to change
	//to the prefab [obj1[x]]
	public Transform[] obj0;
	public Transform[] obj1;
	int x;
	public Transform buildRoot;

	Transform[] o1;
	Transform[] o2;
	Transform[] o3;


	int i;
	int ix, ix2, ix3;
	int ixx;
	Transform objO;
	Transform objX;

	void Start () {

		for (x = 0; x < obj0.Length; x++) {



			for (i = 0; i < buildRoot.childCount; i++) {

				objO = buildRoot.GetChild (i).transform;
				o1 = new Transform[objO.childCount];

				for (ix = 0; ix < objO.childCount; ix++) {

					o1 [ix] = objO.GetChild (ix).transform;

					if (o1 [ix].name.Equals (obj0 [x].name) || o1 [ix].name.StartsWith (obj0 [x].name+ " (")) {

						objX = Instantiate (obj1 [x]);
						objX.position = o1 [ix].position;
						objX.rotation = o1 [ix].rotation;
						Destroy (o1 [ix].gameObject);
					} else {

						if (o1 [ix].childCount > 0) {

							o2 = new Transform[o1 [ix].childCount];

							for (ix2 = 0; ix2 < o2.Length; ix2++) {

								o2 [ix2] = o1 [ix].GetChild (ix2).transform;

								if (o2 [ix2].name.Equals (obj0 [x].name) || o2 [ix2].name.StartsWith (obj0 [x].name+ " (")) {

									objX = Instantiate (obj1 [x]);
									objX.position = o2 [ix2].position;
									objX.rotation = o2 [ix2].rotation;
									Destroy (o2 [ix2].gameObject);

								} else {

									if (o2 [ix2].childCount > 0) {
									
										o3 = new Transform[o2 [ix2].childCount];

										for (ix3 = 0; ix3 < o3.Length; ix3++) {

											o3 [ix3] = o2 [ix2].GetChild (ix3).transform;


											if (o3 [ix3].name.Equals (obj0 [x].name) || o3 [ix3].name.StartsWith (obj0 [x].name+ " (")) {

												objX = Instantiate (obj1 [x]);
												objX.position = o3 [ix3].position;
												objX.rotation = o3 [ix3].rotation;
												Destroy (o3 [ix3].gameObject);

											}


										}
									}
								} 


							}
						}

					}
				}

			}
		}
	
	}

}