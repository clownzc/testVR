using UnityEngine;
using UnityEditor;

namespace ScoredProductions.PGDC
{

	[CustomEditor(typeof(GenerateCharacter))]
	public class GeneratorCharacterEditor :Editor
	{

		public override void OnInspectorGUI() {
			//List<SerializedObject> ExcludedProperties = new List<SerializedObject>();

			DrawDefaultInspector();

			GenerateCharacter script = (GenerateCharacter)target;

			if (!script.Generated() && GUILayout.Button("Build Character")) {
				script.CreateSelfStructure();
			}

			if (GUILayout.Button("Save Avatar File")) {
				script.SaveAvatar();
			}
		}
	}
}