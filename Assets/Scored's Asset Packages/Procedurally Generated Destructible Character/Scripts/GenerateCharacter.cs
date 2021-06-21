using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ScoredProductions.PGDC
{

	[System.Serializable]
	public struct Construct
	{
		public string Name; // Cosmetic
		public GameObject prefab;
		public bool Generate;
		public BuildStats Stats;
	}

	[System.Serializable]
	public struct PrefabConstruct
	{
		public string Name; // Cosmetic
		public GameObject prefab;
		public float Scale;
	}

	[System.Serializable]
	public struct Template
	{
		public Construct[] Limbs;
		public Construct[] Torso;
		public PrefabConstruct[] Joints;
		public PrefabConstruct[] Hands;
		public PrefabConstruct[] Feet;
		public PrefabConstruct Head;
	}

	/// <Index of peices / Body structure>
	/// (children indexed in order named)
	/// Limbs: [8] (child : "ConnectionPoint")
	/// 0 - Left upper arm
	/// 1 - Left lower arm
	/// 2 - Right upper arm
	/// 3 - Right lower arm
	/// 4 - Left upper leg
	/// 5 - Left lower leg
	/// 6 - Right upper leg
	/// 7 - Right lower leg
	/// 
	/// Torso: [2] 
	/// 0 - Lower Body Piece (children : "LeftConnectionPoint" , "RightConnectionPoint" , "HipPosition")
	/// 1 - Upper Body Piece (children : "LeftConnectionPoint" , "RightConnectionPoint" , "BottomConnectionPoint")
	/// 
	/// Joints: [8] (children : "ConnectionPoint" , "JointCenter")
	/// 0 - Left shoulder
	/// 1 - Left elbow
	/// 2 - Right shoulder
	/// 3 - Right elbow
	/// 4 - Left Hip connector
	/// 5 - Left knee
	/// 6 - Right Hip connector
	/// 7 - Right knee
	/// 
	/// Hands: [2] (child : "HandCenter")
	/// 0 - Left
	/// 1 - Right
	/// 
	/// Feet: [2] (children : "FootCenter" , "ToeEnd")
	///	0 - Left
	/// 1 - Right
	/// 
	/// Head: (children : "Camera" , "HeadCenter")
	/// - self explanitory
	/// 
	/// 
	/// Avatar Structure: (More points between these points increase accuracy)
	/// 
	/// Hip - Spine - Chest - Neck - Head
	/// Chest - Shoulder - Upper Arm - Lower Arm - Hand
	/// Hip - Upper Leg - Lower Leg - Feet - Toes
	/// </Index of peices>

	[UnityEngine.ExecuteInEditMode]
	public class GenerateCharacter :MonoBehaviour
	{

		private Template RecoveryBody; // body used when a reset is required

		public GameObject AvatarBodyBlock; // Block used to generate the Avatar Mesh
		public bool UseDefaults; // Will override current settings to generate the basic character 
		public Template Body; // Structure for the whole body
		public int TotalGenBlocks; // Count of all the blocks generated in the limbs // TODO : Not persistant (needs adding a delayed update for edit mode)

		private Vector3[] SkelPoints = new Vector3[21]; // Positions gathered from the generated components
		private GameObject[] ABodyPiece; // Pieces of the avatar mesh which will be combined into the combined mesh
		private GameObject[] AvatarTransforms; // Avatar Points, populated by skelpoints
		private GameObject AvatarCombinedMesh; // GameObject holding the mesh

		[System.NonSerialized]
		public Avatar NewAvatar; // Generated avatar component

		private GameObject CharacterMesh; // Generated mesh component


		private bool _generated = false;
		public bool Generated() { return _generated; }

		public void CreateSelfStructure() {
			ResetAndCleanUp(false);

			if (UseDefaults) {
				Body = UseDefaultData(); // Po plate the blank values with the predetermined ones
			}

			if (ValidCheck()) { // Check the length of all the arrays is correct, saves current template to recovery
				if (InitialiseBodyPieces() && // Create the pieces
					BodyHierarchy() && // Assigne the pieces to their parents (o simplify positioning)
					CheckConnectionPointsValid() && // Check all the pieces have the connection points
					TriggerLimbBuilds() && // Trigger generation for the Limbs
					BodyPositions() && // Position all the pieces now that their all generated
					PopulateSkelPoints() && // Get the positions from the body
					GenerateMesh() && // Create the avatar mesh from the information
					AvatarSetup() && // Create the avatar component
					AlignLimbsToAvatarHierarchy()) { // Change the hierarchy from the original to the avatar equivolent
					_generated = true;
				} else {
					ResetAndCleanUp(true);
				}
			}
		}

		bool ValidCheck() {
			bool checkValid = true;

			if (Body.Limbs.Length != 8) {
				Debug.LogError("Their needs to be 8 Limbs to make a complete body");
				checkValid = false;
			}

			if (Body.Joints.Length != 8) {
				Debug.LogError("Their needs to be 8 Joints to make a complete body");
				checkValid = false;
			}

			if (Body.Torso.Length != 2) {
				Debug.LogError("Their needs to be 2 Torso pieces to make a complete body");
				checkValid = false;
			}

			if (Body.Hands.Length != 2) {
				Debug.LogError("Their needs to be 2 Hands to make a complete body");
				checkValid = false;
			}

			if (Body.Feet.Length != 2) {
				Debug.LogError("Their needs to be 2 Feet to make a complete body");
				checkValid = false;
			}

			for (int x = 0; x < Body.Limbs.Length; x++) {
				if (Body.Limbs[x].prefab == null) {
					Debug.LogError(Body.Limbs[x].Name + " build prefab is missing");
					checkValid = false;
				}
			}

			for (int x = 0; x < Body.Joints.Length; x++) {
				if (Body.Joints[x].prefab == null) {
					Debug.LogError(Body.Joints[x].Name + " build prefab is missing");
					checkValid = false;
				}
			}

			for (int x = 0; x < Body.Torso.Length; x++) {
				if (Body.Torso[x].prefab == null) {
					Debug.LogError(Body.Torso[x].Name + " build prefab is missing");
					checkValid = false;
				}
			}

			for (int x = 0; x < Body.Hands.Length; x++) {
				if (Body.Hands[x].prefab == null) {
					Debug.LogError(Body.Hands[x].Name + " build prefab is missing");
					checkValid = false;
				}
			}

			for (int x = 0; x < Body.Feet.Length; x++) {
				if (Body.Feet[x].prefab == null) {
					Debug.LogError(Body.Feet[x].Name + " build prefab is missing");
					checkValid = false;
				}
			}

			if (Body.Head.prefab == null) {
				Debug.LogError(Body.Head.Name + " build prefab is missing");
				checkValid = false;
			}

			if (checkValid) {
				SaveBodyToRecovery();
			}

			return checkValid;
		}

		bool PopulateSkelPoints() {
			bool checkValid = true;
			SkelPoints = new Vector3[21];

			SkelPoints[0] = Body.Torso[0].prefab.transform.Find("HipPosition").transform.position; // Hip
			SkelPoints[1] = Body.Torso[0].prefab.transform.position; // Spine
			SkelPoints[2] = Body.Torso[1].prefab.transform.Find("ChestPosition").transform.position; // Chest
			SkelPoints[3] = Body.Torso[1].prefab.transform.position; // Neck
			SkelPoints[4] = Body.Head.prefab.transform.Find("HeadCenter").transform.position; // Head
			SkelPoints[5] = Body.Torso[1].prefab.transform.Find("LeftConnectionPoint").transform.position; // Left Shoulder
			SkelPoints[6] = Body.Joints[0].prefab.transform.Find("JointCenter").transform.position; // Left Upper Arm
			SkelPoints[7] = Body.Joints[1].prefab.transform.Find("JointCenter").transform.position; // Left Lower Arm
			SkelPoints[8] = Body.Hands[0].prefab.transform.Find("HandCenter").transform.position; // Left Hand
			SkelPoints[9] = Body.Torso[1].prefab.transform.Find("RightConnectionPoint").transform.position; // Right Shoulder
			SkelPoints[10] = Body.Joints[2].prefab.transform.Find("JointCenter").transform.position; // Right Upper Arm
			SkelPoints[11] = Body.Joints[3].prefab.transform.Find("JointCenter").transform.position; // Right Lower Arm
			SkelPoints[12] = Body.Hands[1].prefab.transform.Find("HandCenter").transform.position; // Right Hand
			SkelPoints[13] = Body.Joints[4].prefab.transform.Find("JointCenter").transform.position; // Left Upper Leg
			SkelPoints[14] = Body.Joints[5].prefab.transform.Find("JointCenter").transform.position; // Left Lower Leg // knee
			SkelPoints[15] = Body.Feet[0].prefab.transform.Find("FootCenter").transform.position; // Left foot
			SkelPoints[16] = Body.Feet[0].prefab.transform.Find("ToeEnd").transform.position; // left ToeEnd
			SkelPoints[17] = Body.Joints[6].prefab.transform.Find("JointCenter").transform.position; // Right Upper Leg
			SkelPoints[18] = Body.Joints[7].prefab.transform.Find("JointCenter").transform.position; // Right Lower LEg // knee
			SkelPoints[19] = Body.Feet[1].prefab.transform.Find("FootCenter").transform.position; // Right Foot
			SkelPoints[20] = Body.Feet[1].prefab.transform.Find("ToeEnd").transform.position; // Right Foot End

			return checkValid;
		}

		GameObject AvatarPieceBuild(Vector3 StartPoint, Vector3 EndPoint) { // create a piece of the avatar mesh
			GameObject AP = Instantiate(AvatarBodyBlock, this.transform); // Spawn the block
			AP.transform.position = Vector3.Lerp(StartPoint, EndPoint, 0.5f); // position half way between the 2 points
			AP.transform.LookAt(EndPoint); // point towrds the end point (z direction)
			Vector3 NewScale = new Vector3(1, 1, Vector3.Distance(StartPoint, EndPoint)); // Get the distance between the 2 points and put it on the Z axis
			AP.transform.localScale = NewScale;  // apply the scale to the object

			return AP;
		}

		bool InitialiseBodyPieces() { // The creation of all the peices
			bool checkValid = true;
			float scaling; // Created for prefabs scale

			for (int x = 0; x < Body.Limbs.Length; x++) {
				Body.Limbs[x].prefab = Instantiate(Body.Limbs[x].prefab.gameObject, this.transform);
				Body.Limbs[x].prefab.name = Body.Limbs[x].Name;

				if (Body.Limbs[x].Generate) {
					if (Body.Limbs[x].prefab.GetComponent<GenerateDestructiblePiece>() == null) {
						Body.Limbs[x].prefab.AddComponent<GenerateDestructiblePiece>();
					}
				}
			}

			for (int x = 0; x < Body.Joints.Length; x++) {
				Body.Joints[x].prefab = Instantiate(Body.Joints[x].prefab.gameObject, this.transform);
				Body.Joints[x].prefab.name = Body.Joints[x].Name;
				scaling = Body.Joints[x].Scale;
				Body.Joints[x].prefab.transform.localScale = Body.Joints[x].prefab.transform.localScale * scaling;
			}

			for (int x = 0; x < Body.Torso.Length; x++) {
				Body.Torso[x].prefab = Instantiate(Body.Torso[x].prefab.gameObject, this.transform);
				Body.Torso[x].prefab.name = Body.Torso[x].Name;
				if (Body.Torso[x].Generate) {
					if (Body.Torso[x].prefab.GetComponent<GenerateDestructiblePiece>() == null) {
						Body.Torso[x].prefab.AddComponent<GenerateDestructiblePiece>();
					}
				}
			}

			for (int x = 0; x < Body.Hands.Length; x++) {
				Body.Feet[x].prefab = Instantiate(Body.Feet[x].prefab.gameObject, this.transform);
				Body.Feet[x].prefab.name = Body.Feet[x].Name;
				scaling = Body.Feet[x].Scale;
				Body.Feet[x].prefab.transform.localScale = Body.Feet[x].prefab.transform.localScale * scaling;
			}

			for (int x = 0; x < Body.Feet.Length; x++) {
				Body.Hands[x].prefab = Instantiate(Body.Hands[x].prefab.gameObject, this.transform);
				Body.Hands[x].prefab.name = Body.Hands[x].Name;
				scaling = Body.Hands[x].Scale;
				Body.Hands[x].prefab.transform.localScale = Body.Hands[x].prefab.transform.localScale * scaling;
			}

			Body.Head.prefab = Instantiate(Body.Head.prefab.gameObject, this.transform);
			Body.Head.prefab.name = Body.Head.Name;
			scaling = Body.Head.Scale;
			Body.Head.prefab.transform.localScale = Body.Head.prefab.transform.localScale * scaling;

			return checkValid;
		}

		bool BodyHierarchy() { // See Summary Above for structure
			bool checkValid = true;

			Body.Torso[0].prefab.transform.parent = this.transform; // Lower Body -> base
			Body.Torso[1].prefab.transform.parent = Body.Torso[0].prefab.transform; // Upper Body -> Lower Body

			Body.Head.prefab.transform.parent = Body.Torso[1].prefab.transform; // Head -> Upper Body

			#region Joints
			Body.Joints[0].prefab.transform.parent = Body.Torso[1].prefab.transform; // Left Shoulder -> Upper Body
			Body.Joints[1].prefab.transform.parent = Body.Limbs[0].prefab.transform; // Left Elbow ->  Left Upper arm
			Body.Joints[2].prefab.transform.parent = Body.Torso[1].prefab.transform; // Right Shoulder -> Upper Body
			Body.Joints[3].prefab.transform.parent = Body.Limbs[2].prefab.transform; // Right Elbow ->  Right Upper arm
			Body.Joints[4].prefab.transform.parent = Body.Torso[0].prefab.transform; // Left Hip -> Lower Body
			Body.Joints[5].prefab.transform.parent = Body.Limbs[4].prefab.transform; // Left Knee -> Left Upper Leg
			Body.Joints[6].prefab.transform.parent = Body.Torso[0].prefab.transform; // Right Hip -> Right Body
			Body.Joints[7].prefab.transform.parent = Body.Limbs[6].prefab.transform; // Right Knee -> Right Upper Leg
			#endregion

			#region Limbs
			Body.Limbs[0].prefab.transform.parent = Body.Joints[0].prefab.transform; // Left Upper Arm -> Left Shoulder
			Body.Limbs[1].prefab.transform.parent = Body.Joints[1].prefab.transform; // Left Lower Arm -> Left Elbow
			Body.Limbs[2].prefab.transform.parent = Body.Joints[2].prefab.transform; // Right Upper Arm -> Right Shoulder
			Body.Limbs[3].prefab.transform.parent = Body.Joints[3].prefab.transform; // Right Lower Arm -> Right Elbow
			Body.Limbs[4].prefab.transform.parent = Body.Joints[4].prefab.transform; // Left Upper Leg -> Left Hip
			Body.Limbs[5].prefab.transform.parent = Body.Joints[5].prefab.transform; // Left Lower Leg -> Left Knee
			Body.Limbs[6].prefab.transform.parent = Body.Joints[6].prefab.transform; // Right Upper Leg -> Right Hip
			Body.Limbs[7].prefab.transform.parent = Body.Joints[7].prefab.transform; // Right Lower Leg -> Right Knee
			#endregion

			Body.Hands[0].prefab.transform.parent = Body.Limbs[1].prefab.transform; // Left Hand -> Left Lower Leg
			Body.Hands[1].prefab.transform.parent = Body.Limbs[3].prefab.transform; // Right Hand -> Right Lower Leg

			Body.Feet[0].prefab.transform.parent = Body.Limbs[5].prefab.transform; // Left Foot -> Left Lower Leg
			Body.Feet[1].prefab.transform.parent = Body.Limbs[7].prefab.transform; // Right Foot -> Right Lower Leg

			return checkValid;
		}

		bool BodyPositions() {
			bool checkValid = true;

			// for the components that contain the conection point in the child, the local position need to be subtracted from 0
			Body.Torso[0].prefab.transform.localPosition = Vector3.zero;
			Body.Torso[1].prefab.transform.localPosition = Vector3.zero - Body.Torso[1].prefab.transform.Find("BottomConnectionPoint").localPosition;

			Body.Head.prefab.transform.localPosition = Vector3.zero;

			Body.Joints[0].prefab.transform.localPosition = Body.Torso[1].prefab.transform.Find("LeftConnectionPoint").localPosition;
			Body.Joints[0].prefab.transform.localRotation = Quaternion.Euler(0, 0, 0); // for T pose
			Body.Joints[1].prefab.transform.localPosition = Body.Limbs[0].prefab.transform.Find("ConnectionPoint").localPosition;
			Body.Joints[2].prefab.transform.localPosition = Body.Torso[1].prefab.transform.Find("RightConnectionPoint").localPosition;
			Body.Joints[2].prefab.transform.localRotation = Quaternion.Euler(0, 0, 180); // for T pose
			Body.Joints[3].prefab.transform.localPosition = Body.Limbs[2].prefab.transform.Find("ConnectionPoint").localPosition;
			Body.Joints[4].prefab.transform.localPosition = Body.Torso[0].prefab.transform.Find("LeftConnectionPoint").localPosition;
			Body.Joints[5].prefab.transform.localPosition = Body.Limbs[4].prefab.transform.Find("ConnectionPoint").localPosition;
			Body.Joints[6].prefab.transform.localPosition = Body.Torso[0].prefab.transform.Find("RightConnectionPoint").localPosition;
			Body.Joints[7].prefab.transform.localPosition = Body.Limbs[6].prefab.transform.Find("ConnectionPoint").localPosition;

			if (Body.Joints.Length == 8 && Body.Limbs.Length == 8) { // 1 limb for 1 Joint
				for (int x = 0; x < Body.Limbs.Length; x++) {
					Body.Limbs[x].prefab.transform.localPosition = Body.Joints[x].prefab.transform.Find("ConnectionPoint").transform.localPosition;
				}

			} else {
				Debug.LogError("For this body their needs to be 8 Limbs and 8 Joints");
				checkValid = false;
			}

			Body.Hands[0].prefab.transform.localPosition = Body.Limbs[1].prefab.transform.Find("ConnectionPoint").localPosition; // Left Hand -> Left Lower Leg
			Body.Hands[1].prefab.transform.localPosition = Body.Limbs[3].prefab.transform.Find("ConnectionPoint").localPosition; // Right Hand -> Right Lower Leg

			Body.Feet[0].prefab.transform.localPosition = Body.Limbs[5].prefab.transform.Find("ConnectionPoint").localPosition; // Left Foot -> Left Lower Leg
			Body.Feet[1].prefab.transform.localPosition = Body.Limbs[7].prefab.transform.Find("ConnectionPoint").localPosition; // Right Foot -> Right Lower Leg

			return checkValid;
		}

		bool CheckConnectionPointsValid() { // Check the structure of the connection points are valid
			bool checkValid = true;

			for (int x = 0; x < Body.Limbs.Length; x++) {
				if (!Body.Limbs[x].Generate) {
					if (!Body.Limbs[x].prefab.transform.Find("ConnectionPoint")) {
						Debug.LogError(Body.Limbs[x].Name + " doesnt have a 'ConnectionPoint'");
						checkValid = false;
					}
				}
			}

			for (int x = 0; x < Body.Joints.Length; x++) {
				if (!Body.Joints[x].prefab.transform.Find("ConnectionPoint")) {
					Debug.LogError(Body.Joints[x].Name + " doesnt have a 'ConnectionPoint'");
					checkValid = false;
				}
			}

			if (!Body.Torso[0].Generate) {
				if (!Body.Torso[0].prefab.transform.Find("LeftConnectionPoint")) {
					Debug.LogError(Body.Torso[0].Name + " doesnt have a 'LeftConnectionPoint'");
					checkValid = false;

				} else if (!Body.Torso[0].prefab.transform.Find("RightConnectionPoint")) {
					Debug.LogError(Body.Torso[0].Name + " doesnt have a 'RightConnectionPoint'");
					checkValid = false;

				} else if (!Body.Torso[0].prefab.transform.Find("HipPosition")) {
					Debug.LogError(Body.Torso[0].Name + " doesnt have a 'HipPosition'");
					checkValid = false;
				}
			}
			if (!Body.Torso[1].Generate) {
				if (!Body.Torso[1].prefab.transform.Find("LeftConnectionPoint")) {
					Debug.LogError(Body.Torso[1].Name + " doesnt have a 'LeftConnectionPoint'");
					checkValid = false;

				} else if (!Body.Torso[1].prefab.transform.Find("RightConnectionPoint")) {
					Debug.LogError(Body.Torso[1].Name + " doesnt have a 'RightConnectionPoint'");
					checkValid = false;

				} else if (!Body.Torso[1].prefab.transform.Find("BottomConnectionPoint")) {
					Debug.LogError(Body.Torso[1].Name + " doesnt have a 'BottomConnectionPoint'");
					checkValid = false;

				} else if (!Body.Torso[1].prefab.transform.Find("ChestPosition")) {
					Debug.LogError(Body.Torso[0].Name + " doesnt have a 'ChestPosition'");
					checkValid = false;
				}
			}

			if (!Body.Head.prefab.transform.Find("HeadCenter")) {
				Debug.LogError(Body.Head.Name + " doesnt have a 'HeadCenter'");
				checkValid = false;
			}

			for (int x = 0; x < Body.Hands.Length; x++) {
				if (!Body.Hands[x].prefab.transform.Find("HandCenter")) {
					Debug.LogError(Body.Hands[x].Name + " doesnt have a 'HandCenter'");
					checkValid = false;
				}
			}

			for (int x = 0; x < Body.Feet.Length; x++) {
				if (!Body.Feet[x].prefab.transform.Find("FootCenter")) {
					Debug.LogError(Body.Feet[x].Name + " doesnt have a 'FootCenter'");
					checkValid = false;
				}
			}

			return checkValid;
		}

		bool TriggerLimbBuilds() { // Trigger the construction of the peices
			bool checkValid = true;

			TotalGenBlocks = 0;
			for (int x = 0; x < Body.Limbs.Length; x++) {
				if (Body.Limbs[x].Generate) {
					if (Body.Limbs[x].prefab.GetComponent<GenerateDestructiblePiece>() == null) { // TODO : test a blank 'generatedestructablepiece' script
						Body.Limbs[x].prefab.AddComponent<GenerateDestructiblePiece>();
					}

					PopulateLimbComponent(Body.Limbs[x]);
					Body.Limbs[x].prefab.GetComponent<GenerateDestructiblePiece>().GenerateStructure();
					TotalGenBlocks += Body.Limbs[x].prefab.GetComponent<GenerateDestructiblePiece>().TotalBlocks;
				}
			}
			for (int x = 0; x < Body.Torso.Length; x++) {
				if (Body.Torso[x].Generate) {
					if (Body.Torso[x].prefab.GetComponent<GenerateDestructiblePiece>() == null) {
						Body.Torso[x].prefab.AddComponent<GenerateDestructiblePiece>();
					}

					PopulateLimbComponent(Body.Torso[x]);
					Body.Torso[x].prefab.GetComponent<GenerateDestructiblePiece>().GenerateStructure();
					TotalGenBlocks += Body.Torso[x].prefab.GetComponent<GenerateDestructiblePiece>().TotalBlocks;
				}
			}

			return checkValid;
		}

		void PopulateLimbComponent(Construct Build) { // Pointless?
			Build.prefab.GetComponent<GenerateDestructiblePiece>().GenData = Build.Stats;
		}

		bool AvatarSetup() {
			bool checkValid = true;

			string[] HTBoneNames = HumanTrait.BoneName;
			HumanDescription HumanDesc = new HumanDescription();

			HumanDesc.upperArmTwist = 0.5f;
			HumanDesc.lowerArmTwist = 0.5f;
			HumanDesc.upperLegTwist = 0.5f;
			HumanDesc.lowerArmTwist = 0.5f;
			HumanDesc.armStretch = 0.5f;
			HumanDesc.legStretch = 0.5f;
			HumanDesc.feetSpacing = 0.0f;
			HumanDesc.hasTranslationDoF = true;

			CreateAvatarTransforms();

			Dictionary<string, Transform> BoneDefiniton = DictionarySetup(); // Populate vaules for the bone structure

			// Bones that will be used in the Avatar component
			List<HumanBone> HumanBones = new List<HumanBone>(HTBoneNames.Length);
			for (int x = 0; x < HTBoneNames.Length; x++) {
				Transform bone;

				if (BoneDefiniton.TryGetValue(HTBoneNames[x], out bone)) {
					HumanBone HB = new HumanBone();
					HB.humanName = HTBoneNames[x];
					HB.boneName = bone.name;
					HB.limit.useDefaultValues = true;

					HumanBones.Add(HB);
				}
			}

			HumanDesc.human = HumanBones.ToArray();

			// as the transform names for the bones are identacle just duplicate the values into the bones
			List<SkeletonBone> SkeletonBones = new List<SkeletonBone>(AvatarTransforms.Length);
			for (int x = 0; x < AvatarTransforms.Length; x++) {
				SkeletonBone SB = new SkeletonBone {
					name = AvatarTransforms[x].name,
					position = AvatarTransforms[x].transform.localPosition,
					rotation = AvatarTransforms[x].transform.localRotation,
					scale = AvatarTransforms[x].transform.localScale
				};

				SkeletonBones.Add(SB);
			}

			SkeletonBone RootBone = new SkeletonBone();
			RootBone.name = this.name; // as parent transforms are needed, include the "this" in the skeleton aswell
			SkeletonBones.Add(RootBone);
			HumanDesc.skeleton = SkeletonBones.ToArray();

			CharacterMesh.GetComponent<SkinnedMeshRenderer>().bones = ReturnGOTransform(AvatarTransforms); // positions of bones

			NewAvatar = AvatarBuilder.BuildHumanAvatar(gameObject, HumanDesc); // create the avatar from this gameobject and the bone structure

			if (NewAvatar != null) {

				NewAvatar.name = "Created Avatar";

				if (!NewAvatar.isValid || !NewAvatar.isHuman) { // Check avatar is valid and human
					Debug.LogError("AvatarFailed"); // no need to throw here considering most of the structure is complete
				}

				if (!this.GetComponent<Animator>()) {
					this.gameObject.AddComponent<Animator>();
				}

				Animator AnimatorComp = this.GetComponent<Animator>();

				AnimatorComp.avatar = NewAvatar;
				AnimatorComp.applyRootMotion = true;
				AnimatorComp.Rebind();

				CharacterMesh.SetActive(false);

			} else {
				checkValid = false;
			}

			return checkValid;
		}

		Dictionary<string, Transform> DictionarySetup() {
			Dictionary<string, Transform> NewDic = new Dictionary<string, Transform>();

			// Bone names and their positions
			#region Dictionary Entries
			NewDic.Add("Hips", AvatarTransforms[0].transform);
			NewDic.Add("Spine", AvatarTransforms[1].transform);
			NewDic.Add("Chest", AvatarTransforms[2].transform);
			NewDic.Add("Neck", AvatarTransforms[3].transform);
			NewDic.Add("Head", AvatarTransforms[4].transform);
			NewDic.Add("LeftShoulder", AvatarTransforms[5].transform);
			NewDic.Add("LeftUpperArm", AvatarTransforms[6].transform);
			NewDic.Add("LeftLowerArm", AvatarTransforms[7].transform);
			NewDic.Add("LeftHand", AvatarTransforms[8].transform);
			NewDic.Add("RightShoulder", AvatarTransforms[9].transform);
			NewDic.Add("RightUpperArm", AvatarTransforms[10].transform);
			NewDic.Add("RightLowerArm", AvatarTransforms[11].transform);
			NewDic.Add("RightHand", AvatarTransforms[12].transform);
			NewDic.Add("LeftUpperLeg", AvatarTransforms[13].transform);
			NewDic.Add("LeftLowerLeg", AvatarTransforms[14].transform);
			NewDic.Add("LeftFoot", AvatarTransforms[15].transform);
			NewDic.Add("LeftToes", AvatarTransforms[16].transform);
			NewDic.Add("RightUpperLeg", AvatarTransforms[17].transform);
			NewDic.Add("RightLowerLeg", AvatarTransforms[18].transform);
			NewDic.Add("RightFoot", AvatarTransforms[19].transform);
			NewDic.Add("RightToes", AvatarTransforms[20].transform);
			#endregion

			return NewDic;
		}

		void CreateAvatarTransforms() {
			AvatarTransforms = new GameObject[SkelPoints.Length];

			for (int x = 0; x < SkelPoints.Length; x++) {
				AvatarTransforms[x] = new GameObject(); // create the bones
				AvatarTransforms[x].transform.position = SkelPoints[x]; // position them accordingly
			}

			#region AvatarTransforms Names
			AvatarTransforms[0].name = "AvatarHips";
			AvatarTransforms[1].name = "AvatarSpine";
			AvatarTransforms[2].name = "AvatarChest";
			AvatarTransforms[3].name = "AvatarNeck";
			AvatarTransforms[4].name = "AvatarHead";

			AvatarTransforms[5].name = "Avatar_L_Shoulder";
			AvatarTransforms[6].name = "Avatar_L_UpperArm";
			AvatarTransforms[7].name = "Avatar_L_LowerArm";
			AvatarTransforms[8].name = "Avatar_L_Hand";

			AvatarTransforms[9].name = "Avatar_R_Shoulder";
			AvatarTransforms[10].name = "Avatar_R_UpperArm";
			AvatarTransforms[11].name = "Avatar_R_LowerArm";
			AvatarTransforms[12].name = "Avatar_R_Hand";

			AvatarTransforms[13].name = "Avatar_L_UpperLeg";
			AvatarTransforms[14].name = "Avatar_L_LowerLeg";
			AvatarTransforms[15].name = "Avatar_L_Foot";
			AvatarTransforms[16].name = "Avatar_L_Toes";

			AvatarTransforms[17].name = "Avatar_R_UpperLeg";
			AvatarTransforms[18].name = "Avatar_R_LowerLeg";
			AvatarTransforms[19].name = "Avatar_R_Foot";
			AvatarTransforms[20].name = "Avatar_R_Toes";
			#endregion

			#region AvatarTransforms Hierarchy
			AvatarTransforms[0].transform.parent = this.transform; // Hips -> ROOT
			AvatarTransforms[1].transform.parent = AvatarTransforms[0].transform; // Spine -> Hips 
			AvatarTransforms[2].transform.parent = AvatarTransforms[1].transform; // Chest -> Spine
			AvatarTransforms[3].transform.parent = AvatarTransforms[2].transform; // Neck -> Chest
			AvatarTransforms[4].transform.parent = AvatarTransforms[3].transform; // Head -> Neck

			AvatarTransforms[5].transform.parent = AvatarTransforms[2].transform; // L_Shoulder -> Chest
			AvatarTransforms[6].transform.parent = AvatarTransforms[5].transform; // L_UpperArm -> L_Shoulder
			AvatarTransforms[7].transform.parent = AvatarTransforms[6].transform; // L_LowerArm -> L_UpperArm
			AvatarTransforms[8].transform.parent = AvatarTransforms[7].transform; // L_Hand -> L_LowerArm

			AvatarTransforms[9].transform.parent = AvatarTransforms[2].transform; // R_Shoulder -> Chest
			AvatarTransforms[10].transform.parent = AvatarTransforms[9].transform; // R_UpperArm -> R_Shoulder
			AvatarTransforms[11].transform.parent = AvatarTransforms[10].transform; // R_LowerArm -> R_UpperArm
			AvatarTransforms[12].transform.parent = AvatarTransforms[11].transform; // R_Hand -> R_LowerArm

			AvatarTransforms[13].transform.parent = AvatarTransforms[0].transform; // L_UpperLeg -> Spine
			AvatarTransforms[14].transform.parent = AvatarTransforms[13].transform; // L_LowerLeg -> L_UpperLeg
			AvatarTransforms[15].transform.parent = AvatarTransforms[14].transform; // L_Foot -> L_LowerLeg
			AvatarTransforms[16].transform.parent = AvatarTransforms[15].transform; // L_Toes -> L_Foot

			AvatarTransforms[17].transform.parent = AvatarTransforms[0].transform; // R_UpperLeg -> Spine
			AvatarTransforms[18].transform.parent = AvatarTransforms[17].transform; // R_LowerLeg -> R_UpperLeg
			AvatarTransforms[19].transform.parent = AvatarTransforms[18].transform; // R_Foot -> R_LowerLeg
			AvatarTransforms[20].transform.parent = AvatarTransforms[19].transform; // R_Toes -> R_Foot
			#endregion
		}

		bool GenerateMesh() { // THIS FAILED TO WORK (doesnt move with the avatar), STILL INCLUDED BUT HIDDEN TO SHOW THE BODY 'SKELETON'
			bool checkValid = true;

			int NumBodyPieces = 22;

			#region AvatarPieceBuild Section
			ABodyPiece = new GameObject[NumBodyPieces];
			//spine
			ABodyPiece[0] = AvatarPieceBuild(SkelPoints[0], SkelPoints[1]);
			ABodyPiece[1] = AvatarPieceBuild(SkelPoints[1], SkelPoints[2]);
			ABodyPiece[2] = AvatarPieceBuild(SkelPoints[2], SkelPoints[3]);
			ABodyPiece[3] = AvatarPieceBuild(SkelPoints[3], SkelPoints[4]);
			//shoulder frame
			ABodyPiece[4] = AvatarPieceBuild(SkelPoints[2], SkelPoints[5]);
			ABodyPiece[5] = AvatarPieceBuild(SkelPoints[2], SkelPoints[9]);
			ABodyPiece[6] = AvatarPieceBuild(SkelPoints[9], SkelPoints[5]); // Cosmetic
																			//left arm
			ABodyPiece[7] = AvatarPieceBuild(SkelPoints[5], SkelPoints[6]);
			ABodyPiece[8] = AvatarPieceBuild(SkelPoints[6], SkelPoints[7]);
			ABodyPiece[9] = AvatarPieceBuild(SkelPoints[7], SkelPoints[8]);
			//right arm
			ABodyPiece[10] = AvatarPieceBuild(SkelPoints[9], SkelPoints[10]);
			ABodyPiece[11] = AvatarPieceBuild(SkelPoints[10], SkelPoints[11]);
			ABodyPiece[12] = AvatarPieceBuild(SkelPoints[11], SkelPoints[12]);
			//hip frame
			ABodyPiece[13] = AvatarPieceBuild(SkelPoints[0], SkelPoints[13]);
			ABodyPiece[14] = AvatarPieceBuild(SkelPoints[0], SkelPoints[17]);
			ABodyPiece[15] = AvatarPieceBuild(SkelPoints[13], SkelPoints[17]); // Cosmetic
																			   //left leg
			ABodyPiece[16] = AvatarPieceBuild(SkelPoints[13], SkelPoints[14]);
			ABodyPiece[17] = AvatarPieceBuild(SkelPoints[14], SkelPoints[15]);
			ABodyPiece[18] = AvatarPieceBuild(SkelPoints[15], SkelPoints[16]);
			//right leg
			ABodyPiece[19] = AvatarPieceBuild(SkelPoints[17], SkelPoints[18]);
			ABodyPiece[20] = AvatarPieceBuild(SkelPoints[18], SkelPoints[19]);
			ABodyPiece[21] = AvatarPieceBuild(SkelPoints[19], SkelPoints[20]);

			#endregion

			// get all generated meshes and put them in a combine list
			List<CombineInstance> Instances = new List<CombineInstance>();
			CombineInstance combine = new CombineInstance();

			for (int x = 0; x < NumBodyPieces; x++) {
				combine.mesh = ABodyPiece[x].GetComponent<MeshFilter>().sharedMesh;
				combine.transform = this.transform.worldToLocalMatrix * ABodyPiece[x].GetComponent<MeshFilter>().transform.localToWorldMatrix;
				Instances.Add(combine);
			}

			// make the mesh out of the combine list
			Mesh MergedMesh = new Mesh {
				name = "CombinedMeshes"
			};
			MergedMesh.CombineMeshes(Instances.ToArray());

			// create the gameobject which holds the mesh and populate
			CharacterMesh = new GameObject {
				name = "AvatarMesh"
			};
			CharacterMesh.transform.parent = this.transform;
			CharacterMesh.gameObject.AddComponent<SkinnedMeshRenderer>();

			SkinnedMeshRenderer SKMReference = CharacterMesh.GetComponent<SkinnedMeshRenderer>();

			SKMReference.material = new Material(Shader.Find("Diffuse"));
			SKMReference.receiveShadows = false;
			SKMReference.rootBone = this.transform;
			SKMReference.sharedMesh = MergedMesh;
			SKMReference.BakeMesh(MergedMesh);

			for (int x = 0; x < ABodyPiece.Length; x++) {
				DestroyImmediate(ABodyPiece[x]); // destroy the remaining peices
			}

			return checkValid;
		}

		// there isnt a function to return an array of game objects transforms so I made this
		Transform[] ReturnGOTransform(GameObject[] ToReturn) {
			Transform[] ReturnTransforms = new Transform[ToReturn.Length];

			for (int x = 0; x < ToReturn.Length; x++) {
				ReturnTransforms[x] = ToReturn[x].transform;
			}

			return ReturnTransforms;
		}

		// Same as 'ReturnGOTransform' but with the recalucation included for the Matrix's
		Matrix4x4[] ReturnBindPosesM4X4(GameObject[] ToReturn) {
			Matrix4x4[] ReturnMatrix = new Matrix4x4[ToReturn.Length];

			for (int x = 0; x < ToReturn.Length; x++) {
				ReturnMatrix[x] = this.transform.worldToLocalMatrix * ToReturn[x].transform.localToWorldMatrix;
			}

			return ReturnMatrix;
		}

		bool AlignLimbsToAvatarHierarchy() { // Set the parents of the objects to be that of the avatar
			bool checkValid = true;

			Body.Torso[0].prefab.transform.parent = AvatarTransforms[0].transform; // Hip
			Body.Torso[1].prefab.transform.parent = AvatarTransforms[2].transform; // Chest

			Body.Head.prefab.transform.parent = AvatarTransforms[4].transform; // Head

			Body.Joints[0].prefab.transform.parent = AvatarTransforms[6].transform; // Left Shoulder
			Body.Joints[1].prefab.transform.parent = AvatarTransforms[7].transform; // Left Elbow
			Body.Joints[2].prefab.transform.parent = AvatarTransforms[10].transform; // Right Shoulder
			Body.Joints[3].prefab.transform.parent = AvatarTransforms[11].transform; // Right Elbow
			Body.Joints[4].prefab.transform.parent = AvatarTransforms[13].transform; // Left Upper Leg
			Body.Joints[5].prefab.transform.parent = AvatarTransforms[14].transform; // Left Knee
			Body.Joints[6].prefab.transform.parent = AvatarTransforms[17].transform; // Right Upper Leg
			Body.Joints[7].prefab.transform.parent = AvatarTransforms[18].transform; // Right Knee

			Body.Hands[0].prefab.transform.parent = AvatarTransforms[8].transform; // Left Hand
			Body.Hands[1].prefab.transform.parent = AvatarTransforms[12].transform; // Left Hand

			Body.Feet[0].prefab.transform.parent = AvatarTransforms[15].transform; // Left Foot
			Body.Feet[1].prefab.transform.parent = AvatarTransforms[19].transform; // Right Foot

			return checkValid;
		}

		Template UseDefaultData() { // in case the values in the premade gameobjects are lost

			Template BaseValues;
			BaseValues.Limbs = new Construct[8];
			BaseValues.Torso = new Construct[2];
			BaseValues.Joints = new PrefabConstruct[8];
			BaseValues.Hands = new PrefabConstruct[2];
			BaseValues.Feet = new PrefabConstruct[2];

			GameObject LimbPrefab;
			GameObject LowerBodyPrefab;
			GameObject UpperBodyPrefab;
			GameObject JointsPrefab;
			GameObject HandPrefab;
			GameObject FeetPrefab;
			GameObject HeadPrefab;

			GameObject GenBlock;

			#region Data Loading

			if (AssetDatabase.LoadAssetAtPath("Assets/Procedurally Generated Destructible Character/Prefabs/BodyPieces/Limb_Piece.prefab", typeof(GameObject)) != null) {
				LimbPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Procedurally Generated Destructible Character/Prefabs/BodyPieces/Limb_Piece.prefab", typeof(GameObject));

			} else {
				throw new Exception("LimbPrefab default is missing");

			}

			if (AssetDatabase.LoadAssetAtPath("Assets/Procedurally Generated Destructible Character/Prefabs/BodyPieces/LowerBody_Piece.prefab", typeof(GameObject)) != null) {
				LowerBodyPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Procedurally Generated Destructible Character/Prefabs/BodyPieces/LowerBody_Piece.prefab", typeof(GameObject));

			} else {
				throw new Exception("LowerBodyPrefab default is missing");

			}

			if (AssetDatabase.LoadAssetAtPath("Assets/Procedurally Generated Destructible Character/Prefabs/BodyPieces/UpperBody_Piece.prefab", typeof(GameObject)) != null) {
				UpperBodyPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Procedurally Generated Destructible Character/Prefabs/BodyPieces/UpperBody_Piece.prefab", typeof(GameObject));

			} else {
				throw new Exception("UpperBodyPrefab default is missing");

			}

			if (AssetDatabase.LoadAssetAtPath("Assets/Procedurally Generated Destructible Character/Prefabs/BodyPieces/Joint_Piece.prefab", typeof(GameObject)) != null) {
				JointsPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Procedurally Generated Destructible Character/Prefabs/BodyPieces/Joint_Piece.prefab", typeof(GameObject));

			} else {
				throw new Exception("JointsPrefab default is missing");

			}

			if (AssetDatabase.LoadAssetAtPath("Assets/Procedurally Generated Destructible Character/Prefabs/BodyPieces/Hand.prefab", typeof(GameObject)) != null) {
				HandPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Procedurally Generated Destructible Character/Prefabs/BodyPieces/Hand.prefab", typeof(GameObject));

			} else {
				throw new Exception("HandPrefab default is missing");

			}

			if (AssetDatabase.LoadAssetAtPath("Assets/Procedurally Generated Destructible Character/Prefabs/BodyPieces/Foot.prefab", typeof(GameObject)) != null) {
				FeetPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Procedurally Generated Destructible Character/Prefabs/BodyPieces/Foot.prefab", typeof(GameObject));

			} else {
				throw new Exception("FeetPrefab default is missing");

			}

			if (AssetDatabase.LoadAssetAtPath("Assets/Procedurally Generated Destructible Character/Prefabs/BodyPieces/Head.prefab", typeof(GameObject)) != null) {
				HeadPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Procedurally Generated Destructible Character/Prefabs/BodyPieces/Head.prefab", typeof(GameObject));

			} else {
				throw new Exception("HeadPrefab default is missing");

			}

			if (AssetDatabase.LoadAssetAtPath("Assets/Procedurally Generated Destructible Character/Prefabs/BodyPieces/BodyCube.prefab", typeof(GameObject)) != null) {
				GenBlock = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Procedurally Generated Destructible Character/Prefabs/BodyPieces/BodyCube.prefab", typeof(GameObject));

			} else {
				throw new Exception("GenBlock default is missing");

			}
			#endregion

			for (int x = 0; x < BaseValues.Limbs.Length; x++) {
				BaseValues.Limbs[x].prefab = LimbPrefab;
				BaseValues.Limbs[x].Generate = true;
				BaseValues.Limbs[x].Stats.BuildPiece = GenBlock;
				BaseValues.Limbs[x].Stats.PieceType = 0;
				BaseValues.Limbs[x].Stats.BuildType = 0;
				BaseValues.Limbs[x].Stats.PrefabSize = 1;
			}

			BaseValues.Torso[0].prefab = LowerBodyPrefab;
			BaseValues.Torso[0].Generate = true;
			BaseValues.Torso[0].Stats.BuildPiece = GenBlock;
			BaseValues.Torso[0].Stats.PrefabSize = 1;
			BaseValues.Torso[1].prefab = UpperBodyPrefab;
			BaseValues.Torso[1].Generate = true;
			BaseValues.Torso[1].Stats.BuildPiece = GenBlock;
			BaseValues.Torso[1].Stats.PrefabSize = 1;

			for (int x = 0; x < BaseValues.Joints.Length; x++) {
				BaseValues.Joints[x].prefab = JointsPrefab;
				BaseValues.Joints[x].Scale = 1;
			}

			BaseValues.Hands[0].prefab = HandPrefab;
			BaseValues.Hands[0].Scale = 1;
			BaseValues.Hands[1].prefab = HandPrefab;
			BaseValues.Hands[1].Scale = 1;

			BaseValues.Feet[0].prefab = FeetPrefab;
			BaseValues.Feet[1].prefab = FeetPrefab;

			BaseValues.Head.prefab = HeadPrefab;
			BaseValues.Head.Scale = 1;

			for (int x = 0; x < 4; x++) { // Arms
				BaseValues.Limbs[x].Stats.Height = 6;
				BaseValues.Limbs[x].Stats.Debth = 2;
				BaseValues.Limbs[x].Stats.SquareX = new Vector2(3, 3);
			}

			for (int x = 4; x < 8; x++) { // Legs
				BaseValues.Limbs[x].Stats.Height = 8;
				BaseValues.Limbs[x].Stats.Debth = 2;
				BaseValues.Limbs[x].Stats.SquareX = new Vector2(4, 4);
			}

			BaseValues.Torso[0].Stats.PieceType = 1;
			BaseValues.Torso[0].Stats.Height = 8;
			BaseValues.Torso[0].Stats.Debth = 3;
			BaseValues.Torso[0].Stats.SquareX = new Vector2(7, 7);

			BaseValues.Torso[1].Stats.PieceType = 2;
			BaseValues.Torso[1].Stats.Height = 6;
			BaseValues.Torso[1].Stats.Debth = 3;
			BaseValues.Torso[1].Stats.SquareX = new Vector2(8, 8);

			BaseValues.Limbs[0].Name = "Left Upper Arm";
			BaseValues.Limbs[0].Name = "Left Lower Arm";
			BaseValues.Limbs[0].Name = "Right Upper Arm";
			BaseValues.Limbs[0].Name = "Right Lower Arm";
			BaseValues.Limbs[0].Name = "Left Upper Leg";
			BaseValues.Limbs[0].Name = "Left Lower Leg";
			BaseValues.Limbs[0].Name = "Right Upper Leg";
			BaseValues.Limbs[0].Name = "Right Lower Leg";

			BaseValues.Joints[0].Name = "Left Shoulder";
			BaseValues.Joints[0].Name = "Left Elbow";
			BaseValues.Joints[0].Name = "Right Shoulder";
			BaseValues.Joints[0].Name = "Right Elbow";
			BaseValues.Joints[0].Name = "Left Hip";
			BaseValues.Joints[0].Name = "Left Knee";
			BaseValues.Joints[0].Name = "Right Hip";
			BaseValues.Joints[0].Name = "Right Knee";

			BaseValues.Torso[0].Name = "Lower Body";
			BaseValues.Torso[0].Name = "Upper Body";

			BaseValues.Hands[0].Name = "Left Hand";
			BaseValues.Hands[0].Name = "Right Hand";

			BaseValues.Feet[0].Name = "Left Foot";
			BaseValues.Feet[0].Name = "Right Foot";

			BaseValues.Head.Name = "Head";

			return BaseValues;
		}

		public void SaveAvatar() { // The avatar is created is only a temp so it needs saving to be reused
			if (NewAvatar == null) {
				if (GetComponent<Animator>() != null) {
					if (GetComponent<Animator>().avatar != null) {
						NewAvatar = this.GetComponent<Animator>().avatar;
						Debug.Log("Avatar in Script not found, using Animator Avatar");
					} else {
						Debug.LogError("Avatar not found, Not available in Scipt or Animator");
						return;
					}
				} else {
					Debug.LogError("Avatar not found, Not available in Scipt and Animator is missing");
					return;
				}
			}
			if (AssetDatabase.Contains(NewAvatar)) {
				AssetDatabase.SaveAssets();
				Debug.Log("Save Found, Rewriting Avatar");
			} else {
				int count = 1;
				bool unique = false;
				while (!unique) {
					if (AssetDatabase.LoadAssetAtPath<Avatar>("Assets/Scored's Asset Packages/Procedurally Generated Destructible Character/Prefabs/Body Templates/Saved Avatars/GeneratedAvatar" + count.ToString() + ".asset")) {
						count++;
					} else {
						unique = true;
					}
				}
				AssetDatabase.CreateAsset(NewAvatar, "Assets/Scored's Asset Packages/Procedurally Generated Destructible Character/Prefabs/Body Templates/Saved Avatars/GeneratedAvatar" + count.ToString() + ".asset");
			}

			//AssetDatabase.CreateAsset(NewAvatar, "Assets/Procedurally Generated Destructible Character/GeneratedAvatar.asset"); // Original
		}

		public void ResetAndCleanUp(bool fail) {
			_generated = false;

			foreach (Transform child in transform) {
				DestroyImmediate(child.gameObject);
			}

			if (fail) {
				LoadBodyFromRecovery();
			}
		}

		#region Save And Load Template Methods

		private void SaveBodyToRecovery() {
			RecoveryBody = new Template() {
				Limbs = new Construct[8] {
				new Construct() {
					Generate = Body.Limbs[0].Generate,
					Name = Body.Limbs[0].Name,
					prefab = Body.Limbs[0].prefab,
					Stats = Body.Limbs[0].Stats
				},
				new Construct() {
					Generate = Body.Limbs[1].Generate,
					Name = Body.Limbs[1].Name,
					prefab = Body.Limbs[1].prefab,
					Stats = Body.Limbs[1].Stats
				},
				new Construct() {
					Generate = Body.Limbs[2].Generate,
					Name = Body.Limbs[2].Name,
					prefab = Body.Limbs[2].prefab,
					Stats = Body.Limbs[2].Stats
				},
				new Construct() {
					Generate = Body.Limbs[3].Generate,
					Name = Body.Limbs[3].Name,
					prefab = Body.Limbs[3].prefab,
					Stats = Body.Limbs[3].Stats
				},
				new Construct() {
					Generate = Body.Limbs[4].Generate,
					Name = Body.Limbs[4].Name,
					prefab = Body.Limbs[4].prefab,
					Stats = Body.Limbs[4].Stats
				},
				new Construct() {
					Generate = Body.Limbs[5].Generate,
					Name = Body.Limbs[5].Name,
					prefab = Body.Limbs[5].prefab,
					Stats = Body.Limbs[5].Stats
				},
				new Construct() {
					Generate = Body.Limbs[6].Generate,
					Name = Body.Limbs[6].Name,
					prefab = Body.Limbs[6].prefab,
					Stats = Body.Limbs[6].Stats
				},
				new Construct() {
					Generate = Body.Limbs[7].Generate,
					Name = Body.Limbs[7].Name,
					prefab = Body.Limbs[7].prefab,
					Stats = Body.Limbs[7].Stats
				}
			},
				Torso = new Construct[2] {
				new Construct() {
					Generate = Body.Torso[0].Generate,
					Name = Body.Torso[0].Name,
					prefab = Body.Torso[0].prefab,
					Stats = Body.Torso[0].Stats
				},
				new Construct() {
					Generate = Body.Torso[1].Generate,
					Name = Body.Torso[1].Name,
					prefab = Body.Torso[1].prefab,
					Stats = Body.Torso[1].Stats
				}
			},
				Joints = new PrefabConstruct[8] {
				new PrefabConstruct() {
					Name = Body.Joints[0].Name,
					prefab = Body.Joints[0].prefab,
					Scale = Body.Joints[0].Scale
				},
				new PrefabConstruct() {
					Name = Body.Joints[1].Name,
					prefab = Body.Joints[1].prefab,
					Scale = Body.Joints[1].Scale
				},
				new PrefabConstruct() {
					Name = Body.Joints[2].Name,
					prefab = Body.Joints[2].prefab,
					Scale = Body.Joints[2].Scale
				},
				new PrefabConstruct() {
					Name = Body.Joints[3].Name,
					prefab = Body.Joints[3].prefab,
					Scale = Body.Joints[3].Scale
				},
				new PrefabConstruct() {
					Name = Body.Joints[4].Name,
					prefab = Body.Joints[4].prefab,
					Scale = Body.Joints[4].Scale
				},
				new PrefabConstruct() {
					Name = Body.Joints[5].Name,
					prefab = Body.Joints[5].prefab,
					Scale = Body.Joints[5].Scale
				},
				new PrefabConstruct() {
					Name = Body.Joints[6].Name,
					prefab = Body.Joints[6].prefab,
					Scale = Body.Joints[6].Scale
				},
				new PrefabConstruct() {
					Name = Body.Joints[7].Name,
					prefab = Body.Joints[7].prefab,
					Scale = Body.Joints[7].Scale
				}
			},
				Hands = new PrefabConstruct[2] {
				new PrefabConstruct() {
					Name = Body.Hands[0].Name,
					prefab = Body.Hands[0].prefab,
					Scale = Body.Hands[0].Scale
				},
				new PrefabConstruct() {
					Name = Body.Hands[1].Name,
					prefab = Body.Hands[1].prefab,
					Scale = Body.Hands[1].Scale
				}
			},
				Feet = new PrefabConstruct[2] {
				new PrefabConstruct() {
					Name = Body.Feet[0].Name,
					prefab = Body.Feet[0].prefab,
					Scale = Body.Feet[0].Scale
				},
				new PrefabConstruct() {
					Name = Body.Feet[1].Name,
					prefab = Body.Feet[1].prefab,
					Scale = Body.Feet[1].Scale
				}
			},
				Head = new PrefabConstruct() {
					Name = Body.Head.Name,
					prefab = Body.Head.prefab,
					Scale = Body.Head.Scale
				}
			};
		}

		private void LoadBodyFromRecovery() {
			Body = new Template() {
				Limbs = new Construct[8] {
				new Construct() {
					Generate = RecoveryBody.Limbs[0].Generate,
					Name = RecoveryBody.Limbs[0].Name,
					prefab = RecoveryBody.Limbs[0].prefab,
					Stats = RecoveryBody.Limbs[0].Stats
				},
				new Construct() {
					Generate = RecoveryBody.Limbs[1].Generate,
					Name = RecoveryBody.Limbs[1].Name,
					prefab = RecoveryBody.Limbs[1].prefab,
					Stats = RecoveryBody.Limbs[1].Stats
				},
				new Construct() {
					Generate = RecoveryBody.Limbs[2].Generate,
					Name = RecoveryBody.Limbs[2].Name,
					prefab = RecoveryBody.Limbs[2].prefab,
					Stats = RecoveryBody.Limbs[2].Stats
				},
				new Construct() {
					Generate = RecoveryBody.Limbs[3].Generate,
					Name = RecoveryBody.Limbs[3].Name,
					prefab = RecoveryBody.Limbs[3].prefab,
					Stats = RecoveryBody.Limbs[3].Stats
				},
				new Construct() {
					Generate = RecoveryBody.Limbs[4].Generate,
					Name = RecoveryBody.Limbs[4].Name,
					prefab = RecoveryBody.Limbs[4].prefab,
					Stats = RecoveryBody.Limbs[4].Stats
				},
				new Construct() {
					Generate = RecoveryBody.Limbs[5].Generate,
					Name = RecoveryBody.Limbs[5].Name,
					prefab = RecoveryBody.Limbs[5].prefab,
					Stats = RecoveryBody.Limbs[5].Stats
				},
				new Construct() {
					Generate = RecoveryBody.Limbs[6].Generate,
					Name = RecoveryBody.Limbs[6].Name,
					prefab = RecoveryBody.Limbs[6].prefab,
					Stats = RecoveryBody.Limbs[6].Stats
				},
				new Construct() {
					Generate = RecoveryBody.Limbs[7].Generate,
					Name = RecoveryBody.Limbs[7].Name,
					prefab = RecoveryBody.Limbs[7].prefab,
					Stats = RecoveryBody.Limbs[7].Stats
				}
			},
				Torso = new Construct[2] {
				new Construct() {
					Generate = RecoveryBody.Torso[0].Generate,
					Name = RecoveryBody.Torso[0].Name,
					prefab = RecoveryBody.Torso[0].prefab,
					Stats = RecoveryBody.Torso[0].Stats
				},
				new Construct() {
					Generate = RecoveryBody.Torso[1].Generate,
					Name = RecoveryBody.Torso[1].Name,
					prefab = RecoveryBody.Torso[1].prefab,
					Stats = RecoveryBody.Torso[1].Stats
				}
			},
				Joints = new PrefabConstruct[8] {
				new PrefabConstruct() {
					Name = RecoveryBody.Joints[0].Name,
					prefab = RecoveryBody.Joints[0].prefab,
					Scale = RecoveryBody.Joints[0].Scale
				},
				new PrefabConstruct() {
					Name = RecoveryBody.Joints[1].Name,
					prefab = RecoveryBody.Joints[1].prefab,
					Scale = RecoveryBody.Joints[1].Scale
				},
				new PrefabConstruct() {
					Name = RecoveryBody.Joints[2].Name,
					prefab = RecoveryBody.Joints[2].prefab,
					Scale = RecoveryBody.Joints[2].Scale
				},
				new PrefabConstruct() {
					Name = RecoveryBody.Joints[3].Name,
					prefab = RecoveryBody.Joints[3].prefab,
					Scale = RecoveryBody.Joints[3].Scale
				},
				new PrefabConstruct() {
					Name = RecoveryBody.Joints[4].Name,
					prefab = RecoveryBody.Joints[4].prefab,
					Scale = RecoveryBody.Joints[4].Scale
				},
				new PrefabConstruct() {
					Name = RecoveryBody.Joints[5].Name,
					prefab = RecoveryBody.Joints[5].prefab,
					Scale = RecoveryBody.Joints[5].Scale
				},
				new PrefabConstruct() {
					Name = RecoveryBody.Joints[6].Name,
					prefab = RecoveryBody.Joints[6].prefab,
					Scale = RecoveryBody.Joints[6].Scale
				},
				new PrefabConstruct() {
					Name = RecoveryBody.Joints[7].Name,
					prefab = RecoveryBody.Joints[7].prefab,
					Scale = RecoveryBody.Joints[7].Scale
				}
			},
				Hands = new PrefabConstruct[2] {
				new PrefabConstruct() {
					Name = RecoveryBody.Hands[0].Name,
					prefab = RecoveryBody.Hands[0].prefab,
					Scale = RecoveryBody.Hands[0].Scale
				},
				new PrefabConstruct() {
					Name = RecoveryBody.Hands[1].Name,
					prefab = RecoveryBody.Hands[1].prefab,
					Scale = RecoveryBody.Hands[1].Scale
				}
			},
				Feet = new PrefabConstruct[2] {
				new PrefabConstruct() {
					Name = RecoveryBody.Feet[0].Name,
					prefab = RecoveryBody.Feet[0].prefab,
					Scale = RecoveryBody.Feet[0].Scale
				},
				new PrefabConstruct() {
					Name = RecoveryBody.Feet[1].Name,
					prefab = RecoveryBody.Feet[1].prefab,
					Scale = RecoveryBody.Feet[1].Scale
				}
			},
				Head = new PrefabConstruct() {
					Name = RecoveryBody.Head.Name,
					prefab = RecoveryBody.Head.prefab,
					Scale = RecoveryBody.Head.Scale
				}
			};
		}

		#endregion
	}
}