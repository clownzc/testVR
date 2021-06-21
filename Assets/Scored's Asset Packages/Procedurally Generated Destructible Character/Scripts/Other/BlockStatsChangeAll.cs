using UnityEngine;

namespace ScoredProductions.PGDC
{

	public class BlockStatsChangeAll :MonoBehaviour
	{ // Code to change all the BlockStats Scripts in an object to different values

		private BlockStats[] AllRefs;

		public bool Destructible; // if it will fall apart when damaged

		[Range(0, Mathf.Infinity)]
		public float Health = 1; // Health of the object : need to make it more accessable to the gen structure

		[Range(0, Mathf.Infinity)]
		public float DestroyTime = 10; // How much time passes before the object despawns

		public bool DestroyOnSleep; // If the object is destroyed on sleep

		public void GetAll() {
			AllRefs = this.GetComponentsInChildren<BlockStats>(true);
			Debug.Log("Number of Scripts found : " + AllRefs.Length); // Confirm its found the scripts
		}

		public void ChangeAll() {
			foreach (BlockStats x in AllRefs) { // Doesnt matter what order
				x.Destructible = Destructible;
				x.Health = Health;
				x.DestroyTime = DestroyTime;
				x.DestroyOnSleep = DestroyOnSleep;
			}
		}
	}
}