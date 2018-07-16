using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace World
{
    public class VoxelBlock : MonoBehaviour
    {
        VoxelChunk chunk;
        
        int posX;
        int posY;
        int posZ;

        public static readonly int size = 6;

        #region mesh_references
        MeshFilter mf;
        MeshRenderer mr;
        Mesh ms;
        #endregion
        

        public static VoxelBlock MakeVoxelChunk(int x, int y, int z, VoxelChunk collection)
        {

            GameObject chunkObject = new GameObject("Voxel " + x + ", " + y + ", " + z);
            chunkObject.transform.position = new Vector3(0,0,0);

            VoxelBlock vox = chunkObject.AddComponent<VoxelBlock>();

            vox.chunk = collection;
            
            vox.posX = x;
            vox.posY = y;
            vox.posZ = z;

            //TODO add a create call for specific chunks
            vox.Create();


            return vox;
        }

        void InitComponents()
        {
            mf = gameObject.AddComponent<MeshFilter>();
            mr = gameObject.AddComponent<MeshRenderer>();

            ms = new Mesh()
            {
                name = "Voxel Mesh"
            };

            mr.material = Resources.Load<Material>("Sand 03/Sand pattern 03");

            mf.mesh = ms;
        }

        void Create()
        {
            InitComponents();

            MakeMesh();
        }

        void Update()
        {
            
        }
        void MakeMesh()
        {

            
            int[] tris;
            Vector2[] uvs;
            Vector3[] norms;
            Vector3[] verts = ProcessVoxels(out uvs, out tris, out norms);

            
            ms.vertices = verts;
            ms.uv = uvs;
            ms.normals = norms;
            ms.triangles = tris;

            print("Verticies: " + ms.vertices.Length);
            print("Triangles: " + (ms.triangles.Length / 3));
        }

        Vector3[] ProcessVoxels(out Vector2[] uvs, out int[] triangles, out Vector3[] normals)
        {
            Dictionary<Vector3, int> vertPositions = new Dictionary<Vector3, int>();

            Vector2[] uv;
            Vector3[] norms;

            Vector3[] verts = MakeVerts(out vertPositions, out uv, out norms);
            int[] tris = MakeTris(verts, vertPositions, norms);


            triangles = tris;
            uvs = uv;
            normals = norms;

            return verts;
        }
        
        Vector3[] MakeVerts(out Dictionary<Vector3, int> vertPositions, out Vector2[] uv, out Vector3[] normals)
        {
            List<Vector3> verts = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector3> norms = new List<Vector3>();
            HashSet<Vector3> addedVerts = new HashSet<Vector3>();
            Dictionary<Vector3, int> vertMap = new Dictionary<Vector3, int>();

            int offSetX = posX * size;
            int offSetY = posY * size;
            int offSetZ = posZ * size;

            for (int x = offSetX; x < size + offSetX + 1; x++)
            {
                for (int y = offSetY; y < size + offSetY + 1; y++)
                {
                    for (int z = offSetZ; z < size + offSetZ + 1; z++)
                    {
                        if(!VoxIsHole(x, y, z))
                        {
                            for (int cx = 1; cx >= -1; cx -= 2)
                            {
                                if (VoxIsHole(x + cx, y, z))
                                {
                                    Vector3 v = new Vector3(x + (cx / 2f), y, z);

                                    if (!addedVerts.Contains(v))
                                    {
                                        addedVerts.Add(v);
                                        Vector2 u = new Vector2(v.x + v.y, v.z + (v.y - 1));
                                        vertMap.Add(v, verts.Count);
                                        verts.Add(v);
                                        uvs.Add(u);

                                        norms.Add(new Vector3(cx, 0, 0));
                                    }

                                }
                            }

                            for (int cy = 1; cy >= -1; cy -= 2)
                            {
                                if (VoxIsHole(x, y + cy, z))
                                {
                                    Vector3 v = new Vector3(x, y + (cy / 2f), z);

                                    if (!addedVerts.Contains(v))
                                    {
                                        addedVerts.Add(v);
                                        Vector2 u = new Vector2(v.x + v.y, v.z + (v.y - 1));
                                        vertMap.Add(v, verts.Count);
                                        verts.Add(v);
                                        uvs.Add(u);

                                        norms.Add(new Vector3(0, cy, 0));
                                    }
                                }
                            }

                            for (int cz = 1; cz >= -1; cz -= 2)
                            {
                                if (VoxIsHole(x, y, z + cz))
                                {
                                    Vector3 v = new Vector3(x, y, z + (cz / 2f));

                                    if (!addedVerts.Contains(v))
                                    {
                                        addedVerts.Add(v);
                                        Vector2 u = new Vector2(v.x + v.y, v.z + (v.y - 1));
                                        vertMap.Add(v, verts.Count);
                                        verts.Add(v);
                                        uvs.Add(u);

                                        norms.Add(new Vector3(0, 0, cz));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            normals = norms.ToArray();
            uv = uvs.ToArray();
            vertPositions = vertMap;
            return verts.ToArray();
        }

        HashSet<Vector3> GetCorrectDirecions()
        {
            HashSet<Vector3> hs = new HashSet<Vector3>();

            hs.Add(Vector3.down);
            hs.Add(Vector3.forward);
            hs.Add(Vector3.right);

            Vector3 vec = new Vector3(0, 2, 2);
            vec = Vector3.ClampMagnitude(vec, 1);
            hs.Add(vec);


            vec = new Vector3(2, 0, 2);
            vec = Vector3.ClampMagnitude(vec, 1);
            hs.Add(vec);

            vec = new Vector3(2, -2, 0);
            vec = Vector3.ClampMagnitude(vec, 1);
            hs.Add(vec);

            vec = new Vector3(0, -2, 2);
            vec = Vector3.ClampMagnitude(vec, 1);
            hs.Add(vec);

            vec = new Vector3(-2, -2, 0);
            vec = Vector3.ClampMagnitude(vec, 1);
            hs.Add(vec);

            vec = new Vector3(-2, 0, 2);
            vec = Vector3.ClampMagnitude(vec, 1);
            hs.Add(vec);

            vec = new Vector3(-1, 1, -1);
            vec = Vector3.ClampMagnitude(vec, 1);
            hs.Add(vec);

            vec = new Vector3(1, 1, 1);
            vec = Vector3.ClampMagnitude(vec, 1);
            hs.Add(vec);

            vec = new Vector3(1, -1, 1);
            vec = Vector3.ClampMagnitude(vec, 1);
            hs.Add(vec);

            vec = new Vector3(-1, -1, -1);
            vec = Vector3.ClampMagnitude(vec, 1);
            hs.Add(vec);

            return hs;
        }

        int[] MakeTris(Vector3[] verts, Dictionary<Vector3, int> vertPositions, Vector3[] normals)
        {
            List<int> tris = new List<int>();
            int[] faceVerts = new int[16];
            Vector3[] vertsInFace = new Vector3[16];

            Vector3 checkPoint = Vector3.zero;
            Vector3 modifer = Vector3.zero;
            int vCount;

            HashSet<Vector3> correctOrderDirections = GetCorrectDirecions();

            Vector3 offSet = new Vector3(posX * size - 1, posY * size - 1, posZ * size - 1);

            for(checkPoint.x = -.5f; checkPoint.x <= size + .5f; checkPoint.x++)
            {
                for (checkPoint.y = -.5f; checkPoint.y <= size + .5f; checkPoint.y++)
                {
                    for (checkPoint.z = -.5f; checkPoint.z <= size + .5f; checkPoint.z++)
                    {
                        vCount = 0;

                        for(modifer.x = .5f; modifer.x >= -.5f; modifer.x -= .5f)
                        {
                            for (modifer.y = .5f; modifer.y >= -.5f; modifer.y -= .5f)
                            {
                                for (modifer.z = .5f; modifer.z >= -.5f; modifer.z -= .5f)
                                {
                                    
                                    if(vertPositions.ContainsKey(checkPoint + modifer + offSet))
                                    {
                                        faceVerts[vCount++] = vertPositions[checkPoint + modifer + offSet];
                                    }

                                }
                            }
                        }
                        
                        if(vCount == 4)
                        {

                            Vector3 faceNormal = normals[faceVerts[0]] + normals[faceVerts[1]] + normals[faceVerts[2]] + normals[faceVerts[3]];
                            faceNormal = Vector3.ClampMagnitude(faceNormal, 1);

                            if(correctOrderDirections.Contains(faceNormal))
                            {
                                tris.Add(faceVerts[0]);
                                tris.Add(faceVerts[2]);
                                tris.Add(faceVerts[3]);

                                tris.Add(faceVerts[3]);
                                tris.Add(faceVerts[1]);
                                tris.Add(faceVerts[0]);
                            }
                            else
                            {
                                tris.Add(faceVerts[3]);
                                tris.Add(faceVerts[2]);
                                tris.Add(faceVerts[0]);

                                tris.Add(faceVerts[0]);
                                tris.Add(faceVerts[1]);
                                tris.Add(faceVerts[3]);
                            }
                        }
                        else if(vCount == 3)
                        {

                            Vector3 faceNormal = normals[faceVerts[0]] + normals[faceVerts[1]] + normals[faceVerts[2]];
                            faceNormal = Vector3.ClampMagnitude(faceNormal, 1);

                            if(correctOrderDirections.Contains(faceNormal))
                            {
                                tris.Add(faceVerts[0]);
                                tris.Add(faceVerts[1]);
                                tris.Add(faceVerts[2]);
                            }
                            else
                            {
                                tris.Add(faceVerts[2]);
                                tris.Add(faceVerts[1]);
                                tris.Add(faceVerts[0]);
                            }
                        }

                    }
                }
            }

           #region old
            //for (int i = 0; i < verts.Length; i++)
            //{
            //    int vi = vertPositions[verts[i]];
            //    Vector3 vv = verts[i];
            //    Vector3 n = normals[i];

            //    Vector3 voxPosition = vv - (.5f * n);
                
            //    int vCount = 0;

            //    Vector3 query;

            //    vertsInFace[vCount] = vv;
            //    faceVerts[vCount++] = vi;

            //    if (n.x != 0)
            //    {
            //        query = voxPosition + (n.x * new Vector3(0, .5f, 0));
            //        if (vertPositions.ContainsKey(query))
            //        {
            //            vertsInFace[vCount] = query;
            //            faceVerts[vCount++] = vertPositions[query];
            //        }

            //        query = voxPosition + (n.x * new Vector3(0, 0, .5f));
            //        if (vertPositions.ContainsKey(query))
            //        {
            //            vertsInFace[vCount] = query;
            //            faceVerts[vCount++] = vertPositions[query];
            //        }
            //    }

            //    if (n.y != 0)
            //    {
            //        query = voxPosition + (n.y * new Vector3(.5f, 0, 0));
            //        if (vertPositions.ContainsKey(query))
            //        {
            //            vertsInFace[vCount] = query;
            //            faceVerts[vCount++] = vertPositions[query];
            //        }

            //        query = voxPosition + (n.y * new Vector3(0, 0, .5f));
            //        if (vertPositions.ContainsKey(query))
            //        {
            //            vertsInFace[vCount] = query;
            //            faceVerts[vCount++] = vertPositions[query];
            //        }
            //    }

            //    if (n.z != 0)
            //    {
            //        query = voxPosition + (n.z * new Vector3(.5f, 0, 0));
            //        if (vertPositions.ContainsKey(query))
            //        {
            //            vertsInFace[vCount] = query;
            //            faceVerts[vCount++] = vertPositions[query];
            //        }

            //        query = voxPosition + (n.z * new Vector3(0, .5f, 0));
            //        if (vertPositions.ContainsKey(query))
            //        {
            //            vertsInFace[vCount] = query;
            //            faceVerts[vCount++] = vertPositions[query];
            //        }
            //    }


            //    if (vCount == 3)
            //    {
            //        //vert is part of corner
            //        tris.Add(faceVerts[0]);
            //        tris.Add(faceVerts[1]);
            //        tris.Add(faceVerts[2]);

            //    }
            //    else if (vCount == 2)
            //    {
            //        continue;
            //        //slope

            //        Vector3 dir = new Vector3(1, 1, 1);
            //        dir -= normals[vi];
            //        dir -= normals[faceVerts[1]];

            //        dir.x = Mathf.Clamp(dir.x, -1, 1);
            //        dir.y = Mathf.Clamp(dir.y, -1, 1);
            //        dir.z = Mathf.Clamp(dir.z, -1, 1);

            //        query = voxPosition + dir;

            //        if (vertPositions.ContainsKey(query))
            //        {
            //            faceVerts[vCount++] = vertPositions[query];
            //        }

            //        query = vertsInFace[1] + dir;

            //        if (vertPositions.ContainsKey(query))
            //        {
            //            faceVerts[vCount++] = vertPositions[query];
            //        }

            //        if (vCount == 3)
            //        {
            //            tris.Add(vi);
            //            tris.Add(faceVerts[1]);
            //            tris.Add(faceVerts[0]);

            //            tris.Add(vi);
            //            tris.Add(faceVerts[1]);
            //            tris.Add(faceVerts[2]);
            //        }
            //        else
            //        {
            //            Debug.LogWarning("Failed to make slope");
            //        }
            //    }
            //    else if (vCount == 1)
            //    {
            //        //wall/floor

            //        Vector3 dir1;
            //        Vector3 dir2;

            //        if (n.x != 0)
            //        {
            //            dir1 = new Vector3(0, 1, 0);
            //            dir2 = new Vector3(0, 0, 1);
            //        }
            //        else if (n.y != 0)
            //        {
            //            dir1 = new Vector3(1, 0, 0);
            //            dir2 = new Vector3(0, 0, 1);
            //        }
            //        else if (n.z != 0)
            //        {
            //            dir1 = new Vector3(1, 0, 0);
            //            dir2 = new Vector3(0, 1, 0);
            //        }
            //        else
            //        {
            //            dir1 = Vector3.zero;
            //            dir2 = dir1;
            //            Debug.LogError("Normal is not straight");
            //        }

            //        query = vv + dir1;
            //        if (vertPositions.ContainsKey(query))
            //        {
            //            faceVerts[vCount++] = vertPositions[query];
            //        }

            //        query = vv + dir2;
            //        if (vertPositions.ContainsKey(query))
            //        {
            //            faceVerts[vCount++] = vertPositions[query];
            //        }

            //        query = vv + dir1 + dir2;
            //        if (vertPositions.ContainsKey(query))
            //        {
            //            faceVerts[vCount++] = vertPositions[query];
            //        }

            //        if (vCount == 4)
            //        {
            //            if (n == Vector3.right || n == Vector3.forward || n == Vector3.down)
            //            {
            //                tris.Add(faceVerts[0]);
            //                tris.Add(faceVerts[1]);
            //                tris.Add(faceVerts[3]);

            //                tris.Add(faceVerts[0]);
            //                tris.Add(faceVerts[3]);
            //                tris.Add(faceVerts[2]);
            //            }
            //            else
            //            {
            //                tris.Add(faceVerts[1]);
            //                tris.Add(faceVerts[0]);
            //                tris.Add(faceVerts[3]);

            //                tris.Add(faceVerts[3]);
            //                tris.Add(faceVerts[0]);
            //                tris.Add(faceVerts[2]);
            //            }

            //        }
            //        else
            //        {
            //            Debug.LogWarning("Failed to make wall or floor for " + vi);
            //        }
            //    }
            //    else
            //    {
            //        Debug.LogWarning("Vert with " + vCount + ". Unsure what to do");
            //    }
            //}
            #endregion

            return tris.ToArray();
        }

        int GetVertsForVoxel(int x, int y, int z, out Vector3[] verts, Dictionary<Vector3, int> vertPositions, Vector3[] normals)
        {
            int count = 0;
            Vector3[] v = new Vector3[8];

            Vector3 voxPosition = new Vector3(x, y, z);
            Vector3 q;

            if (!VoxIsHole(x, y, z))
            {
                for(int cx = 1; cx >= -1; cx -= 2)
                {
                    q = voxPosition + new Vector3(cx / 2f, 0, 0);

                    if (vertPositions.ContainsKey(q))
                    {
                        v[count++] = q;
                    }
                }

                for (int cy = 1; cy >= -1; cy -= 2)
                {
                    q = voxPosition + new Vector3(0, cy / 2f, 0);

                    if (vertPositions.ContainsKey(q))
                    {
                        v[count++] = q;
                    }
                }

                for (int cz = 1; cz >= -1; cz -= 2)
                {
                    q = voxPosition + new Vector3(0, 0, cz / 2f);

                    if (vertPositions.ContainsKey(q))
                    {
                        v[count++] = q;
                    }
                }
            }


            verts = v;
            return count;
        }

        void ProcessVoxel(int x, int y, int z, byte config, List<Vector3> verts, List<Vector2> uvs, Dictionary<Vector3, int> vertMap, List<int> triangles)
        {
            switch(config)
            {

                //empty hole
                case 0:
                case 255:
                    return;

                case 240:

                    Vector3 vert = new Vector3(x, y + .5f, z);
                    Vector2 uv = new Vector2(vert.x + vert.y, vert.z + (1 - vert.y));

                    if(!vertMap.ContainsKey(vert))
                    {
                        vertMap.Add(vert, verts.Count);
                        verts.Add(vert);
                        uvs.Add(uv);
                    }

                    triangles.Add(vertMap[vert]);
                    int s = vertMap[vert];

                    vert = new Vector3(x, y + .5f, z + 1);
                    uv = new Vector2(vert.x + vert.y, vert.z + (1 - vert.y));

                    if (!vertMap.ContainsKey(vert))
                    {
                        vertMap.Add(vert, verts.Count);
                        verts.Add(vert);
                        uvs.Add(uv);
                    }

                    triangles.Add(vertMap[vert]);

                    vert = new Vector3(x + 1, y + .5f, z + 1);
                    uv = new Vector2(vert.x + vert.y, vert.z + (1 - vert.y));

                    if (!vertMap.ContainsKey(vert))
                    {
                        vertMap.Add(vert, verts.Count);
                        verts.Add(vert);
                        uvs.Add(uv);
                    }

                    triangles.Add(vertMap[vert]);
                    triangles.Add(vertMap[vert]);

                    vert = new Vector3(x + 1, y + .5f, z);
                    uv = new Vector2(vert.x + vert.y, vert.z + (1 - vert.y));

                    if (!vertMap.ContainsKey(vert))
                    {
                        vertMap.Add(vert, verts.Count);
                        verts.Add(vert);
                        uvs.Add(uv);
                    }
                    triangles.Add(vertMap[vert]);
                    triangles.Add(s);

                    return;
            }
        }

        bool MakeVertFor( int x, int y, int z, Vector3 dir )
        {
            return VoxelHasVert(x, y, z);
        }

        List<int> GetTrisForCube(Position pos, Dictionary<Position, int> vertPositions )
        {
            List<int> tris = new List<int>();

            return tris;

        }

        List<int> GetAdjecentVerts(Position pos, Dictionary<Position, int> vertPositions)
        {
            List<int> verts = new List<int>();

            int baseVert = vertPositions[pos];

            

            return verts;
        }

        bool VoxelHasVert( int x, int y, int z )
        {
            return false;
        }

        bool VoxIsHole( int x, int y, int z )
        {
            return VoxelChunk.MaterialIsHole(chunk.GetMaterialForVoxel(x, y, z));// != WorldMaterial.NONE;
        }

        private static byte ConvertBoolArrayToByte( bool[] source )
        {
            byte result = 0;
            // This assumes the array never contains more than 8 elements!
            int index = 8 - source.Length;

            // Loop through the array
            foreach (bool b in source)
            {
                // if the element is 'true' set the bit at that position
                if (b)
                    result |= (byte)(1 << (7 - index));

                index++;
            }

            return result;
        }
    }
}

