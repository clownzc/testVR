using UnityEngine;

namespace ScoredProductions.PGDC
{

	[System.Serializable]
	public struct BuildStats
	{
		public int PieceType;
		public bool FillCore;
		public bool Destructible;
		public float FillHealth;
		public float DestroyTime;
		public bool DestroyOnSleep;
		public GameObject CustomFill;
		public GameObject BuildPiece;
		public float PrefabSize;
		public int Height;
		public int Debth;
		public int BuildType;
		public int CircleX;
		public float Offset;
		public Vector2 SquareX;
	}

	// Global struct to simplify passing the information between the body stats and the generation script

}