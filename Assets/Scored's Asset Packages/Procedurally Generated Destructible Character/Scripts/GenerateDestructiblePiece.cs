using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScoredProductions.PGDC
{

	public class GenerateDestructiblePiece :MonoBehaviour
	{

		public BuildStats GenData;

		private int PieceType; // 0 for a limb, 1 for the lower body, 2 for the upper body
		private bool FillCore; // Fills the empty space with a single object
		private bool DestCore; // If the core is destrucable or not
		private float FillHealth; // Health of the core
		private float DestroyTime; // time it takes for it to be destroyed
		private bool DestroyOnSleep; // when the object stops moving should it be destroyed (doesnt use 'DestroyTime')
		private GameObject CustomFill; // GameObject used to fill the core (duplicated, merged together)
		private GameObject BuildPiece; // GameObject used to generate over the limb
		private int BuildType; // 0 = Square, 1 = Circle...
		private float PrefabSize; // example : if "BuildPiece" is the default cube, Prefab size = 1 in terms of size
		private int Height; // How tall do you want it
		private int Debth; // How many inner layers to generate
		private int CircleX; // amount of blocks in the circumference (size calulated appropriatly)
		private float Offset; // How far in/out should the blocks be positioned on the circumference (0.567 makes the cubes outer corners connect)
		private Vector2 SquareX; // Length = x, Width = y

		private GameObject[] Points; // Connection points attached (generated if needed)
		private bool[] PointReq; // if pregenerated doesnt exist anymore then use this reference
		public int TotalBlocks; // how many blocks have been generated
		private GameObject[,,] AllGeneratedBlocks; // Reference to every block
		private GameObject FillBlock; // if you want the gap in the middle filling this is it
		private GameObject BlockParent; // Container for all the blocks

		public void GenerateStructure() { // create the block structure here
			TotalBlocks = 0; // Start process resets the value to 0

			PopulateVariables(); // populate vaules from the global struct 'BuildStats'

			CheckConnectionPoints(); // Checks if the connection points are their, if not creates them

			switch (BuildType) { // Made to be expanded uppon with different styles (dont have time to think of new patterns)
				case 0:
					SquareBuild();
					break;
				case 1:
					CircleBuild();
					break;
			}
			ConnectionPointPlacement(); // place the connection points after generation
		}

		void CheckConnectionPoints() { // Made to check the 3 connection types for validity ("Limb_Piece", "LowerBody_Piece", "UpperBody_Piece")
			if (PieceType == 0) { // "Limb_Piece" section
				Points = new GameObject[1]; // array of gameobjects if needed
				PointReq = new bool[1]; // array of bools to determin which need generating

				// Checker and Creator
				if (!this.transform.Find("ConnectionPoint")) { // if the object of this name cant be found
					Points[0] = new GameObject(); // Create the object
					Points[0].transform.parent = this.transform; // Assign its parent
					Points[0].name = "ConnectionPoint"; // Give it its name
					PointReq[0] = true; // Identity this part needs positioning (default position)

				} else {
					PointReq[0] = false; // Part found, doesnt need work
				}

			} else if (PieceType == 1) { // "LowerBody_Piece" section
				Points = new GameObject[3]; // array of gameobjects if needed
				PointReq = new bool[3]; // array of bools to determin which need generating

				// Checker and Creator
				if (!this.transform.Find("LeftConnectionPoint").gameObject) { // if the object of this name cant be found
					Points[0] = new GameObject();
					Points[0].transform.parent = this.transform;
					Points[0].name = "LeftConnectionPoint";
					PointReq[0] = true;

				} else {
					PointReq[0] = false;
				}

				// Checker and Creator
				if (!this.transform.Find("RightConnectionPoint").gameObject) { // if the object of this name cant be found
					Points[1] = new GameObject();
					Points[1].transform.parent = this.transform;
					Points[1].name = "RightConnectionPoint";
					PointReq[1] = true;

				} else {
					PointReq[1] = false;
				}

				// Checker and Creator
				if (!this.transform.Find("HipPosition").gameObject) { // if the object of this name cant be found
					Points[2] = new GameObject();
					Points[2].transform.parent = this.transform;
					Points[2].name = "HipPosition";
					PointReq[2] = true;

				} else {
					PointReq[2] = false;
				}

			} else if (PieceType == 2) { // "UpperBody_Piece" section
				Points = new GameObject[4]; // array of gameobjects if needed
				PointReq = new bool[4]; // array of bools to determin which need generating

				// Checker and Creator
				if (!this.transform.Find("LeftConnectionPoint").gameObject) { // if the object of this name cant be found
					Points[0] = new GameObject();
					Points[0].transform.parent = this.transform;
					Points[0].name = "LeftConnectionPoint";
					PointReq[0] = true;

				} else {
					PointReq[0] = false;
				}

				// Checker and Creator
				if (!this.transform.Find("RightConnectionPoint").gameObject) { // if the object of this name cant be found
					Points[1] = new GameObject();
					Points[1].transform.parent = this.transform;
					Points[1].name = "RightConnectionPoint";
					PointReq[1] = true;

				} else {
					PointReq[1] = false;
				}

				// Checker and Creator
				if (!this.transform.Find("BottomConnectionPoint").gameObject) { // if the object of this name cant be found
					Points[2] = new GameObject();
					Points[2].transform.parent = this.transform;
					Points[2].name = "BottomConnectionPoint";
					PointReq[2] = true;

				} else {
					PointReq[2] = false;
				}

				// Checker and Creator
				if (!this.transform.Find("ChestPosition").gameObject) { // if the object of this name cant be found
					Points[3] = new GameObject();
					Points[3].transform.parent = this.transform;
					Points[3].name = "ChestPosition";
					PointReq[3] = true;

				} else {
					PointReq[3] = false;
				}
			}
		}

		void PopulateVariables() { // throw all the GenData values into the the local values, prevents modification after the process starts
			PieceType = GenData.PieceType;
			FillCore = GenData.FillCore;
			DestCore = GenData.Destructible;
			BuildPiece = GenData.BuildPiece;
			BuildType = GenData.BuildType;
			PrefabSize = GenData.PrefabSize;
			Height = GenData.Height;
			Debth = GenData.Debth;
			CircleX = GenData.CircleX;
			SquareX = GenData.SquareX;
			DestroyOnSleep = GenData.DestroyOnSleep;
			FillHealth = GenData.FillHealth;
			DestroyTime = GenData.DestroyTime;
			CustomFill = GenData.CustomFill;
			Offset = GenData.Offset;
		}

		void ConnectionPointPlacement() { // move the connection points to their appropriate places
			float Xposition = 0;

			switch (BuildType) { // pattern style
				case 0:
					Xposition = SquareX.x / 2; // Calculation for square
					break;
				case 1:
					Xposition = CircleX / (2 * Mathf.PI) - Offset; // Calculation for circle
					break;
			}

			switch (PieceType) { // Connection point
				case 0:
					if (PointReq[0]) { // If the point needs adjusting 
						Points[0].transform.localPosition = new Vector3(0, -(Height - 1), 0) * PrefabSize;

					} else {
						this.transform.Find("ConnectionPoint").transform.localPosition = new Vector3(0, -(Height - 1), 0) * PrefabSize;
					}

					break;
				case 1: // Left, Right, Hip
					if (PointReq[0]) { // If the point needs adjusting 
						Points[0].transform.localPosition = new Vector3(-Xposition, -(Height - 1), 0) * PrefabSize;

					} else {
						this.transform.Find("LeftConnectionPoint").transform.localPosition = new Vector3(-(Xposition - 1), -(Height - 1), 0) * PrefabSize;
					}

					if (PointReq[1]) { // If the point needs adjusting 
						Points[1].transform.localPosition = new Vector3(Xposition, -(Height - 1), 0) * PrefabSize;

					} else {
						this.transform.Find("RightConnectionPoint").transform.localPosition = new Vector3(Xposition - 1, -(Height - 1), 0) * PrefabSize;
					}

					if (PointReq[2]) { // If the point needs adjusting 
						Points[2].transform.localPosition = new Vector3(0, -(Height - 1), 0) * PrefabSize;

					} else {
						this.transform.Find("HipPosition").transform.localPosition = new Vector3(0, -(Height - 1) / 1.5f, 0) * PrefabSize;
					}

					break;
				case 2: // Left, Right, Bottom, Chest
					if (PointReq[0]) { // If the point needs adjusting 
						Points[0].transform.localPosition = new Vector3(-Xposition, -1, 0) * PrefabSize;

					} else {
						this.transform.Find("LeftConnectionPoint").transform.localPosition = new Vector3(-(Xposition - 1), -1, 0) * PrefabSize;
					}

					if (PointReq[1]) { // If the point needs adjusting 
						Points[1].transform.localPosition = new Vector3(Xposition, -1, 0) * PrefabSize;

					} else {
						this.transform.Find("RightConnectionPoint").transform.localPosition = new Vector3(Xposition - 1, -1, 0) * PrefabSize;
					}

					if (PointReq[2]) { // If the point needs adjusting 
						Points[2].transform.localPosition = new Vector3(0, -(Height), 0) * PrefabSize;

					} else {
						this.transform.Find("BottomConnectionPoint").transform.localPosition = new Vector3(0, -(Height), 0) * PrefabSize;
					}

					if (PointReq[3]) { // If the point needs adjusting 
						Points[3].transform.localPosition = new Vector3(0, -(Height) / 2, 0) * PrefabSize;

					} else {
						this.transform.Find("ChestPosition").transform.localPosition = new Vector3(0, -(Height) / 2, 0) * PrefabSize;
					}

					break;
			}
		}

		void CheckFillHasMesh() { // check if the custom gameobject use to fill the core has a mesh, if not throw
			if (CustomFill.GetComponent<MeshFilter>() != null) {
				if (CustomFill.GetComponent<MeshFilter>().mesh == null) {
					throw new Exception("No mesh assigned to Custom Fill prefab");
				}

			} else if (CustomFill.GetComponent<SkinnedMeshRenderer>() != null) {
				if (CustomFill.GetComponent<SkinnedMeshRenderer>().sharedMesh == null) {
					throw new Exception("No mesh assigned to Custom Fill prefab");
				}

			} else {
				throw new Exception("No Renderer Component assigned to Custom Fill prefab");
			}
		}

		void SquareBuild() { // Function to build the structure in the pattern
			AllGeneratedBlocks = new GameObject[Height, Debth, Mathf.CeilToInt(2 * (SquareX.x + SquareX.y))]; // initialise storage

			if (CustomFill != null) { // I think this is in the wrong place
				CheckFillHasMesh();
			}

			// Create the gameobject to hold the created blocks
			BlockParent = new GameObject();
			BlockParent.transform.parent = this.transform; // Assign place under this
			BlockParent.transform.localPosition = Vector3.zero; // Set to 0 and its empty and only a container
			BlockParent.name = "BlockHolder";

			float Xnum, Ynum;
			Xnum = SquareX.x; // Initialise to avoid warnings
			Ynum = SquareX.y;

			Vector3[] Corners = new Vector3[4]; // The 4 corners of a square, no need for 8 as height is moved automaticly

			// Its made by taking the row, then for each layer, build the blocks around the shape of the square 
			for (int y = 0; y < Height; y++) { // Each row
				Xnum = SquareX.x - 1; // WHY -1? I DONT GET IT BUT IT WORKS
				Ynum = SquareX.y - 1; // WHY -1? I DONT GET IT BUT IT WORKS

				Corners[0] = new Vector3(0, -y, 0) * PrefabSize; // Top Right
				Corners[1] = new Vector3(0, -y, 0) * PrefabSize; // Top Left
				Corners[2] = new Vector3(0, -y, 0) * PrefabSize; // Bottom Left
				Corners[3] = new Vector3(0, -y, 0) * PrefabSize; // Bottom Right

				for (int z = 0; z < Debth; z++) { // Each layer
					if (z != 0) { // If the debth is not the base level
						Xnum -= 2; // Remove 2 squares (1 layer to the next is 2 less blocks)
						Ynum -= 2;
					}

					if (Xnum > 0 && Ynum > 0) { // If their are blocks to generate (either = 0 means none can be made)
												// Input new x and z / DONT CHANGE Y
						Corners[0].x = (Xnum / 2) * PrefabSize; // Half the diamater then multiplied by the size
						Corners[0].z = (Ynum / 2) * PrefabSize;
						Corners[1].x = (-Xnum / 2) * PrefabSize;
						Corners[1].z = (Ynum / 2) * PrefabSize;
						Corners[2].x = (-Xnum / 2) * PrefabSize;
						Corners[2].z = (-Ynum / 2) * PrefabSize;
						Corners[3].x = (Xnum / 2) * PrefabSize;
						Corners[3].z = (-Ynum / 2) * PrefabSize;

						int Xcount = 0;

						for (int q = 0; q < 4; q++) { // The 4 sides of a square
							switch (q) {
								case 0:
									for (int x = 0; x < Xnum; x++) { // For each block
										AllGeneratedBlocks[y, z, Xcount] = Instantiate(BuildPiece, this.transform); // Create the block
										AllGeneratedBlocks[y, z, Xcount].transform.localPosition = Corners[q]; // Assign the position to the corner

										if (x != 0) { // If its not the first block
											AllGeneratedBlocks[y, z, Xcount].transform.localPosition += new Vector3(-x, 0, 0) * PrefabSize; // Move according to which block it is
										}

										AllGeneratedBlocks[y, z, Xcount].transform.parent = BlockParent.transform; // Assign parent
										Xcount++; // Increment to the next position in the array
										TotalBlocks++; // Track how many blocks have been made
									}
									break;
								case 1:
									for (int x = 0; x < Ynum; x++) { // For each block
										AllGeneratedBlocks[y, z, Xcount] = Instantiate(BuildPiece, this.transform); // Create the block
										AllGeneratedBlocks[y, z, Xcount].transform.localPosition = Corners[q]; // Assign the position to the corner

										if (x != 0) { // If its not the first block
											AllGeneratedBlocks[y, z, Xcount].transform.localPosition += new Vector3(0, 0, -x) * PrefabSize; // Move according to which block it is
										}

										AllGeneratedBlocks[y, z, Xcount].transform.parent = BlockParent.transform; // Assign Parent
										Xcount++; // Increment to the next position in the array
										TotalBlocks++; // Track how many blocks have been made
									}
									break;
								case 2:
									for (int x = 0; x < Xnum; x++) { // For each block
										AllGeneratedBlocks[y, z, Xcount] = Instantiate(BuildPiece, this.transform); // Create the block
										AllGeneratedBlocks[y, z, Xcount].transform.localPosition = Corners[q]; // Assign the position to the corner

										if (x != 0) { // If its not the first block
											AllGeneratedBlocks[y, z, Xcount].transform.localPosition += new Vector3(x, 0, 0) * PrefabSize; // Move according to which block it is
										}

										AllGeneratedBlocks[y, z, Xcount].transform.parent = BlockParent.transform; // Assign Parent
										Xcount++; // Increment to the next position in the array
										TotalBlocks++; // Track how many blocks have been made
									}
									break;
								case 3:
									for (int x = 0; x < Ynum; x++) { // For each block
										AllGeneratedBlocks[y, z, Xcount] = Instantiate(BuildPiece, this.transform); // Create the block
										AllGeneratedBlocks[y, z, Xcount].transform.localPosition = Corners[q]; // Assign the position to the corner

										if (x != 0) { // If its not the first block
											AllGeneratedBlocks[y, z, Xcount].transform.localPosition += new Vector3(0, 0, x) * PrefabSize; // Move according to which block it is
										}

										AllGeneratedBlocks[y, z, Xcount].transform.parent = BlockParent.transform; // Assign Parent
										Xcount++; // Increment to the next position in the array
										TotalBlocks++; // Track how many blocks have been made
									}
									break;
							}
						}
					} else {
						z = Debth; // End the outer for loop
					}
				}
			}

			Xnum++; // Replace the lost 1
			Ynum++; // Replace the lost 1

			if (FillCore) { // Create the fill if requested
				SquareFill(Xnum, Ynum);
			}
		}

		void SquareFill(float X, float Y) { // 1 debth causes the fill to be out of position, doesnt affect limb pieces
			Vector3 StartPoint = new Vector3();

			float FillX = X - 2; // Fill layer is the inner debth layer (more more then currently made)
			float FillY = Y - 2;

			GameObject[] FillBlocks = new GameObject[Mathf.CeilToInt(FillX * FillY * Height)]; // Blocks to be made into the fill layer
			int FillBlockCount = 0;

			if (FillX > 0 && FillY > 0) { // Make sure their is a size to the fill
				for (int h = 0; h < Height; h++) {
					for (int y = 0; y < FillY; y++) {
						StartPoint = new Vector3(((FillX - 1) / 2), -h, ((FillY - 1) / 2) - y) * PrefabSize; // Top Right

						for (int x = 0; x < FillX; x++) {
							if (CustomFill == null) { // If a custom prefab is to be used
								FillBlocks[FillBlockCount] = Instantiate(BuildPiece, this.transform);

							} else {
								FillBlocks[FillBlockCount] = Instantiate(CustomFill, this.transform);
							}

							// follows the same construction pattern as SquareBuild
							FillBlocks[FillBlockCount].transform.localPosition = StartPoint;

							if (x != 0) {
								FillBlocks[FillBlockCount].transform.localPosition += new Vector3(-x, 0, 0) * PrefabSize;
							}

							FillBlockCount++;
						}
					}
				}

				FillArea(FillBlocks);
			}
		}

		void CircleBuild() {
			AllGeneratedBlocks = new GameObject[Height, Debth, CircleX];

			if (CustomFill != null) { // I think this is in the wrong position
				CheckFillHasMesh();
			}

			// Create the gameobject to hold the created blocks
			BlockParent = new GameObject();
			BlockParent.transform.parent = this.transform;
			BlockParent.transform.localPosition = Vector3.zero;
			BlockParent.name = "BlockHolder";

			float Xnum = CircleX;

			for (int y = 0; y < Height; y++) { // Each row
				Xnum = CircleX;
				Vector3 Origin = new Vector3(0, -y, 0) * PrefabSize;

				for (int z = 0; z < Debth; z++) { // Each layer
					Xnum -= 2;
					float angle = 360.0f / Xnum; // Segments are determined by the amount of blocks to be created
					float radius = (Xnum * PrefabSize) / (2 * Mathf.PI); // Redo distance away from the circle for each layer

					if (Xnum > 0) {
						for (int x = 0; x < Xnum; x++) { // Single spawn loop
							float Len, Wid;
							Len = Mathf.Sin(Mathf.Deg2Rad * angle) * (radius - Offset); // Circle calculation for the position, radius gives it distance
							Wid = Mathf.Cos(Mathf.Deg2Rad * angle) * (radius - Offset);

							if (CustomFill == null) {
								AllGeneratedBlocks[y, z, x] = Instantiate(BuildPiece, this.transform); // Create block

							} else {
								AllGeneratedBlocks[y, z, x] = Instantiate(CustomFill, this.transform);
							}

							AllGeneratedBlocks[y, z, x].transform.localPosition = Origin + new Vector3(Len, 0, Wid);
							AllGeneratedBlocks[y, z, x].transform.parent = BlockParent.transform;
							AllGeneratedBlocks[y, z, x].transform.LookAt(this.transform.position + Origin); // Point the block towards the center

							angle += 360.0f / Xnum; // Add another segment to the angle

							TotalBlocks++; // Increment the amount of blocks
						}
					}
				}
			}
			if (FillCore) {
				CircleFill(Xnum);
			}
		}

		void CircleFill(float CircleX) { // Alittle expensive shape creation but makes a complete fill

			float Xnum = CircleX - 2; // -2 for the next layer

			if (Xnum > 0) { // No next layer then nothing to do
							// Process of creation is the same as the build method
				GameObject[] FillBlocks = new GameObject[Mathf.CeilToInt((Mathf.Pow(CircleX, 2)) * Height)];
				int FillBlockCount = 0;

				for (int y = 0; y < Height; y++) {
					Xnum = CircleX - 2;
					Vector3 Origin = new Vector3(0, -y, 0) * PrefabSize;

					for (int z = 0; z < Mathf.Sqrt(CircleX); z++) {
						Xnum -= 2;
						float angle = 360.0f / Xnum;
						float radius = (Xnum * PrefabSize) / (2 * Mathf.PI);

						if (Xnum > 0) {
							for (int x = 0; x < Xnum; x++) {
								float Len, Wid;
								Len = Mathf.Sin(Mathf.Deg2Rad * angle) * (radius - Offset);
								Wid = Mathf.Cos(Mathf.Deg2Rad * angle) * (radius - Offset);

								if (CustomFill == null) {
									FillBlocks[FillBlockCount] = Instantiate(BuildPiece, this.transform);
								} else {
									FillBlocks[FillBlockCount] = Instantiate(CustomFill, this.transform);
								}

								FillBlocks[FillBlockCount].transform.localPosition = Origin + new Vector3(Len, 0, Wid);
								FillBlocks[FillBlockCount].transform.parent = BlockParent.transform;
								FillBlocks[FillBlockCount].transform.LookAt(this.transform.position + Origin);

								angle += 360.0f / Xnum;

								FillBlockCount++;
							}
						}
					}
				}

				FillArea(FillBlocks);
			}
		}

		void FillArea(GameObject[] FillObjects) { // Process for combining the created blocks

			List<CombineInstance> Instances = new List<CombineInstance>(); // List of data to combine
			CombineInstance combine = new CombineInstance(); // data of the mesh and its postion

			for (int x = 0; x < FillObjects.Length; x++) { // All blocks
				if (FillObjects[x]) {
					if (FillObjects[x].GetComponent<MeshFilter>() != null) { // MeshFiler or SkinnedMeshRenderer data
						combine.mesh = FillObjects[x].GetComponent<MeshFilter>().sharedMesh;

					} else {
						combine.mesh = FillObjects[x].GetComponent<SkinnedMeshRenderer>().sharedMesh;

					}
					combine.transform = FillObjects[x].GetComponent<MeshFilter>().transform.localToWorldMatrix;

					Instances.Add(combine); // Add it to the list
				}
			}

			Mesh MergedMesh = new Mesh(); // Create the mesh object
			MergedMesh.name = "CombinedMeshes";
			MergedMesh.CombineMeshes(Instances.ToArray()); // Combine the meshes to create the single mesh

			GameObject FillMesh = new GameObject();  // Create the gameobject, its components and assign the new mesh
			FillMesh.name = "FillMesh";
			FillMesh.transform.parent = BlockParent.transform;
			FillMesh.gameObject.AddComponent<MeshFilter>();
			FillMesh.gameObject.AddComponent<MeshRenderer>();
			FillMesh.GetComponent<MeshFilter>().mesh = MergedMesh;

			if (CustomFill == null) {  // Default material is used
				FillMesh.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));

			} else {
				if (CustomFill.GetComponent<MeshRenderer>() != null) { // Get custom material used
					FillMesh.GetComponent<MeshRenderer>().material = CustomFill.GetComponent<MeshRenderer>().material;

				} else if (CustomFill.GetComponent<SkinnedMeshRenderer>() != null) {
					FillMesh.GetComponent<MeshRenderer>().material = CustomFill.GetComponent<SkinnedMeshRenderer>().material;

				} else { // If none use default
					FillMesh.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
				}

				if (FillMesh.GetComponent<MeshRenderer>().material == null) { // If found material in customfill is null
					FillMesh.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));

				}
			}

			for (int x = 0; x < FillObjects.Length; x++) {
				DestroyImmediate(FillObjects[x]); // Destroy original building blocks (as they were made into the scene to make the new mesh)

			}

			FillMesh.AddComponent<BlockStats>();

			if (this.transform.GetComponent<BlockStats>() != null) { // Apply basic destrucutable script if not available already
				FillMesh.transform.GetComponent<BlockStats>().Health = FillHealth;
				FillMesh.transform.GetComponent<BlockStats>().DestroyTime = DestroyTime;

				if (DestCore) {
					FillMesh.transform.GetComponent<BlockStats>().Destructible = true;
				}
				if (DestroyOnSleep) {
					FillMesh.transform.GetComponent<BlockStats>().DestroyOnSleep = true;
				}
			}
		}

		public void ClearBlocks() {
			if (BlockParent) { // Destroy all generated blocks, all should be stored under this gameobject
				DestroyImmediate(BlockParent);
			}
		}
	}
}