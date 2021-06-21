using UnityEngine;
using UnityEditor;

namespace ScoredProductions.PGDC
{

	[CustomEditor(typeof(BlockStatsChangeAll))]
	public class BlockStatsChangeAllEditor :Editor
	{

		public override void OnInspectorGUI() {
			DrawDefaultInspector();

			BlockStatsChangeAll myScript = (BlockStatsChangeAll)target;

			if (GUILayout.Button("Get Scripts")) {
				myScript.GetAll();
			}

			if (GUILayout.Button("Change All")) {
				myScript.ChangeAll();
			}
		}
	}
}