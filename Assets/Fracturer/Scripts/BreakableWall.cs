﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using csDelaunay;

namespace Bytesized
{
    /// <summary>
    ///  Breaks the wall/surface into independent shards using delaunay triangulation.
    /// </summary>
	public class BreakableWall : MonoBehaviour
	{
		[Tooltip("Detail for the triangulation. This is more or less equal to the amount of triangles on the given surface.")]
		public int Detail = 40;
		[Tooltip("Optimization option that only creates front facing part of the mesh for the shard. Useful for far away objects where detail is not needed.")]
		public bool OnlySurface;
		[Tooltip("Layer to set shards on when instatiating them. Useful for making raycasts against them.")]
		public LayerMask ShardLayer;

		void Start()
		{
		    var offset = default(Vector2);
			var regions = CreateRegions(out offset);
			Subdivide(regions, transform.localScale, offset);
			/* We don't need to show the original mesh anymore since now its composed of shards. */
			this.GetComponent<MeshRenderer>().enabled = false;
		}

		//
		public void SetSubdivide()
		{

		}

		/// <summary>
		/// Creates the regions to use as subdivision using the open source libary csDelaunay (https://github.com/PouletFrit/csDelaunay)
		/// </summary>
		/// <returns>A list of the generated regions.</returns>
		private List<List<Vector2f>> CreateRegions(out Vector2 Offset)
		{
			/* Use the scale of the parent transform */
			var scale = transform.localScale;
			var left = -scale.x/2.0f;
			var right = scale.x/2.0f;
			var down = -scale.y/2.0f;
			var up = scale.y/2.0f;

			Offset = new Vector2(left, down);
			var bounds = new Rectf(0, 0, right - left, up - down);
			var sites = new List<Vector2f>();

			/* Create some random points that will act as the "center" of the triangles. Higher detail = Higher triangles */
			for(int i = 0; i < Detail; i++)
			{
				var x = Random.Range(0, right - left);
				var y = Random.Range(0, up - down);
				var point = new Vector2f(x, y);
				sites.Add(point);
			}

			/* Pass the generated data to the open source library */
			return new Voronoi(sites, bounds, 1).Regions();
		}

		/// <summary>
		/// Subdivides the mesh of the attached object.
		/// </summary>
		/// <param name="Regions">The array of regions generated by the delaunay triangulation.</param>
		/// <param name="Scale">The scale to use for the shards. Usually the scale of the parent.</param>
		private void Subdivide(List<List<Vector2f>> Regions, Vector3 Scale, Vector2 Offset)
		{
			for(var i = 0; i < Regions.Count; ++i)
			{
				var region = Regions[i];
				var len = region.Count;
				var trisCount = 12 * (len - 1);
				var tris = new int[trisCount];
				var verts = new Vector3[3 * len];
				var uvs = new Vector2[3 * len];

				AppendVerticesAndUVs(verts, uvs, region, Scale, Offset);
				AppendTriangles(tris, len);
				CreateShard(tris, verts, uvs);
			}
		}

		/// <summary>
		/// Generates the vertices and UVs for the new mesh give the region.
		/// </summary>
		/// <param name="Vertices">The vertices of the mesh</param>
		/// <param name="UVs">The UVs of the mesh</param>
		/// <param name="Region">The region generated by the delaunay triangulation.</param>
		/// <param name="Scale">The scale to use for the shard</param>
		private void AppendVerticesAndUVs(Vector3[] Vertices, Vector2[] UVs, List<Vector2f> Region, Vector3 Scale, Vector2 Offset)
		{
			for(int i = 0; i < Region.Count; ++i)
			{
				var coord = Region[i];
				int one = i, two = Region.Count + i, three = 2 * Region.Count + i;
				Vertices[one] = new Vector3(coord.x + Offset.x, coord.y + Offset.y, Scale.z/2.0f);
				Vertices[two] = new Vector3(coord.x + Offset.x, coord.y + Offset.y, -Scale.z/2.0f);
				Vertices[three] = new Vector3(coord.x + Offset.x, coord.y + Offset.y, -Scale.z/2.0f);

				var scaleX = (float)1f / Scale.x;
				var scaleY = (float)1f / Scale.y;
				UVs[one] = new Vector2(scaleX, scaleY);
				UVs[two] = new Vector2(coord.x * -scaleX, coord.y * -scaleY);
				UVs[three] = new Vector2(scaleX, scaleY);
			}
		}

		/// <summary>
		/// Generates the triangles for the shard mesh.
		/// </summary>
		/// <param name="Tris">The indices (or triangles) of the mesh</param>
		/// <param name="Len">The length of the region array</param>
		private void AppendTriangles(int[] Tris, int Len)
		{
			var t = 0;
			/* Add the front facing polygon of the shard */
			for(int v = 1; v < Len - 1; v++)
			{
				Tris[t++] = Len + v + 1;
				Tris[t++] = Len + v;
				Tris[t++] = Len;

				Tris[t++] = 2 * Len;
				Tris[t++] = 2 * Len + v;
				Tris[t++] = 2 * Len + v + 1;
			}

			/* If the OnlySurface surface optimization is not enabled add the remaining faces */
			if (OnlySurface) return;
			/* Creates the rest of the faces */
			for(int v = 0; v < Len; v++)
			{
				var n = v == (Len-1) ? 0 : v+1;

				Tris[t++] = v;
				Tris[t++] = Len + v;
				Tris[t++] = Len + n;

				Tris[t++] = v;
				Tris[t++] = Len + n;
				Tris[t++] = n;
			}
		}

		/// <summary>
		/// Creates and instantiates a shard object which represents a piece of the wall.
		/// </summary>
		/// <param name="Tris">The indices (or triangles) of the mesh</param>
		/// <param name="Vertices">The vertices of the mesh</param>
		/// <param name="UVs">The UVs of the mesh</param>
		private void CreateShard(int[] Tris, Vector3[] Vertices, Vector2[] UVs)
		{
			var mesh = BuildMeshWithoutSharedVertices(Tris, Vertices, UVs);
			var shard = new GameObject();
			shard.name = transform.name + "_Shard";
			shard.transform.SetParent(transform, false);
			shard.transform.localPosition = Vector3.zero;
			shard.transform.localRotation = Quaternion.identity;
			shard.transform.localScale = Vector3.one;
			shard.layer = LayerMaskToLayer(ShardLayer);
			shard.AddComponent<MeshCollider>();
			shard.AddComponent<MeshFilter>();
			shard.AddComponent<MeshRenderer>();
			shard.AddComponent<Rigidbody>();
			shard.GetComponent<Rigidbody>().isKinematic = true;
			shard.GetComponent<MeshCollider>().convex = true;
			shard.GetComponent<MeshCollider>().sharedMesh = mesh;
			shard.GetComponent<MeshFilter>().sharedMesh = mesh;
			shard.GetComponent<Renderer>().material = transform.GetComponent<Renderer>().material;
			transform.localScale = Vector3.one;
		}

		private int LayerMaskToLayer(LayerMask Layer)
		{
			var n = Layer.value;
			var mask = 0;
			while(n > 1)
			{
				n = n >> 1;
				mask++;
			}
			return mask;
		}

		/// <summary>
		/// Creates a mesh without shared vertices so that each triangle can have independent normals.
		/// </summary>
		/// <param name="Tris">The indices (or triangles) of the mesh</param>
		/// <param name="Vertices">The vertices of the mesh</param>
		/// <param name="UVs">The UVs of the mesh</param>
		/// <returns>A mesh object</returns>
		private Mesh BuildMeshWithoutSharedVertices(int[] Tris, Vector3[] Vertices, Vector2[] UVs)
		{
			var newVertices = new List<Vector3>(Vertices);
			var newUVs = new List<Vector2>(UVs);
			/* Hashsets have O(1) access */
			var visited = new HashSet<int>();
			/* Iterate over all the triangles and if we find 2 triangles that share a vertex we create another vertex with the same values. */
			for(var i = 0; i < Tris.Length; ++i)
			{
				if(visited.Contains(Tris[i]))
				{
					/* Duplicate the vertex so its not shared anymore */
					newVertices.Add(Vertices[Tris[i]]);
					newUVs.Add(UVs[Tris[i]]);
					Tris[i] = newVertices.Count-1;
				}
				visited.Add(Tris[i]);
			}
			/* Create the mesh with the desired data */
			var mesh = new Mesh();
			mesh.vertices = newVertices.ToArray();
			mesh.uv = newUVs.ToArray();
			mesh.triangles = Tris;
			/* Now that no triangle shares vertices, calculate the normals. */
			mesh.RecalculateNormals();
			return mesh;
		}
	}
}
