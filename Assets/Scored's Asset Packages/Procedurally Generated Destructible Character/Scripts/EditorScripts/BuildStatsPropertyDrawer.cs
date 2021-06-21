using UnityEngine;
using UnityEditor;

namespace ScoredProductions.PGDC
{

	[CustomPropertyDrawer(typeof(BuildStats))]
	public class BuildStatsPropertyDrawer :PropertyDrawer
	{

		private Rect[] Fields = new Rect[14];
		private SerializedProperty PieceType, FillCore, Destructible, BuildPiece, PrefabSize, Height,
		Debth, BuildType, CircleX, SquareX, DestroyOnSleep, FillHealth, DestroyTime, CustomFill,
		Offset;

		private bool Check;

		public override void OnGUI(Rect position, SerializedProperty Property, GUIContent lable) {

			int Space = 20; // Gap between each rectangle 
			int Yadd = Space; // Total distance down the rectangle

			EditorGUI.BeginProperty(position, lable, Property);

			// assign the values
			PieceType = Property.FindPropertyRelative("PieceType");
			FillCore = Property.FindPropertyRelative("FillCore");
			Destructible = Property.FindPropertyRelative("Destructible");
			BuildPiece = Property.FindPropertyRelative("BuildPiece");
			PrefabSize = Property.FindPropertyRelative("PrefabSize");
			Height = Property.FindPropertyRelative("Height");
			Debth = Property.FindPropertyRelative("Debth");
			BuildType = Property.FindPropertyRelative("BuildType");
			CircleX = Property.FindPropertyRelative("CircleX");
			SquareX = Property.FindPropertyRelative("SquareX");
			DestroyOnSleep = Property.FindPropertyRelative("DestroyOnSleep");
			FillHealth = Property.FindPropertyRelative("FillHealth");
			DestroyTime = Property.FindPropertyRelative("DestroyTime");
			CustomFill = Property.FindPropertyRelative("CustomFill");
			Offset = Property.FindPropertyRelative("Offset");

			Fields[0] = new Rect(position.x, position.y, position.width / 2, 17);
			EditorGUI.PropertyField(Fields[0], PieceType);

			Fields[1] = new Rect(position.x, position.y + Yadd, position.width / 2, 17);
			EditorGUI.PropertyField(Fields[1], FillCore);

			if (FillCore.boolValue) {
				Check = true;
				Fields[2] = new Rect(position.x + (position.width / 2), position.y + Yadd, position.width, 17);
				Yadd += Space;
				Fields[3] = new Rect(position.x, position.y + Yadd, position.width / 2, 17); // Fixed the fixed value problem
				Fields[4] = new Rect(position.x + (position.width / 2), position.y + Yadd, position.width / 2, 17); // Fixed the fixed value problem
				Yadd += Space;
				Fields[5] = new Rect(position.x, position.y + Yadd, position.width, 17);
				EditorGUI.PropertyField(Fields[2], Destructible);
				EditorGUI.PropertyField(Fields[3], FillHealth);
				EditorGUI.PropertyField(Fields[5], DestroyOnSleep);
				if (!DestroyOnSleep.boolValue) {
					EditorGUI.PropertyField(Fields[4], DestroyTime);
				}
				Yadd += Space;
				Fields[6] = new Rect(position.x, position.y + Yadd, position.width, 17);
				EditorGUI.PropertyField(Fields[6], CustomFill);

			} else {
				Check = false;
			}

			Yadd += Space;
			Fields[7] = new Rect(position.x, position.y + Yadd, position.width, 17);
			EditorGUI.PropertyField(Fields[7], BuildPiece);

			Yadd += Space;
			Fields[8] = new Rect(position.x, position.y + Yadd, position.width / 2, 17);
			EditorGUI.PropertyField(Fields[8], PrefabSize);

			Yadd += Space;
			Fields[9] = new Rect(position.x, position.y + Yadd, position.width / 2, 17);
			EditorGUI.PropertyField(Fields[9], Height);

			Yadd += Space;
			Fields[10] = new Rect(position.x, position.y + Yadd, position.width / 2, 17);
			EditorGUI.PropertyField(Fields[10], Debth);

			Yadd += Space;
			Fields[11] = new Rect(position.x, position.y + Yadd, position.width / 2, 17);
			EditorGUI.PropertyField(Fields[11], BuildType);

			Yadd += Space;

			if (BuildType.intValue == 0) {
				Fields[12] = new Rect(position.x, position.y + Yadd, position.width, 17);
				EditorGUI.PropertyField(Fields[12], SquareX);

			} else if (BuildType.intValue == 1) {
				Fields[12] = new Rect(position.x, position.y + Yadd, position.width / 2, 17);
				EditorGUI.PropertyField(Fields[12], CircleX);
				Fields[13] = new Rect(position.x + (position.width / 2), position.y + Yadd, position.width / 2, 17);
				EditorGUI.PropertyField(Fields[13], Offset);
			}

			if (PieceType.intValue < 0) {
				PieceType.intValue = 0;
			}

			if (PrefabSize.floatValue < 0) {
				PrefabSize.floatValue = 0;
			}

			if (Height.intValue < 0) {
				Height.intValue = 0;
			}

			if (Debth.intValue < 0) {
				Debth.intValue = 0;
			}

			if (BuildType.intValue < 0) {
				BuildType.intValue = 0;
			}

			if (CircleX.intValue < 0) {
				CircleX.intValue = 0;
			}

			if (FillHealth.floatValue < 0) {
				FillHealth.floatValue = 0;
			}

			if (DestroyTime.floatValue < 0) {
				DestroyTime.floatValue = 0;
			}

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			int AreaHeight;

			if (Check) {
				AreaHeight = 220;

			} else {
				AreaHeight = 160;
			}

			return AreaHeight;
		}
	}
}