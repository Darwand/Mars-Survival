using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace World
{
    public enum WorldMaterial
    {
        NONE, Dirt, MAX
    }


    public class VoxelBlock : MonoBehaviour
    {
        VoxelChunk chunk;


        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                voxelMap[0, size - 1, 0] = WorldMaterial.NONE;
                MakeMesh();
            }
        }

        int posX;
        int posY;
        int posZ;

        WorldMaterial[,,] voxelMap;

        #region generator_vars

        static float peakHeight = 17;
        static float valleyHeight = 0;


        static int size = 6;

        static float dirtValue = .7f;

        static float xzxSeed = 0;
        static float xzySeed = 0;

        static float xyxSeed = 0;
        static float xyySeed = 0;

        #endregion

        #region mesh_references
        MeshFilter mf;
        MeshRenderer mr;
        Mesh ms;
        #endregion
        

        public static VoxelBlock MakeVoxelChunk(int x, int y, int z, VoxelChunk collection)
        {

            GameObject chunkObject = new GameObject("Voxel " + x + ", " + y + ", " + z);
            chunkObject.transform.position = new Vector3(x * size, y * size, z * size);

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

            ms = new Mesh();

            mr.material = Resources.Load<Material>("Sand 03/Sand pattern 03");

            mf.mesh = ms;
        }

        void Create()
        {
            InitComponents();
            

            //TODO thread this

            voxelMap = new WorldMaterial[size,size,size];

            for (int x = 0; x < size; x++)
            {
                for(int y = 0; y < size; y++)
                {
                    for(int z = 0; z < size; z++)
                    {
                        voxelMap[x, y, z] = CreateMaterialForVoxel(x, y, z);
                    }
                }
            }

            MakeMesh();

        }

        bool VoxPoint(int x, int y, int z)
        {
            return IsHole(GetMaterialForVoxel(x, y, z));
        }

        bool IsHole(WorldMaterial material)
        {
            return material == WorldMaterial.NONE || material == WorldMaterial.MAX;
        }

        bool VoxHasAdjacentHole( int x, int y, int z )
        {
            for(int cx = 1; cx >= -1; cx -= 1)
            {
                for (int cy = 1; cy >= -1; cy -= 1)
                {
                    for (int cz = 1; cz >= -1; cz -= 1)
                    {
                        //print("For vox : " + x + " : " + " : " + y + ": " + z);
                        //print("Checked vox : " + cx + " : " + " : " + cy + ": " + cz);

                        if(IsHole(GetMaterialForVoxel(x + cx, y + cy, z + cz)))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        void MakeMesh()
        {

            Dictionary<Position, int> vertPositions;
            Vector2[] uvs;
            Vector3[] verts = MakeVerts(out vertPositions, out uvs);

            int[] tris = MakeTris(verts, vertPositions);
            
            ms.vertices = verts;
            ms.uv = uvs;
            ms.triangles = tris;
        }

        Vector3[] MakeVerts( out Dictionary<Position, int> vertPositions, out Vector2[] uv )
        {
            List<Vector3> verts = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            Dictionary<Position, int> positions = new Dictionary<Position, int>();

            for (int x = 0; x < size; x++)
            {
                for(int y = 0; y < size; y++)
                {
                    for(int z = 0; z < size; z++)
                    {
                        if(VoxHasAdjacentHole(x,y,z))
                        {
                            positions.Add(new Position(x, y, z), verts.Count);
                            verts.Add(new Vector3(x, y, z));
                            uvs.Add(new Vector2(x + y, z + (y + 1)));
                        }
                    }
                }
            }

            vertPositions = positions;
            uv = uvs.ToArray();

            return verts.ToArray();
        }

        int[] MakeTris(Vector3[] verts, Dictionary<Position, int> vertPositions)
        {
            List<int> tris = new List<int>();

            HashSet<int> usedVerts = new HashSet<int>();

            for(int i = 0; i < verts.Length; i++)
            {
                Vector3 v = verts[i];

                List<int> adj = GetAdjecentVerts(Position.FromVector(v), vertPositions);

                for(int c = 0; c < adj.Count; c+=3)
                {
                    tris.Add(adj[c]);
                    tris.Add(adj[c + 1]);
                    tris.Add(adj[c + 2]);
                }

                usedVerts.Add(i);
            }

            return tris.ToArray();
        }
        bool VoxHasHoleInDir( int x, int y, int z, Vector3 dir )
        {
            Position offset = Position.FromVector(dir);

            return VoxPoint(x + offset.x, y + offset.y, z + offset.z);
        }


        bool MakeVertFor( int x, int y, int z, Vector3 dir )
        {
            return VoxelHasVert(x, y, z) && VoxHasHoleInDir(x, y, z, dir);
        }

        List<int> GetAdjecentVerts(Position pos, Dictionary<Position, int> vertPositions)
        {
            List<int> verts = new List<int>();

            int baseVert = vertPositions[pos];

            #region straights

            //forward faces

            //+x +y
            if (MakeVertFor(pos.x + 1, pos.y, pos.z, Vector3.forward) && MakeVertFor(pos.x, pos.y + 1, pos.z, Vector3.forward))
            {
                int v1 = vertPositions[new Position(pos.x + 1, pos.y, pos.z)];
                int v2 = vertPositions[new Position(pos.x, pos.y + 1, pos.z)];

                verts.Add(baseVert);
                verts.Add(v1);
                verts.Add(v2);
            }

            //-x -y
            if (MakeVertFor(pos.x - 1, pos.y, pos.z, Vector3.forward) && MakeVertFor(pos.x, pos.y - 1, pos.z, Vector3.forward))
            {
                int v1 = vertPositions[new Position(pos.x - 1, pos.y, pos.z)];
                int v2 = vertPositions[new Position(pos.x, pos.y - 1, pos.z)];

                verts.Add(baseVert);
                verts.Add(v1);
                verts.Add(v2);
            }

            //back faces

            //-x +y
            if (MakeVertFor(pos.x - 1, pos.y, pos.z, -Vector3.forward) && MakeVertFor(pos.x, pos.y + 1, pos.z, -Vector3.forward))
            {
                int v1 = vertPositions[new Position(pos.x - 1, pos.y, pos.z)];
                int v2 = vertPositions[new Position(pos.x, pos.y + 1, pos.z)];

                verts.Add(baseVert);
                verts.Add(v1);
                verts.Add(v2);
            }
            
            //+x -y
            if (MakeVertFor(pos.x + 1, pos.y, pos.z, -Vector3.forward) && MakeVertFor(pos.x, pos.y - 1, pos.z, -Vector3.forward))
            {
                int v1 = vertPositions[new Position(pos.x, pos.y - 1, pos.z)];
                int v2 = vertPositions[new Position(pos.x + 1, pos.y, pos.z)];

                verts.Add(baseVert);
                verts.Add(v2);
                verts.Add(v1);

            }


            //down faces

            //+x +z
            if (MakeVertFor(pos.x + 1, pos.y, pos.z, -Vector3.up) && MakeVertFor(pos.x, pos.y, pos.z + 1, -Vector3.up))
            {
                int v1 = vertPositions[new Position(pos.x + 1, pos.y, pos.z)];
                int v2 = vertPositions[new Position(pos.x, pos.y, pos.z + 1)];

                verts.Add(baseVert);
                verts.Add(v1);
                verts.Add(v2);
            }
            
            //-x -z
            if (MakeVertFor(pos.x - 1, pos.y, pos.z, -Vector3.up) && MakeVertFor(pos.x, pos.y, pos.z - 1, -Vector3.up))
            {
                int v1 = vertPositions[new Position(pos.x - 1, pos.y, pos.z)];
                int v2 = vertPositions[new Position(pos.x, pos.y, pos.z - 1)];

                verts.Add(baseVert);
                verts.Add(v1);
                verts.Add(v2);
            }



            //up faces
            
            //-x +z
            if (MakeVertFor(pos.x - 1, pos.y, pos.z, Vector3.up) && MakeVertFor(pos.x, pos.y, pos.z + 1, Vector3.up))
            {
                int v1 = vertPositions[new Position(pos.x - 1, pos.y, pos.z)];
                int v2 = vertPositions[new Position(pos.x, pos.y, pos.z + 1)];

                verts.Add(baseVert);
                verts.Add(v1);
                verts.Add(v2);
            }

            //+x -z
            if (MakeVertFor(pos.x + 1, pos.y, pos.z, Vector3.up) && MakeVertFor(pos.x, pos.y, pos.z - 1, Vector3.up))
            {
                int v1 = vertPositions[new Position(pos.x + 1, pos.y, pos.z)];
                int v2 = vertPositions[new Position(pos.x, pos.y, pos.z - 1)];

                verts.Add(baseVert);
                verts.Add(v1);
                verts.Add(v2);
            }


            //left faces

            //+z +y
            if (MakeVertFor(pos.x, pos.y, pos.z + 1, -Vector3.right) && MakeVertFor(pos.x, pos.y + 1, pos.z, -Vector3.right))
            {
                int v1 = vertPositions[new Position(pos.x, pos.y, pos.z + 1)];
                int v2 = vertPositions[new Position(pos.x, pos.y + 1, pos.z)];

                verts.Add(baseVert);
                verts.Add(v1);
                verts.Add(v2);
            }

            //-z -y
            if (MakeVertFor(pos.x, pos.y, pos.z - 1, -Vector3.right) && MakeVertFor(pos.x, pos.y - 1, pos.z, -Vector3.right))
            {
                int v1 = vertPositions[new Position(pos.x, pos.y, pos.z - 1)];
                int v2 = vertPositions[new Position(pos.x, pos.y - 1, pos.z)];

                verts.Add(baseVert);
                verts.Add(v1);
                verts.Add(v2);
            }


            //right faces
            
            //+z -y
            if (MakeVertFor(pos.x, pos.y, pos.z + 1, Vector3.right) && MakeVertFor(pos.x, pos.y - 1, pos.z, Vector3.right))
            {
                int v1 = vertPositions[new Position(pos.x, pos.y, pos.z + 1)];
                int v2 = vertPositions[new Position(pos.x, pos.y - 1, pos.z)];

                verts.Add(baseVert);
                verts.Add(v1);
                verts.Add(v2);
            }

            //-z +y
            if (MakeVertFor(pos.x, pos.y, pos.z - 1, Vector3.right) && MakeVertFor(pos.x, pos.y + 1, pos.z, Vector3.right))
            {
                int v1 = vertPositions[new Position(pos.x, pos.y, pos.z - 1)];
                int v2 = vertPositions[new Position(pos.x, pos.y + 1, pos.z)];

                verts.Add(baseVert);
                verts.Add(v1);
                verts.Add(v2);
            }



            #endregion

            return verts;
        }

        bool VoxelHasVert( int x, int y, int z )
        {
            if(!VoxHasAdjacentHole(x, y, z))
            {
                return false;
            }

            WorldMaterial mat = GetMaterialForVoxel(x, y, z);

            return mat != WorldMaterial.NONE && mat != WorldMaterial.MAX;
        }

        WorldMaterial GetMaterialForVoxel( int x, int y, int z )
        {
            VoxelBlock block = this;

            if (x < 0)
            {
                x = size - x;
                block = chunk.GetChunkAt(block.posX - 1, block.posY, block.posZ);
            }
            else if(x >= size)
            {
                x = x - size;
                block = chunk.GetChunkAt(block.posX + 1, block.posY, block.posZ);
            }

            if (block == null)
            {
                return WorldMaterial.MAX;
            }

            if (y < 0)
            {
                y = size - y;
                block = chunk.GetChunkAt(block.posX, block.posY - 1, block.posZ);
            }
            else if(y >= size)
            {
                y -= size;
                block = chunk.GetChunkAt(block.posX, block.posY + 1, block.posZ);
            }

            if (block == null)
            {
                return WorldMaterial.MAX;
            }

            if (z < 0)
            {
                z = size - z;
                block = chunk.GetChunkAt(block.posX, block.posY, block.posZ - 1);
            }
            else if(z >= size)
            {
                z -= size;
                block = chunk.GetChunkAt(block.posX, block.posY, block.posZ + 1);
            }
            
            if (block == null)
            {
                return WorldMaterial.MAX;
            }

            return block.voxelMap[x, y, z];
        }

        WorldMaterial CreateMaterialForVoxel(int x, int y, int z)
        {
            float w = GetWeightAtPosition(x, y, z);

            if(w < dirtValue)
            {
                return WorldMaterial.NONE;
            }
            else if(w >= dirtValue)
            {
                return WorldMaterial.Dirt;
            }

            return WorldMaterial.Dirt;
        }

        float GetWeightAtPosition( int x, int y, int z )
        {
            float height = GetHeight(x, z);

            float weight = 0;

            if(z <= height)
            {
                weight = height * GetDepthDenstity(x, y, z);
            }
            else
            {
                weight = height;
            }

            return weight;
        }

        float GetHeight( float x, float y )
        {

            float height = 0;
            float basePerlin = Mathf.PerlinNoise(xzxSeed + x, xzySeed + y);

            height = valleyHeight + (basePerlin * (peakHeight - valleyHeight));
            return height;
        }

        float GetDepthDenstity( int x, int y, int z )
        {
            return Mathf.PerlinNoise(xyxSeed + x, xyySeed + y);
        }
    }
}

