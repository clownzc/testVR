using UnityEngine;
using UnityEditor;

namespace ScoredProductions.PGDC
{

	[CustomEditor(typeof(GenerateDestructiblePiece))]
	public class GenerateDestructiblePieceEditor :Editor
	{

		public override void OnInspectorGUI() {
			DrawDefaultInspector();

			GenerateDestructiblePiece myScript = (GenerateDestructiblePiece)target;

			if (GUILayout.Button("Build")) {
				myScript.GenerateStructure();
			}

			if (GUILayout.Button("Clear Blocks")) {
				myScript.ClearBlocks();
			}
		}
	}
}