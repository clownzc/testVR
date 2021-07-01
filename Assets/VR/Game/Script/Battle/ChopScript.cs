
using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;

namespace Battle
{
    public static class ChopScript
    {
        private static bool isPartOf(BoneWeight b, List<int> indices, float threshold)
        {
            float num = 0f;
            for (int i = 0; i < indices.Count; i++)
            {
                if (b.boneIndex0 == indices[i])
                {
                    num += b.weight0;
                }
                if (b.boneIndex1 == indices[i])
                {
                    num += b.weight1;
                }
                if (b.boneIndex2 == indices[i])
                {
                    num += b.weight2;
                }
                if (b.boneIndex3 == indices[i])
                {
                    num += b.weight3;
                }
            }
            var result = (num >= threshold);
            return result;
        }

        private static bool FastContains(List<int> sortedNumbers, int value)
        {
            for (int i = 0; i < sortedNumbers.Count; i++)
            {
                if (sortedNumbers[i] == value)
                {
                    return true;
                }
                if (sortedNumbers[i] > value)
                {
                    return false;
                }
            }
            return false;
        }

        private static BoneWeight CheckInnerBoneWeight(BoneWeight bw, List<int> boneNumbers, int targetBoneIndex)
        {
            if (!ChopScript.FastContains(boneNumbers, bw.boneIndex0))
            {
                bw.boneIndex0 = targetBoneIndex;
            }
            if (!ChopScript.FastContains(boneNumbers, bw.boneIndex1))
            {
                bw.boneIndex1 = targetBoneIndex;
            }
            if (!ChopScript.FastContains(boneNumbers, bw.boneIndex2))
            {
                bw.boneIndex2 = targetBoneIndex;
            }
            if (!ChopScript.FastContains(boneNumbers, bw.boneIndex3))
            {
                bw.boneIndex3 = targetBoneIndex;
            }
            return bw;
        }

        private static BoneWeight CheckOuterBoneWeight(BoneWeight bw, List<int> boneNumbers, int parentBone)
        {
            if (ChopScript.FastContains(boneNumbers, bw.boneIndex0))
            {
                bw.boneIndex0 = parentBone;
            }
            if (ChopScript.FastContains(boneNumbers, bw.boneIndex1))
            {
                bw.boneIndex1 = parentBone;
            }
            if (ChopScript.FastContains(boneNumbers, bw.boneIndex2))
            {
                bw.boneIndex2 = parentBone;
            }
            if (ChopScript.FastContains(boneNumbers, bw.boneIndex3))
            {
                bw.boneIndex3 = parentBone;
            }
            return bw;
        }

        private static Vector3[] RandomizeEdgeVerts(Vector3[] verts, List<int> edgeIndices)
        {
            for (int i = 0; i < edgeIndices.Count; i++)
            {
                Vector3 val = verts[edgeIndices[i]];
                val += UnityEngine.Random.insideUnitSphere * 0.3f;
            }
            return verts;
        }

        private static Quaternion capOrientation(Vector3[] verts, List<int> edges)
        {
            int index = Mathf.FloorToInt((float) (edges.Count / 3));
            int index2 = Mathf.FloorToInt((float) (edges.Count * 2 / 3));
            Vector3 a = verts[edges[0]];
            Vector3 b = verts[edges[index]];
            Vector3 a2 = verts[edges[index2]];
            return Quaternion.LookRotation(Vector3.Cross(a - b, a2 - b));
        }

        public static Mesh CapMesh(Mesh parent, List<int> edges, bool facing, BoneWeight[] boneWeights)
        {
            if (edges.Count < 2)
            {
                return null;
            }
            int[] array = new int[edges.Count * 3 / 2];
            Vector2[] array2 = new Vector2[parent.uv.Length];
            Vector3[] array3 = new Vector3[parent.normals.Length];
            Vector3[] vertices = parent.vertices;
            Vector2 zero = Vector2.zero;
            Vector2 zero2 = Vector2.zero;
            Quaternion rotation = ChopScript.capOrientation(vertices, edges);
            for (int i = 0; i < edges.Count - 1; i += 2)
            {
                Vector3 vector = rotation * vertices[edges[i]];
                if (i == 0 || vector.x < zero[0])
                {
                    zero[0] = vector.x;
                }
                if (i == 0 || vector.x > zero[1])
                {
                    zero[1] = vector.x;
                }
                if (i == 0 || vector.y < zero2[0])
                {
                    zero2[0] = vector.y;
                }
                if (i == 0 || vector.y > zero2[1])
                {
                    zero2[1] = vector.y;
                }
                Vector3 vector2 = rotation * vertices[edges[i + 1]];
                if (i == 0 || vector2.x < zero[0])
                {
                    zero[0] = vector2.x;
                }
                if (i == 0 || vector2.x > zero[1])
                {
                    zero[1] = vector2.x;
                }
                if (i == 0 || vector2.y < zero2[0])
                {
                    zero2[0] = vector2.y;
                }
                if (i == 0 || vector2.y > zero2[1])
                {
                    zero2[1] = vector2.y;
                }
            }
            for (int j = 0; j < edges.Count - 1; j += 2)
            {
                array[j * 3 / 2] = edges[0];
                array[j * 3 / 2 + 1] = ((!facing) ? edges[j + 1] : edges[j]);
                array[j * 3 / 2 + 2] = ((!facing) ? edges[j] : edges[j + 1]);
                for (int k = 0; k < 3; k++)
                {
                    Vector3 vector3 = rotation * vertices[array[j * 3 / 2 + k]];
                    array2[array[j * 3 / 2 + k]] = new Vector2((vector3.x - zero[0]) / (zero[1] - zero[0]),
                        (vector3.y - zero2[0]) / (zero2[1] - zero2[0]));
                    array3[array[j * 3 / 2 + k]] =
                        ((!facing) ? (rotation * Vector3.forward) : (rotation * Vector3.back));
                }
            }
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.bindposes = parent.bindposes;
            mesh.boneWeights = boneWeights;
            mesh.triangles = array;
            mesh.uv = array2;
            mesh.RecalculateNormals();
            return mesh;
        }

        private static bool CheckSlicePos(int vert, Vector3[] bakedMeshVerts, Vector3 slicePos, Vector3 sliceNormal,
            Transform meshTransform, float sideOfParent)
        {
            Vector3 lhs = meshTransform.TransformPoint(bakedMeshVerts[vert]) - slicePos;
            if (Mathf.Sign(Vector3.Dot(lhs, sliceNormal)) != Mathf.Sign(sideOfParent))
            {
                return true;
            }
            return false;
        }

        private static void RecursivelyMoveChildrenOver(Choppable[] children, SkinnedMeshRenderer newMeshRenderer,
            GameObject newTransformParent, GameObject choppedBy)
        {
            if (children != null)
            {
                foreach (Choppable choppable in children)
                {
                    choppable.transform.parent = newTransformParent.transform;
                    choppable.meshRenderer = newMeshRenderer;
                    /*
                    if ((UnityEngine.Object)((Component)choppable).GetComponent<SeverableLimb>() != (UnityEngine.Object)null)
                    {
                        ((Component)choppable).GetComponent<SeverableLimb>().Sever(choppedBy);
                    }
                    */
                    ChopScript.RecursivelyMoveChildrenOver(choppable.children, newMeshRenderer, newTransformParent,
                        choppedBy);
                }
            }
        }

        public static bool ChopRagdoll(Choppable target,
            Material fill,
            Vector3 slicePoint,
            Vector3 sliceNormal,
            GameObject choppedBy)
        {
            float threshold = UnityEngine.Random.Range(0.4f, 0.6f);
            if (target == null || target.hasBeenChopped)
            {
                Debug.Log("Chop target is null or has already been chopped");
                return false;
            }

            SkinnedMeshRenderer skinnedMeshRenderer = null;
            SkinnedMeshRenderer meshRenderer = target.meshRenderer;
            Transform rootBone = target.bones[0];
            Mesh sharedMesh = meshRenderer.sharedMesh;
            int num = -1;
            int targetBoneIndex = -1;
            List<int> list = new List<int>();
            List<int> list2 = new List<int>();
            Transform[] bones = meshRenderer.bones;
            foreach (Transform item in target.bones)
            {
                for (int i = 0; i < bones.Length; i++)
                {
                    if (bones[i] == item)
                    {
                        list.Add(i);
                        foreach (var child in target.children)
                        {
                            foreach (Transform bone in child.bones)
                            {
                                for (int i2 = 0; i2 < bones.Length; i2++)
                                {
                                    if (bones[i2] == bone)
                                    {
                                        list.Add(i2);
                                    }
                                }
                            }
                        }
                    }
                    if (bones[i] == target.parentBoneTransform)
                    {
                        num = i;
                    }
                    if (bones[i] == target.bones[0])
                    {
                        targetBoneIndex = i;
                    }
                }
            }
            foreach (var child in target.children)
            {
                foreach (Transform item2 in child.bones)
                {
                    for (int j = 0; j < bones.Length; j++)
                    {
                        if (bones[j] == item2)
                        {
                            list2.Add(j);
                        }
                    }
                }
            }
            list2.Sort();
            list.Sort();

            BoneWeight[] boneWeights = sharedMesh.boneWeights;
            Mesh mesh = UnityEngine.Object.Instantiate(sharedMesh);
            Mesh mesh2 = UnityEngine.Object.Instantiate(sharedMesh);
            BoneWeight[] boneWeights2 = mesh2.boneWeights;
            BoneWeight[] boneWeights3 = mesh.boneWeights;
            List<int> list3 = new List<int>();
            Mesh mesh3 = new Mesh();
            float sideOfParent = 0f;
            Vector3[] bakedMeshVerts = null;
            if (slicePoint != Vector3.zero)
            {
                meshRenderer.BakeMesh(mesh3);
                bakedMeshVerts = mesh3.vertices;
                Vector3 lhs = target.parentBoneTransform.position - slicePoint;
                sideOfParent = Vector3.Dot(lhs, sliceNormal);
            }
            bool[] array = new bool[sharedMesh.vertexCount];
            bool[] array2 = new bool[sharedMesh.vertexCount];
            for (int k = 0; k < array.Length; k++)
            {
                array[k] = ChopScript.isPartOf(boneWeights[k], list, threshold);
                array2[k] = ChopScript.isPartOf(boneWeights[k], list2, threshold);
            }
            for (int l = 0; l < sharedMesh.subMeshCount; l++)
            {
                int[] triangles = sharedMesh.GetTriangles(l);
                List<int> list4 = new List<int>();
                List<int> list5 = new List<int>();
                for (int m = 0; m < triangles.Length; m += 3)
                {
                    bool flag = array[triangles[m]];
                    bool flag2 = array[triangles[m + 1]];
                    bool flag3 = array[triangles[m + 2]];
                    if (slicePoint != Vector3.zero && sliceNormal != Vector3.zero)
                    {
                        flag = (flag && (array2[triangles[m]] ||
                                         ChopScript.CheckSlicePos(triangles[m],
                                             bakedMeshVerts, slicePoint, sliceNormal,
                                             meshRenderer.transform, sideOfParent)));
                        flag2 = (flag2 && (array2[triangles[m + 1]] ||
                                           ChopScript.CheckSlicePos(triangles[m + 1],
                                               bakedMeshVerts, slicePoint, sliceNormal,
                                               meshRenderer.transform, sideOfParent)));
                        flag3 = (flag3 && (array2[triangles[m + 2]] ||
                                           ChopScript.CheckSlicePos(triangles[m + 2],
                                               bakedMeshVerts, slicePoint, sliceNormal,
                                               meshRenderer.transform, sideOfParent)));
                    }
                    if (flag || flag2 || flag3)
                    {
                        if (flag && !flag2 && !flag3)
                        {
                            list3.Add(triangles[m + 1]);
                            list3.Add(triangles[m + 2]);
                        }
                        if (!flag && flag2 && !flag3)
                        {
                            list3.Add(triangles[m + 2]);
                            list3.Add(triangles[m]);
                        }
                        if (!flag && !flag2 && flag3)
                        {
                            list3.Add(triangles[m]);
                            list3.Add(triangles[m + 1]);
                        }
                        list5.Add(triangles[m]);
                        boneWeights2[triangles[m]] =
                            ChopScript.CheckInnerBoneWeight(boneWeights2[triangles[m]],
                                list, targetBoneIndex);
                        list5.Add(triangles[m + 1]);
                        boneWeights2[triangles[m + 1]] =
                            ChopScript.CheckInnerBoneWeight(boneWeights2[triangles[m + 1]],
                                list, targetBoneIndex);
                        list5.Add(triangles[m + 2]);
                        boneWeights2[triangles[m + 2]] =
                            ChopScript.CheckInnerBoneWeight(boneWeights2[triangles[m + 2]],
                                list, targetBoneIndex);
                    }
                    else
                    {
                        list4.Add(triangles[m]);
                        list4.Add(triangles[m + 1]);
                        list4.Add(triangles[m + 2]);
                    }
                }
                for (int n = 0; n < boneWeights3.Length; n++)
                {
                    boneWeights3[n] = ChopScript.CheckOuterBoneWeight(boneWeights3[n], list, num);
                }
                mesh.SetTriangles(list4.ToArray(), l);
                mesh2.SetTriangles(list5.ToArray(), l);
            }
            if (list3.Count == 0)
            {
                UnityEngine.Debug.Log("Slice fail!");
                return false;
            }
            if (sliceNormal == Vector3.zero)
            {
                mesh2.vertices = ChopScript.RandomizeEdgeVerts(mesh2.vertices, list3);
                mesh.vertices = ChopScript.RandomizeEdgeVerts(mesh.vertices, list3);
            }

            var choppedGameObject = new GameObject("chopped " + target.name);
            //gameObject.AddComponent<DespawnTracker>();

            GameObject gameObject2 = UnityEngine.Object.Instantiate(target.meshRenderer.gameObject,
                target.meshRenderer.transform.position, target.meshRenderer.transform.rotation);
            gameObject2.name = target.meshRenderer.gameObject.name + " (mesh)";
            gameObject2.transform.parent = choppedGameObject.transform;
            mesh2.boneWeights = boneWeights2;
            mesh.boneWeights = boneWeights3;
            SkinnedMeshRenderer meshRenderer2 = target.meshRenderer;
            skinnedMeshRenderer = gameObject2.GetComponent<SkinnedMeshRenderer>();
            //((Component)skinnedMeshRenderer).GetComponent<CharacterDamageDisplay>().blendTex = ((Component)meshRenderer2).GetComponent<CharacterDamageDisplay>().blendTex;
            meshRenderer2.sharedMesh = mesh;
            skinnedMeshRenderer.sharedMesh = mesh2;
            skinnedMeshRenderer.rootBone = rootBone;
            //rootBone.transform.parent.parent = choppedGameObject.transform;

            //gameObject2.GetComponent<MeshFilter>().mesh = mesh2;

            // gameObject3: 子体切面
            // gameObject4: 本体切面
            GameObject gameObject3 = UnityEngine.Object.Instantiate(skinnedMeshRenderer.gameObject);
            GameObject gameObject4 = UnityEngine.Object.Instantiate(meshRenderer2.gameObject);
            gameObject3.transform.parent = skinnedMeshRenderer.transform.parent;
            gameObject3.name = skinnedMeshRenderer.gameObject.name + " (chopped)";
            gameObject4.transform.parent = meshRenderer2.transform.parent;
            gameObject4.name = meshRenderer2.gameObject.name + " (chopped)";
            gameObject3.GetComponent<SkinnedMeshRenderer>().sharedMesh =
                ChopScript.CapMesh(mesh2, list3, false, boneWeights2);
            gameObject4.GetComponent<SkinnedMeshRenderer>().sharedMesh =
                ChopScript.CapMesh(mesh, list3, true, boneWeights3);
            gameObject4.GetComponent<SkinnedMeshRenderer>().sharedMesh.boneWeights = boneWeights3;
            gameObject3.GetComponent<SkinnedMeshRenderer>().materials = new Material[1];
            gameObject3.GetComponent<SkinnedMeshRenderer>().material = fill;
            gameObject4.GetComponent<SkinnedMeshRenderer>().materials = new Material[1];
            gameObject4.GetComponent<SkinnedMeshRenderer>().material = fill;
            gameObject3.GetComponent<SkinnedMeshRenderer>().rootBone = rootBone;
            /*
            gameObject3.AddComponent<MeshBloodSpurter>().attachedTo = target.bones[0];
            target.parentSpurter = gameObject4.AddComponent<MeshBloodSpurter>();
            target.parentSpurter.attachedTo = bones[num];
            */
            var enemyRoot = target.GetComponentInParent<Enemy>();
            enemyRoot.RemoveMuscle(target, choppedGameObject);
            target.transform.parent = choppedGameObject.transform;
            target.meshRenderer = skinnedMeshRenderer;
            //UnityEngine.Object.Destroy(((Component)target).GetComponent<Joint>());
            /*
            if ((UnityEngine.Object)((Component)target).GetComponent<SeverableLimb>() != (UnityEngine.Object)null)
            {
                ((Component)target).GetComponent<SeverableLimb>().Sever(choppedBy);
            }
            */
            //ChopScript.RecursivelyMoveChildrenOver(target.children, skinnedMeshRenderer, choppedGameObject, choppedBy);
            /*
            if (slicePoint != Vector3.zero && sliceNormal != Vector3.zero && (UnityEngine.Object)((Component)target).GetComponent<Head>() != (UnityEngine.Object)null)
            {
                ((Component)target).GetComponent<Head>().GetChopped(slicePoint, sliceNormal, target.parentBoneTransform);
            }
            */
            target.hasBeenChopped = true;
            return true;
        }

        private static bool GibRagdoll(Choppable target, Material fill, Vector3 hitPoint, Vector3 hitForce,
            GameObject choppedBy)
        {
            float threshold = UnityEngine.Random.Range(0.4f, 0.6f);
            if ((UnityEngine.Object) target == (UnityEngine.Object) null)
            {
                return false;
            }
            SkinnedMeshRenderer meshRenderer = target.meshRenderer;
            Transform transform = target.bones[0];
            Mesh sharedMesh = meshRenderer.sharedMesh;
            int num = -1;
            int targetBoneIndex = -1;
            List<int> list = new List<int>();
            List<int> list2 = new List<int>();
            Transform[] bones = meshRenderer.bones;
            foreach (Transform item in target.bones)
            {
                for (int i = 0; i < bones.Length; i++)
                {
                    if ((UnityEngine.Object) bones[i] == (UnityEngine.Object) item)
                    {
                        list.Add(i);
                    }
                    if ((UnityEngine.Object) bones[i] == (UnityEngine.Object) target.parentBoneTransform)
                    {
                        num = i;
                    }
                    if ((UnityEngine.Object) bones[i] == (UnityEngine.Object) target.bones[0])
                    {
                        targetBoneIndex = i;
                    }
                }
            }
            foreach (var child in target.children)
            {
                foreach (Transform item2 in child.bones)
                {
                    for (int j = 0; j < bones.Length; j++)
                    {
                        if ((UnityEngine.Object) bones[j] == (UnityEngine.Object) item2)
                        {
                            list2.Add(j);
                        }
                    }
                }
            }
            list2.Sort();
            list.Sort();
            BoneWeight[] boneWeights = sharedMesh.boneWeights;
            Mesh mesh = UnityEngine.Object.Instantiate(sharedMesh);
            BoneWeight[] boneWeights2 = mesh.boneWeights;
            BoneWeight[] boneWeights3 = mesh.boneWeights;
            List<int> list3 = new List<int>();
            Mesh mesh2 = new Mesh();
            bool[] array = new bool[sharedMesh.vertexCount];
            bool[] array2 = new bool[sharedMesh.vertexCount];
            for (int k = 0; k < array.Length; k++)
            {
                array[k] = ChopScript.isPartOf(boneWeights[k], list, threshold);
                array2[k] = ChopScript.isPartOf(boneWeights[k], list2, threshold);
            }
            for (int l = 0; l < sharedMesh.subMeshCount; l++)
            {
                int[] triangles = sharedMesh.GetTriangles(l);
                List<int> list4 = new List<int>();
                List<int> list5 = new List<int>();
                for (int m = 0; m < triangles.Length; m += 3)
                {
                    bool flag = array[triangles[m]];
                    bool flag2 = array[triangles[m + 1]];
                    bool flag3 = array[triangles[m + 2]];
                    if (flag || flag2 || flag3)
                    {
                        if (flag && !flag2 && !flag3)
                        {
                            list3.Add(triangles[m + 1]);
                            list3.Add(triangles[m + 2]);
                        }
                        if (!flag && flag2 && !flag3)
                        {
                            list3.Add(triangles[m + 2]);
                            list3.Add(triangles[m]);
                        }
                        if (!flag && !flag2 && flag3)
                        {
                            list3.Add(triangles[m]);
                            list3.Add(triangles[m + 1]);
                        }
                        list5.Add(triangles[m]);
                        boneWeights2[triangles[m]] =
                            ChopScript.CheckInnerBoneWeight(boneWeights2[triangles[m]], list, targetBoneIndex);
                        list5.Add(triangles[m + 1]);
                        boneWeights2[triangles[m + 1]] =
                            ChopScript.CheckInnerBoneWeight(boneWeights2[triangles[m + 1]], list, targetBoneIndex);
                        list5.Add(triangles[m + 2]);
                        boneWeights2[triangles[m + 2]] =
                            ChopScript.CheckInnerBoneWeight(boneWeights2[triangles[m + 2]], list, targetBoneIndex);
                    }
                    else
                    {
                        list4.Add(triangles[m]);
                        list4.Add(triangles[m + 1]);
                        list4.Add(triangles[m + 2]);
                    }
                }
                for (int n = 0; n < boneWeights3.Length; n++)
                {
                    boneWeights3[n] = ChopScript.CheckOuterBoneWeight(boneWeights3[n], list, num);
                }
                mesh.SetTriangles(list4.ToArray(), l);
            }
            if (list3.Count == 0)
            {
                UnityEngine.Debug.Log("Slice fail!");
                return false;
            }
            mesh.vertices = ChopScript.RandomizeEdgeVerts(mesh.vertices, list3);
            mesh.boneWeights = boneWeights3;
            SkinnedMeshRenderer meshRenderer2 = target.meshRenderer;
            meshRenderer2.sharedMesh = mesh;
            GameObject gameObject = UnityEngine.Object.Instantiate(meshRenderer2.gameObject);
            //gameObject.transform.parent = meshRenderer2.transform.parent;
            gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh =
                ChopScript.CapMesh(mesh, list3, true, boneWeights3);
            gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh.boneWeights = boneWeights3;
            gameObject.GetComponent<SkinnedMeshRenderer>().materials = new Material[1];
            gameObject.GetComponent<SkinnedMeshRenderer>().material = fill;
            /*
            target.parentSpurter = gameObject.AddComponent<MeshBloodSpurter>();
            target.parentSpurter.attachedTo = bones[num];
            if ((UnityEngine.Object)((Component)target).GetComponent<SeverableLimb>() != (UnityEngine.Object)null)
            {
                ((Component)target).GetComponent<SeverableLimb>().Sever(choppedBy);
            }
            */
            //UnityEngine.Object.Destroy(target.gameObject);
            target.hasBeenChopped = true;
            target.gameObject.SetActive(false);
            return true;
        }

        public static bool Chop(Choppable target, Material fill, Vector3 slicePoint, Vector3 sliceNormal,
            GameObject choppedBy)
        {
            if (!target.canBeChopped) return false;
            return ChopRagdoll(target, fill, slicePoint, sliceNormal, choppedBy);
        }

        public static bool Gib(Choppable chop, Material fill, Vector3 hitPoint, Vector3 hitForce, GameObject gibbedBy)
        {
            if (!chop.canBeChopped) return false;
            return ChopScript.GibRagdoll(chop, fill, hitPoint, hitForce, gibbedBy);
        }
    }
}
